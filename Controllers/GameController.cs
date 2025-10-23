using FyraIRad.Models;
using Microsoft.AspNetCore.Mvc;

namespace FyraIRad.Controllers
{
    public class GameController : Controller
    {
        private readonly UserMethods userMethods;
        private readonly GameMethods gameMethods;
        private readonly MoveMethods moveMethods;
        private object currentUserId;

        public GameController(IConfiguration configuration)
        {
            userMethods = new UserMethods(configuration);
            gameMethods = new GameMethods(configuration);
            moveMethods = new MoveMethods(configuration);
        }
        //Starta spel, sätt spelstatus till waiting för att andra spelare ska kunna gå med
        [HttpPost]
        public IActionResult StartGame(int id)
        {
            GameDetails newGame = new GameDetails();

            newGame.playerRedId = (int)HttpContext.Session.GetInt32("UserId");
            newGame.playerYellowId = id;
            newGame.CurrentTurn = 'Y';
            newGame.Status = "Waiting";
            gameMethods.CreateGame(newGame);
            ViewBag.GameStatus = "Waiting";

            List<GameDetails> gameList = new List<GameDetails>();
            gameList = gameMethods.GetGameDetails(out string errormsg);
            newGame = gameList.Last();

           return RedirectToAction("ActiveGame", new { gameId = newGame.GameId });
           
        }
        //Joina spel. Kontroll att spelare kan joina, vilken spelare som joinar/startat spelet samt ändrar status till aktiv
        [HttpPost]
        public IActionResult JoinGame(int id)
        {
            List<GameDetails> gameList = new List<GameDetails>();
            gameList = gameMethods.GetGameDetails(out string errormsg);

            for (int i = 0; i < gameList.Count(); i++)
            {
                if ((gameList[i].playerRedId == id && gameList[i].playerYellowId == HttpContext.Session.GetInt32("UserId")) || (gameList[i].playerYellowId == id && gameList[i].playerRedId == HttpContext.Session.GetInt32("UserId")))
                {
                    if (gameList[i].Status == "Waiting" && gameList[i].playerYellowId == HttpContext.Session.GetInt32("UserId"))
                    {
                        gameList[i].Status = "Active";
                        
                    }
                    gameMethods.UpdateGameStatus(gameList[i]);

                    return RedirectToAction("ActiveGame", new { gameId = gameList[i].GameId });

                }
            }

            ViewBag.JoinError = "Could not find game to join";
            return RedirectToAction("ShowUserList", "LogIn");
        }
        //Aktiva spelet. Skickar data för spelare/färg, spelstatus, vinnare och gameID
        public IActionResult ActiveGame(int gameId)
        {
            GameDetails game = gameMethods.GetGameById(gameId, out string errormsg);

            if (game.Status != "Active")
            {
                ViewBag.MoveError = "Game not active";
            }
           
            ViewBag.GameStatus = game.Status;
            ViewBag.Winner = game.Winner;


            List<MoveDetails> moveList = moveMethods.GetMoveDetailsForGame(gameId, out string moveErr);
            ViewBag.MoveList = moveList;
            ViewBag.GameId = game.GameId;

            int currentUserID = (int)HttpContext.Session.GetInt32("UserId");

            List<UserDetails> allUsers = userMethods.GetUserDetails(out string userErr);

            UserDetails redPlayer = allUsers.FirstOrDefault(u => u.UserId == game.playerRedId);
            UserDetails yellowPlayer = allUsers.FirstOrDefault(u => u.UserId == game.playerYellowId);

            ViewBag.RedPlayerName = redPlayer != null ? redPlayer.Username : "Röd spelare";
            ViewBag.YellowPlayerName = yellowPlayer != null ? yellowPlayer.Username : "Gul spelare";
            ViewBag.CurrentTurn = game.CurrentTurn;

            return View(game);
        }
        //Skapa drag, tar in vilket spel samt vilken kolumn disken droppas i
        [HttpPost]
        public IActionResult CreateMove(int gameId, int column)
        {
            GameDetails currentGame = gameMethods.GetGameById(gameId, out string errormsg);

            char playerColor;

            // Kollar vilken färg spelaren har och sätter variabel till det
            if (currentGame.playerRedId == HttpContext.Session.GetInt32("UserId"))
            {
                playerColor = 'R';
            }
            else if (currentGame.playerYellowId == HttpContext.Session.GetInt32("UserId"))
            {
                playerColor = 'Y';
            }
            else
            {
                ViewBag.MoveError = "You are not a player in this game";
                return RedirectToAction("ActiveGame", new { gameId = currentGame.GameId });
            }
            // Kollar ifall det är den spelaren som försöker göra draget och lägger draget i newMove
            if (currentGame.CurrentTurn == playerColor && currentGame.Status == "Active")
            {
                MoveDetails newMove = new MoveDetails();
                newMove.GameId = gameId;
                newMove.ColumnIndex = column;

                List<MoveDetails> moveList = new List<MoveDetails>();
                moveList = moveMethods.GetMoveDetailsForGame(gameId, out string errorMsg);
                moveList = moveList.Where(m => m.ColumnIndex == column).ToList();

                int newRowIndex = 1;

               
                //Kontroll om en column är full

                if (moveList.Count() >= 6)
                {
                    ViewBag.MoveError = "Column is full, choose another column";
                    return RedirectToAction("ActiveGame", new { gameId = currentGame.GameId });
                }
                else if (moveList.Count() >= 1)
                {
                    newRowIndex = moveList.Min(m => m.RowIndex) - 1;
                }
                else
                {
                    newRowIndex = 6;
                }

                newMove.RowIndex = newRowIndex;

                //Lägger till spelare och färg baserat på vems tur det är till newMove

                if (currentGame.CurrentTurn == 'R')
                {
                    newMove.PlayerId = currentGame.playerRedId;
                    newMove.DiscColor = 'R';
                }
                else
                {
                    newMove.PlayerId = currentGame.playerYellowId;
                    newMove.DiscColor = 'Y';
                }
                moveMethods.CreateMove(newMove);
                // Switch turn
                if (currentGame.CurrentTurn == 'R')
                {
                    currentGame.CurrentTurn = 'Y';
                }
                else
                {
                    currentGame.CurrentTurn = 'R';
                }
                gameMethods.UpdateCurrentTurn(currentGame);

                // Tar in alla moves och skapar ett 7x6 bräde med moves för att kolla vinnare
                List<MoveDetails> allMoves = moveMethods.GetMoveDetailsForGame(gameId, out string allMovesErrorMsg);
                char[,] board = new char[7, 6];
                foreach (var move in allMoves)
                {
                    int row = move.RowIndex - 1;
                    int col = move.ColumnIndex - 1;
                    board[col, row] = move.DiscColor;
                }

                //Anropar checkwinner för att se om någon vunnit och uppdaterar spelet i så fall
                char winner = CheckWinner(board);
                if (winner == 'R' || winner == 'Y')
                {
                    // Mark game as finished, set winner
                    currentGame.Status = "Finished";
                    currentGame.Winner = winner;
                    gameMethods.UpdateGameStatus(currentGame);
                    gameMethods.UpdateWinner(currentGame);
                    return RedirectToAction("ActiveGame", new { gameId = currentGame.GameId });
                }

                // Kontroll om brädet är fullt för oavgjort
                bool isDraw = true;
                for (int c = 0; c < 7; c++)
                {
                    for (int r = 0; r < 6; r++)
                    {
                        if (board[c, r] == '\0')
                        {
                            isDraw = false;
                            break;
                        }
                    }
                    if (!isDraw) break;
                }

                // Om det är oavgjort, uppdatera spelet
                if (isDraw)
                {
                    currentGame.Status = "Draw";
                    currentGame.Winner = null;
                    gameMethods.UpdateGameStatus(currentGame);
                    gameMethods.UpdateWinner(currentGame);
                    return RedirectToAction("ActiveGame", new { gameId = currentGame.GameId });
                }

                return RedirectToAction("ActiveGame", new { gameId = currentGame.GameId });
            }
            else
            {
                ViewBag.MoveError = "It's not your turn";
                return RedirectToAction("ActiveGame", new { gameId = currentGame.GameId });
            }
        }
        [HttpPost]
        public IActionResult SurrenderGame(int gameId)
        {
        GameDetails game = gameMethods.GetGameById(gameId, out string errormsg);

        if (game != null)
            {
                // Om du vill markera som 'Surrendered' i databasen först (valfritt)
                game.Status = "Surrender";
                gameMethods.UpdateGameStatus(game);
                if (game.playerRedId == HttpContext.Session.GetInt32("UserId"))
                {
                    game.Winner = 'Y';
                }
                else if (game.playerYellowId == HttpContext.Session.GetInt32("UserId"))
                {
                    game.Winner = 'R';
                }
                gameMethods.UpdateWinner(game);
            }
            return RedirectToAction("ActiveGame", new { gameId = game.GameId });
}


        // Logik för att kontrollera om det finns vinnare på brädet
        private char CheckWinner(char[,] board)
        {
            int rows = 6, cols = 7;
            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    char color = board[c, r];
                    if (color == '\0') continue;

                    // Horisontell check
                    if (c <= cols - 4 && Enumerable.Range(0, 4).All(i => board[c + i, r] == color))
                        return color;
                    // Vertikal check
                    if (r <= rows - 4 && Enumerable.Range(0, 4).All(i => board[c, r + i] == color))
                        return color;
                    // Diagonal "/" check
                    if (c <= cols - 4 && r >= 3 && Enumerable.Range(0, 4).All(i => board[c + i, r - i] == color))
                        return color;
                    // Diagonal "\" check
                    if (c <= cols - 4 && r <= rows - 4 && Enumerable.Range(0, 4).All(i => board[c + i, r + i] == color))
                        return color;
                }
            }
            return '\0';
        }
        //Delete game eller markera som borttagen efter vinst/oavgjort
        public IActionResult DeleteGame(int gameId)
        {
            GameDetails game = gameMethods.GetGameById(gameId, out string errormsg);
            if (game.CurrentTurn == 'D')
            {
                gameMethods.DeleteGame(game);
            }
            else
            {
                game.CurrentTurn = 'D';
                HttpContext.Session.SetInt32("EndedGame", gameId);
                gameMethods.UpdateCurrentTurn(game);
            }
                return RedirectToAction("ShowUserList", "LogIn");
        }

    }
}
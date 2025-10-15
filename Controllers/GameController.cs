using FyraIRad.Models;
using Microsoft.AspNetCore.Mvc;

namespace FyraIRad.Controllers
{
    public class GameController : Controller
    {
        private readonly UserMethods userMethods;
        private readonly GameMethods gameMethods;
        private readonly MoveMethods moveMethods;

        public GameController(IConfiguration configuration)
        {
            userMethods = new UserMethods(configuration);
            gameMethods = new GameMethods(configuration);
            moveMethods = new MoveMethods(configuration);
        }

        [HttpPost]
        public IActionResult StartGame(int id)
        {
            GameDetails newGame = new GameDetails();

            newGame.playerRedId = (int)HttpContext.Session.GetInt32("UserId");
            newGame.playerYellowId = id;
            newGame.CurrentTurn = 'Y';
            newGame.Status = "Waiting";
            gameMethods.CreateGame(newGame);

            List<GameDetails> gameList = new List<GameDetails>();
            gameList = gameMethods.GetGameDetails(out string errormsg);
            newGame = gameList.Last();

            

            return RedirectToAction("ActiveGame", newGame);
        }

        [HttpPost]
        public IActionResult JoinGame(int id)
        {
            List<GameDetails> gameList = new List<GameDetails>();
            gameList = gameMethods.GetGameDetails(out string errormsg);
            
            for(int i = 0; i < gameList.Count(); i++)
            {
                if ((gameList[i].playerRedId == id && gameList[i].playerYellowId == HttpContext.Session.GetInt32("UserId"))||(gameList[i].playerYellowId == id && gameList[i].playerRedId == HttpContext.Session.GetInt32("UserId")) && (gameList[i].Status == "Waiting" || gameList[i].Status == "Active"))
                { 
                    if(gameList[i].Status == "Waiting"&& gameList[i].playerYellowId == HttpContext.Session.GetInt32("UserId"))
                    {
                        gameList[i].Status = "Active";
                    }
                    gameMethods.UpdateGameStatus(gameList[i]);
                    
                    return RedirectToAction("ActiveGame", gameList[i]);
                }
            }

            ViewBag.JoinError = "Could not find game to join";
            return RedirectToAction("ShowUserList");
        }

        public IActionResult ActiveGame(GameDetails game)
        {
            List<MoveDetails> moveList = new List<MoveDetails>();
            moveList = moveMethods.GetMoveDetailsForGame((int)game.GameId, out string errormsg);
            ViewBag.MoveList = moveList;

            ViewBag.GameId = game.GameId;

            return View(game);
        }

        [HttpPost]
        public IActionResult CreateMove(int gameId, int column)
        {

            GameDetails currentGame = gameMethods.GetGameById(gameId, out string errormsg);

            char playerColor;

            // Check what color the player is
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
                return RedirectToAction("ActiveGame", currentGame);

            }
            // Check if it's the player's turn
            if (currentGame.CurrentTurn == playerColor && currentGame.Status == "Active")
            {

                MoveDetails newMove = new MoveDetails();
                newMove.GameId = gameId;
                newMove.ColumnIndex = column;

                List<MoveDetails> moveList = new List<MoveDetails>();
                moveList = moveMethods.GetMoveDetailsForGame(gameId, out string errorMsg);
                moveList = moveList.Where(m => m.ColumnIndex == column).ToList();

                int newRowIndex = 1;

                // Check if column is full
                //Check how many moves are in the column

                if (moveList.Count() >= 6)
                {
                    ViewBag.MoveError = "Column is full, choose another column";
                    return RedirectToAction("ActiveGame", currentGame);
                }
                else if (moveList.Count() >= 1)
                {
                    newRowIndex = 7-moveList.Count();
                }

                newMove.RowIndex = newRowIndex;


                // Assign player and color based on current turn

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
                return RedirectToAction("ActiveGame", currentGame);
            }
            else
            {
                ViewBag.MoveError = "It's not your turn";
                return RedirectToAction("ActiveGame", currentGame);
            }
        }
    }
}

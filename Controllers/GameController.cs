using FyraIRad.Models;
using Microsoft.AspNetCore.Mvc;

namespace FyraIRad.Controllers
{
    public class GameController : Controller
    {
        private readonly UserMethods userMethods;
        private readonly GameMethods gameMethods;
        public GameController(IConfiguration configuration)
        {
            userMethods = new UserMethods(configuration);
            gameMethods = new GameMethods(configuration);
        }

        [HttpPost]
        public IActionResult StartGame(int id)
        {
            GameDetails newGame = new GameDetails();

            newGame.playerRedId = (int)HttpContext.Session.GetInt32("UserId");
            newGame.playerYellowId = id;
            newGame.CurrentTurn = 'R';
            newGame.Status = "Waiting";
            gameMethods.CreateGame(newGame);


            return View("ActiveGame", newGame);
        }

        [HttpPost]
        public IActionResult JoinGame(int id)
        {
            List<GameDetails> gameList = new List<GameDetails>();
            gameList = gameMethods.GetGameDetails(out string errormsg);
            
            for(int i = 0; i < gameList.Count(); i++)
            {
                if (gameList[i].playerRedId == id && gameList[i].playerYellowId == HttpContext.Session.GetInt32("UserId") && gameList[i].Status == "Waiting")
                { 
                    gameList[i].Status = "Active";
                    gameMethods.UpdateGameStatus(gameList[i]);
                    return View("ActiveGame", gameList[i]);
                }
            }

            ViewBag.JoinError = "Could not find game to join";
            return RedirectToAction("Show UserList");
        }
    }
}

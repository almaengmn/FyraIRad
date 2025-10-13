using FyraIRad.Models;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;


namespace FyraIRad.Controllers
{
    public class LogInController : Controller
    {
        private readonly UserMethods userMethods;
        private readonly GameMethods gameMethods;
        public LogInController(IConfiguration configuration)
        {
            userMethods = new UserMethods(configuration);
            gameMethods = new GameMethods(configuration);
        }


        public IActionResult Index()
        {
            UserDetails user = new UserDetails();
            UserDetails newUser = new UserDetails();
            return View();
        }


        [HttpPost]
        public IActionResult LogIn(UserDetails tryLogin)
        {
            List<UserDetails> userList = new List<UserDetails>();
           

            userList = userMethods.GetUserDetails(out string errormsg);

            for (int i = 0; i < userList.Count; i++)
            {
                if (tryLogin.Username == userList[i].Username && tryLogin.Password == userList[i].Password)
                {
                    HttpContext.Session.SetInt32("UserId", (int)userList[i].UserId);
                    HttpContext.Session.SetString("Username", tryLogin.Username);
                    HttpContext.Session.SetString("Password", tryLogin.Password);
                    HttpContext.Session.SetString("IsLoggedIn", "true");

                    return RedirectToAction("ShowUserList");
                }
                
                
            }

                ViewBag.Error = "Username or password is incorrect";
                return View("Index");
           
        }

        [HttpPost]
        public IActionResult CreateUser(UserDetails newUser)
        {
            List<UserDetails> userList = new List<UserDetails>();


            userList = userMethods.GetUserDetails(out string errormsg);

            for (int i = 0; i < userList.Count; i++)
            {
                if (newUser.Username == userList[i].Username)
                {
                    ViewBag.Error = "Username already exists";
                    return View("Index");
                }
            }

                userMethods.CreateUser(newUser);

            return RedirectToAction("ShowUserList");
        }


        [HttpGet]
        public IActionResult ShowUserList()
        {
            if(HttpContext.Session.GetString("IsLoggedIn") == "true") {
                List<UserDetails> userList = new List<UserDetails>();

                userList = userMethods.GetUserDetails(out string errormsg);

                List<GameDetails> gameList = new List<GameDetails>();
                gameList = gameMethods.GetGameDetails(out string errorMsg);

                gameList = gameList.Where(g => g.Status == "Waiting" && (g.playerYellowId == HttpContext.Session.GetInt32("UserId") )).ToList();

                ViewBag.ActiveGames = gameList; 

                for (int i = 0; i < userList.Count; i++)
                {
                    if (userList[i].UserId == HttpContext.Session.GetInt32("UserId"))
                    {
                        userList.RemoveAt(i);
                    }
                    
                }

                return View(userList);
            }
            else
            {
                ViewBag.Error = "Not logged in";
                return RedirectToAction("Index");
            }
            
        }

        [HttpGet]
        public IActionResult EditUser(int id)
        {
            if (HttpContext.Session.GetString("IsLoggedIn") == "true")
            {
                UserDetails user = new UserDetails();

                user = userMethods.GetOneUserDetails(id, out string errormsg);
                return View(user);
            }
            {
                ViewBag.Error = "Not logged in";
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public IActionResult EditUser(UserDetails editUser)
        {
            
            userMethods.EditUser(editUser);
            HttpContext.Session.SetString("Username", editUser.Username);
            HttpContext.Session.SetString("Password", editUser.Password);
            return RedirectToAction("ShowUserList");
        }

        [HttpGet]
        public IActionResult DeleteUser(int id)
        {
            if (HttpContext.Session.GetString("IsLoggedIn") == "true")
            {
                UserDetails user = new UserDetails();
                user = userMethods.GetOneUserDetails(id, out string errormsg);
                return View(user);
            }
            {
                ViewBag.Error = "Not logged in";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult DeleteUser(UserDetails deleteUser)
        {
            userMethods.DeleteUser(deleteUser);
            HttpContext.Session.Clear();
            return RedirectToAction("ShowUserList");
        }
    }
}


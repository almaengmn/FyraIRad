using FyraIRad.Models;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;


namespace FyraIRad.Controllers
{
    public class LogInController : Controller
    {
        private readonly UserMethods userMethods;
        public LogInController(IConfiguration configuration)
        {
            userMethods = new UserMethods(configuration);
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
                    HttpContext.Session.SetInt32("UserId", (int)userList[i].Id);
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
            
            userMethods.CreateUser(newUser);

            return RedirectToAction("ShowUserList");
        }


        [HttpGet]
        public IActionResult ShowUserList()
        {
            if(HttpContext.Session.GetString("IsLoggedIn") == "true") {
                List<UserDetails> userList = new List<UserDetails>();

                userList = userMethods.GetUserDetails(out string errormsg);

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
            return RedirectToAction("Index");
        }
    }
}

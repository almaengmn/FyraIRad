using Microsoft.AspNetCore.Mvc;
using FyraIRad.Models;

namespace FyraIRad.Controllers
{
    public class LogInController : Controller
    {
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
            UserMethods userMethods = new UserMethods();

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
                else
                {
                    ViewBag.Error = "Username or password is incorrect";
                    return View("Index");
                }
            }
            return View("Index");
        }

        [HttpPost]
        public IActionResult CreateUser(UserDetails newUser)
        {
            UserMethods userMethods = new UserMethods();
            userMethods.CreateUser(newUser);

            return RedirectToAction("ShowUserList");
        }


        [HttpGet]
        public IActionResult ShowUserList()
        {
            List<UserDetails> userList = new List<UserDetails>();

            UserMethods userMethods = new UserMethods();

            userList = userMethods.GetUserDetails(out string errormsg);

            return View(userList);
        }

        [HttpGet]
        public IActionResult EditUser(int id)
        {
            UserDetails user = new UserDetails();
            UserMethods userMethods = new UserMethods();
            user = userMethods.GetOneUserDetails(id, out string errormsg);
            return View(user);

        }

        [HttpPost]
        public IActionResult EditUser(UserDetails editUser)
        {
            UserMethods userMethods = new UserMethods();
            userMethods.EditUser(editUser);
            HttpContext.Session.SetString("Username", editUser.Username);
            HttpContext.Session.SetString("Password", editUser.Password);
            return RedirectToAction("ShowUserList");
        }
    }
}

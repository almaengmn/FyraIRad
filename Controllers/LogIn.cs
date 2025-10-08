using Microsoft.AspNetCore.Mvc;
using FyraIRad.Models;

namespace FyraIRad.Controllers
{
    public class LogIn : Controller
    {
        public IActionResult Index()
        {
            UserDetails user = new UserDetails();
            UserDetails newUser = new UserDetails();
            return View();
        }

      .
        [HttpPost]
        public IActionResult LogInUser(UserDetails tryLogin)
        {
            List<UserDetails> userList = new List<UserDetails>();
            UserMethods userMethods = new UserMethods();

            userList = userMethods.GetUserDetails(out string errormsg);

            for (int i = 0; i < userList.Count; i++)
            {
                if (tryLogin.Username == userList[i].Username && tryLogin.Password == userList[i].Password)
                {
                    ViewBag.Id = userList[i].Id;
                    ViewBag.User = userList[i].Username;
                    ViewBag.Password = userList[i].Password;
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
    }
}

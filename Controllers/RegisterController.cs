using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using bank.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace bank.Controllers
{
    public class RegisterController : Controller
    {
        private BankContext _context;
        
            public RegisterController(BankContext context)
            {
                _context = context;
            }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("login")]
        public IActionResult LoginPage()
        {
            return View("Login");
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login(string LogEmail, string LogPassword)
        {
            var Hasher = new PasswordHasher<User>();
            User FoundUser = _context.Users.SingleOrDefault(user => user.Email == LogEmail);
            if (FoundUser == null || 0 == Hasher.VerifyHashedPassword(FoundUser, FoundUser.Password, LogPassword))
            {
                ViewBag.Message = "Login failed.";
                return View("Index");
            }
            else
            {
                HttpContext.Session.SetInt32("UserId", FoundUser.UserId);
                return RedirectToAction("Index", "Account", new { accountNum = HttpContext.Session.GetInt32("UserId")});
            }
        }

        [HttpPost]
        [Route("Register")]
        public IActionResult Register(RegisterViewModel NewUser)
        {
            if (ModelState.IsValid)
            {
                User ExistingUser = _context.Users.SingleOrDefault(user => user.Email == NewUser.Email);
                if (ExistingUser != null)
                {
                    ViewBag.Message = "User with this email already exists!";
                    return View("Index");
                }
                PasswordHasher<RegisterViewModel> Hasher = new PasswordHasher<RegisterViewModel>();
                NewUser.Password = Hasher.HashPassword(NewUser, NewUser.Password);
                User User = new User
                {
                    FirstName = NewUser.FirstName,
                    LastName = NewUser.LastName,
                    Email = NewUser.Email,
                    Password = NewUser.Password,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Add(User);
                _context.SaveChanges();
               User LoggedUser = _context.Users.SingleOrDefault(user => user.Email == NewUser.Email);
                HttpContext.Session.SetInt32("UserId", User.UserId);
                return RedirectToAction("Index", "Account", new { accountNum = HttpContext.Session.GetInt32("UserId")});
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet]
        [Route("Logoff")]
        public IActionResult Logoff()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }
    }
}
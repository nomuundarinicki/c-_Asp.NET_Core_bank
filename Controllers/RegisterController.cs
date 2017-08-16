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
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User ExistingUser = _context.Users.SingleOrDefault(user => user.Email == model.Email);
                if (ExistingUser != null)
                {
                    ViewBag.Message = "User with this email already exists!";
                    return View("Index", model);
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                User NewUser = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = model.Password,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                NewUser.Password = Hasher.HashPassword(NewUser, NewUser.Password);
                _context.Add(NewUser);
                _context.SaveChanges();
                NewUser = _context.Users.SingleOrDefault(user => user.Email == NewUser.Email);
                HttpContext.Session.SetInt32("UserId", NewUser.UserId);
                return RedirectToAction("Index", "Account", new { accountNum = HttpContext.Session.GetInt32("UserId")});
            }
            else
            {
                return View("Index", model);
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
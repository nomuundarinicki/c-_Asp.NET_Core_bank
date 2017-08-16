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
    public class AccountController : Controller
    {
        private BankContext _context;
        
            public AccountController(BankContext context)
            {
                _context = context;
            }

            [HttpGet]   
            [Route("account/{accountNum}")]
            public IActionResult Index(int accountNum)
            {
              if (accountNum != (int)HttpContext.Session.GetInt32("UserId"))
              {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Register");
              }
              List<Transaction> MyTransactions = _context.Transactions.Where(transaction => transaction.UserId == accountNum).OrderByDescending(transaction => transaction.CreatedAt).ToList();
              ViewBag.MyTransactions = MyTransactions;
              Console.WriteLine(ViewBag.MyTransactions.Count);
              ViewBag.Balance = 0;
              foreach (Transaction trans in MyTransactions)
              {
                ViewBag.Balance += trans.Amount;
              }
              if (ViewBag.Balance <= 0)
              {
                ViewBag.Minimum = 0;
              }
              else
              {
                ViewBag.Minimum = 0 - ViewBag.Balance;
              }
              ViewBag.Accountholder = _context.Users.SingleOrDefault(user => user.UserId == accountNum).FirstName;
              return View();
            }

            [HttpPost]
            [Route("Transact")]
            public IActionResult Transact(double Amount)
            {
              Console.WriteLine($"The amount received from the transaction form is {Amount}.");
              User Transactor = _context.Users.SingleOrDefault(user => user.UserId == HttpContext.Session.GetInt32("UserId"));
              double balance = 0;
              List<Transaction> MyTransactions = _context.Transactions.Where(transaction => transaction.UserId == Transactor.UserId).ToList();
              foreach (Transaction trans in MyTransactions)
              {
                balance += (double)trans.Amount;
              }
              if (balance + Amount >= 0 && Transactor != null)
              {
                // new transaction
                Transaction NewTransaction = new Transaction {
                  Amount = (decimal)Amount,
                  CreatedAt = DateTime.UtcNow,
                  UpdatedAt = DateTime.UtcNow,
                  UserId = Transactor.UserId
                };
                _context.Transactions.Add(NewTransaction);
                _context.SaveChanges();
                return RedirectToAction("Index", new { accountNum = HttpContext.Session.GetInt32("UserId")});
              }
              else
              {
                // return error message
                return View("Index");
              }
              
            }
    }
}
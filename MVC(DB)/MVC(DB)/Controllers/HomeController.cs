using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC_DB_.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_DB_.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            DBmanager dbmanager = new DBmanager();
            List<account> accounts = dbmanager.getAccounts();
            ViewBag.accounts = accounts;
            return View();
        }

        public IActionResult addAccount()
        {
            return View();
        }

        [HttpPost]
        public IActionResult addAccount(account user)
        {

            DBmanager dbmanager = new DBmanager();
            try
            {
                dbmanager.newAccount(user);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return RedirectToAction("Index");
        }        


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

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
        private readonly DBmanager _dbManager;

        public HomeController(ILogger<HomeController> logger, DBmanager dbManager)
        {
            _logger = logger;
            _dbManager = dbManager;
        }

        public IActionResult Index()
        {
            try
            {
                _logger.LogInformation("Fetching accounts for Index page");
                List<account> accounts = _dbManager.getAccounts();
                ViewBag.accounts = accounts;
                _logger.LogInformation($"Successfully retrieved {accounts.Count} accounts");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching accounts");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public IActionResult addAccount()
        {
            return View();
        }

        [HttpPost]
        public IActionResult addAccount(account user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state when adding account");
                    return View(user);
                }

                _logger.LogInformation($"Attempting to add new account for user: {user.userName}");
                _dbManager.newAccount(user);
                _logger.LogInformation($"Successfully added new account for user: {user.userName}");
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error adding new account for user: {user.userName}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
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

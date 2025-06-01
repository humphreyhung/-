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
        //private readonly DBmanager _dbManager;

        public HomeController(ILogger<HomeController> logger/*, DBmanager dbManager*/)
        {
            _logger = logger;
            //_dbManager = dbManager;
        }

        public IActionResult Index()
        {
            try
            {
                _logger.LogInformation("Fetching accounts for Index page");
                //List<account> accounts = _dbManager.getAccounts();
                //ViewBag.accounts = accounts;
                //_logger.LogInformation($"Successfully retrieved {accounts.Count} accounts");
                ViewBag.accounts = new List<account>(); // 初始化一個空列表
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching accounts");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public IActionResult index_R()
        {
            return View();
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

                if (user == null)
                {
                    _logger.LogWarning("User object is null");
                    return View();
                }

                _logger.LogInformation($"Attempting to add new account for user: {user.userName}");
                //_dbManager.newAccount(user);
                _logger.LogInformation($"Successfully added new account for user: {user.userName}");
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error adding new account for user: {user?.userName ?? "unknown"}");
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

        public IActionResult Project()
        {
            var viewModel = new ProjectViewModel
            {
                Projects = new List<Project>
                {
                    new Project
                    {
                        Id = 1,
                        Title = "地方的孩子需要吃飯",
                        Description = "賞口飯吃好嗎(๑•́ ₃ •̀๑)",
                        ImageUrl = "fa-utensils",
                        CurrentAmount = 150000,
                        TargetAmount = 500000,
                        RemainingDays = 15
                    },
                    new Project
                    {
                        Id = 2,
                        Title = "地方的孩子需要教材",
                        Description = "知識就是力量(╬☉д⊙)",
                        ImageUrl = "fa-tablet-screen-button",
                        CurrentAmount = 200000,
                        TargetAmount = 800000,
                        RemainingDays = 20
                    },
                    new Project
                    {
                        Id = 3,
                        Title = "地方的孩子需要家",
                        Description = "(:3[___]=",
                        ImageUrl = "fa-school",
                        CurrentAmount = 300000,
                        TargetAmount = 1000000,
                        RemainingDays = 25
                    }
                }
            };
            return View(viewModel);
        }

        public IActionResult ProjectDetails(int id)
        {
            // 這裡可以根據 id 獲取專案詳情
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // TODO: 實作登入邏輯
            return RedirectToAction("Index");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // TODO: 實作註冊邏輯
            return RedirectToAction("Login");
        }

        public IActionResult Sponsor(int id)
        {
            // 這裡可以根據 id 獲取專案資訊
            var viewModel = new SponsorViewModel
            {
                ProjectId = id,
                ProjectTitle = "地方的孩子需要吃飯",
                ProjectTargetAmount = 500000,
                ProjectCurrentAmount = 150000
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Sponsor(SponsorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 重定向到確認頁面
            return RedirectToAction("SponsorConfirm", new
            {
                projectId = model.ProjectId,
                amount = model.Amount,
                name = model.Name,
                email = model.Email,
                phone = model.Phone,
                city = model.City,
                district = model.District,
                address = model.Address,
                message = model.Message,
                paymentMethod = model.PaymentMethod
            });
        }

        public IActionResult SponsorConfirm(int projectId, decimal amount, string name, string email, string phone, 
            string city, string district, string address, string message, string paymentMethod)
        {
            var viewModel = new SponsorConfirmViewModel
            {
                ProjectId = projectId,
                ProjectTitle = "地方的孩子需要吃飯", // 這裡應該從資料庫獲取
                Amount = amount,
                Name = name,
                Email = email,
                Phone = phone,
                City = city,
                District = district,
                Address = address,
                Message = message,
                PaymentMethod = paymentMethod,
                TransactionId = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(),
                TransactionTime = DateTime.Now
            };
            return View(viewModel);
        }

        public IActionResult SponsorPlan(int id)
        {
            var viewModel = new SponsorPlanViewModel
            {
                ProjectId = id,
                ProjectTitle = "地方的孩子需要吃飯",
                ProjectTargetAmount = 500000,
                ProjectCurrentAmount = 150000,
                Plans = new List<SponsorPlan>
                {
                    new SponsorPlan
                    {
                        Title = "基本贊助",
                        Amount = 1000,
                        Benefits = new List<string>
                        {
                            "感謝卡一張",
                            "電子感謝函",
                            "專案進度更新",
                            "蛋捲一盒"
                        },
                        IsFeatured = false,
                        EggRollFlavors = new List<string>
                        {
                            "原味",
                            "巧克力",
                            "抹茶"
                        }
                    },
                    new SponsorPlan
                    {
                        Title = "進階贊助",
                        Amount = 3000,
                        Benefits = new List<string>
                        {
                            "感謝卡一張",
                            "電子感謝函",
                            "專案進度更新",
                            "限量紀念品",
                            "贊助者名單刊登",
                            "蛋捲一盒",
                            "餅乾一盒"
                        },
                        IsFeatured = true,
                        EggRollFlavors = new List<string>
                        {
                            "原味",
                            "巧克力",
                            "抹茶",
                            "咖啡",
                            "芝麻",
                            "紅茶",
                            "草莓"
                        },
                        CookieFlavors = new List<string>
                        {
                            "原味",
                            "巧克力",
                            "抹茶",
                            "蔓越莓",
                            "杏仁"
                        }
                    },
                    new SponsorPlan
                    {
                        Title = "高級贊助",
                        Amount = 5000,
                        Benefits = new List<string>
                        {
                            "感謝卡一張",
                            "電子感謝函",
                            "專案進度更新",
                            "限量紀念品",
                            "贊助者名單刊登",
                            "專屬活動邀請",
                            "蛋捲一盒",
                            "餅乾一盒",
                            "蛋糕一盒"
                        },
                        IsFeatured = false,
                        EggRollFlavors = new List<string>
                        {
                            "原味",
                            "巧克力",
                            "抹茶",
                            "咖啡",
                            "芝麻",
                            "紅茶",
                            "草莓"
                        },
                        CookieFlavors = new List<string>
                        {
                            "原味",
                            "巧克力",
                            "抹茶",
                            "蔓越莓",
                            "杏仁",
                            "咖啡",
                            "檸檬",
                            "椰子"
                        },
                        CakeFlavors = new List<string>
                        {
                            "原味",
                            "巧克力",
                            "抹茶",
                            "草莓",
                            "芒果",
                            "藍莓",
                            "提拉米蘇",
                            "起司"
                        }
                    }
                }
            };
            return View(viewModel);
        }

        public IActionResult RewardCart(int id, decimal amount)
        {
            // 根據 id 和 amount 找到對應的贊助方案
            var sponsorPlan = new SponsorPlanViewModel
            {
                ProjectId = id,
                ProjectTitle = "地方的孩子需要吃飯",
                ProjectTargetAmount = 500000,
                ProjectCurrentAmount = 150000,
                Plans = new List<SponsorPlan>
                {
                    new SponsorPlan
                    {
                        Title = "基本贊助",
                        Amount = 1000,
                        Benefits = new List<string>
                        {
                            "感謝卡一張",
                            "電子感謝函",
                            "專案進度更新",
                            "蛋捲一盒"
                        },
                        IsFeatured = false,
                        EggRollFlavors = new List<string>
                        {
                            "原味",
                            "巧克力",
                            "抹茶",
                            "咖啡",
                            "芝麻",
                            "紅茶",
                            "草莓"
                        },
                        CookieFlavors = new List<string>
                        {
                            "原味",
                            "巧克力",
                            "抹茶",
                            "蔓越莓",
                            "杏仁",
                            "咖啡",
                            "檸檬",
                            "椰子"
                        }
                    },
                    new SponsorPlan
                    {
                        Title = "進階贊助",
                        Amount = 3000,
                        Benefits = new List<string>
                        {
                            "感謝卡一張",
                            "電子感謝函",
                            "專案進度更新",
                            "限量紀念品",
                            "贊助者名單刊登",
                            "蛋捲一盒",
                            "餅乾一盒"
                        },
                        IsFeatured = true,
                        EggRollFlavors = new List<string>
                        {
                            "原味",
                            "巧克力",
                            "抹茶",
                            "咖啡",
                            "芝麻",
                            "紅茶",
                            "草莓"
                        },
                        CookieFlavors = new List<string>
                        {
                            "原味",
                            "巧克力",
                            "抹茶",
                            "蔓越莓",
                            "杏仁",
                            "咖啡",
                            "檸檬",
                            "椰子"
                        }
                    },
                    new SponsorPlan
                    {
                        Title = "高級贊助",
                        Amount = 5000,
                        Benefits = new List<string>
                        {
                            "感謝卡一張",
                            "電子感謝函",
                            "專案進度更新",
                            "限量紀念品",
                            "贊助者名單刊登",
                            "專屬活動邀請",
                            "蛋捲一盒",
                            "餅乾一盒",
                            "蛋糕一盒"
                        },
                        IsFeatured = false,
                        EggRollFlavors = new List<string>
                        {
                            "原味",
                            "巧克力",
                            "抹茶",
                            "咖啡",
                            "芝麻",
                            "紅茶",
                            "草莓"
                        },
                        CookieFlavors = new List<string>
                        {
                            "原味",
                            "巧克力",
                            "抹茶",
                            "蔓越莓",
                            "杏仁",
                            "咖啡",
                            "檸檬",
                            "椰子"
                        }
                    }
                }
            }.Plans.FirstOrDefault(p => p.Amount == amount);

            if (sponsorPlan == null)
            {
                return RedirectToAction("SponsorPlan", new { id = id });
            }

            var viewModel = new RewardCartViewModel
            {
                ProjectId = id,
                ProjectTitle = "地方的孩子需要吃飯",
                ProjectImage = "~/images/pic1.png",
                Rewards = new List<RewardItem>
                {
                    new RewardItem
                    {
                        RewardId = 1,
                        Title = sponsorPlan.Title,
                        Description = "感謝您的贊助",
                        Price = sponsorPlan.Amount,
                        Quantity = 1,
                        EstimatedDelivery = "贊助後立即發送",
                        Benefits = sponsorPlan.Benefits,
                        EggRollFlavors = sponsorPlan.EggRollFlavors,
                        CookieFlavors = sponsorPlan.CookieFlavors
                    }
                },
                TotalAmount = sponsorPlan.Amount
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult RewardCart(RewardCartViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // TODO: 處理贊助訂單
            return RedirectToAction("SponsorConfirm", new { projectId = model.ProjectId });
        }

        public IActionResult ProductDetail()
        {
            return View();
        }
    }
}


using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC_DB_.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using MVC_DB_.Data;
using System.Text.Json;

namespace MVC_DB_.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DBmanager _dbManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger, DBmanager dbManager, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _dbManager = dbManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            try
            {
                _logger.LogInformation("Fetching accounts for Index page");
                var accounts = _dbManager.getAccounts();
                _logger.LogInformation($"Successfully retrieved {accounts.Count} accounts");
                return View(accounts);
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
            return View(new account());
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
                _logger.LogInformation($"Successfully added new account for user: {user.userName}");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding new account for user: {user.userName}");
                ModelState.AddModelError("", "An error occurred while adding the account. Please try again.");
                return View(user);
            }
        }        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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

        public IActionResult Projectpage()
        {
            var viewModel = new ProjectpageViewModel
            {
                Projects = new List<Projectpage>
                {
                    new Projectpage
                    {
                        Id = 1,
                        Title = "地方的孩子需要吃飯",
                        Description = "賞口飯吃好嗎(๑•́ ₃ •̀๑)",
                        ImageUrl = "fa-utensils",
                        CurrentAmount = 150000,
                        TargetAmount = 500000,
                        RemainingDays = 15
                    },
                    new Projectpage
                    {
                        Id = 2,
                        Title = "地方的孩子需要教材",
                        Description = "知識就是力量(╬☉д⊙)",
                        ImageUrl = "fa-tablet-screen-button",
                        CurrentAmount = 200000,
                        TargetAmount = 800000,
                        RemainingDays = 20
                    },
                    new Projectpage
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

        public IActionResult Project2()
        {
            var viewModel = new ProjectpageViewModel
            {
                Projects = new List<Projectpage>
                {
                    new Projectpage
                    {
                        Id = 2,
                        Title = "讓愛酥進心裡，用餅乾陪他們走過抗癌旅程",
                        Description = "每一盒餅乾都承載著我們的關懷與祝福",
                        ImageUrl = "fa-cookie",
                        CurrentAmount = 230000,
                        TargetAmount = 1000000,
                        RemainingDays = 180
                    }
                }
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject(IFormCollection form, List<decimal> rewardAmounts, List<string> rewardContents, IFormFile projectImage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string imageUrl = "";
            if (projectImage != null && projectImage.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + projectImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await projectImage.CopyToAsync(fileStream);
                }
                imageUrl = "/images/" + uniqueFileName;
            }

            var project = new Project
            {
                Title = form["projectName"],
                Description = form["projectDescription"],
                Category = form["projectCategory"],
                TargetAmount = decimal.Parse(form["targetAmount"]), 
                CurrentAmount = 0,
                StartDate = DateTime.Parse(form["startDate"]), 
                EndDate = DateTime.Parse(form["endDate"]), 
                ProposerName = form["proposerName"],
                ProposerEmail = form["proposerEmail"],
                ProposerPhone = form["proposerPhone"],
                ProjectContent = form["projectContent"],
                RelatedWebsite = form["relatedWebsite"],
                VideoUrl = form["videoUrl"],
                ImageUrl = imageUrl,
                Rewards = new List<Reward>()
            };

            for (int i = 0; i < rewardAmounts.Count; i++)
            {
                project.Rewards.Add(new Reward
                {
                    Amount = rewardAmounts[i],
                    Content = rewardContents[i]
                });
            }

            // 將 Project 物件暫存到 TempData
            // TempData["NewProjectData"] = JsonSerializer.Serialize(project);

            // 重定向到新建立的提案詳情頁面
            // return RedirectToAction("ProjectDetail", new { id = project.Id });

            // 直接返回 Project 物件的 JSON
            return Json(project);
        }

        public IActionResult ProjectDetail()
        {
            // ProjectDetail 頁面現在會從 sessionStorage 讀取資料
            // 我們只返回一個空的視圖，內容由客戶端填充
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

            return RedirectToAction("Login");
        }

        public IActionResult Sponsor(int id)
        {
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
                ProjectTitle = "地方的孩子需要吃飯",
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

            return RedirectToAction("SponsorConfirm", new { projectId = model.ProjectId });
        }

        public IActionResult ProductDetail()
        {
            return View();
        }
        public IActionResult Proposal()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }

        public IActionResult FAQ()
        {
            return View();
        }

        public IActionResult StartProject()
        {
            return View();
        }

        public IActionResult CreateProject()
        {
            return View();
        }

        public IActionResult project2page()
        {
            return View();
        }

        public IActionResult ProposalService()
        {
            return View();
        }

        public IActionResult ProposalService2()
        {
            return View();
        }

        public IActionResult SponsorGuarantee()
        {
            return View();
        }
    }
}


using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MVC_DB_.Services; // Assuming you have a CampaignService in your Services folder
using MVC_DB_.Models; // Assuming you have a Campaign model in your Models folder

namespace MVC_DB_.Controllers
//namespace Campaign.Controllers
{
    //[Authorize] // Require authentication for all actions
    public class CampaignController : Controller
    {
        private readonly ICampaignService _service;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<CampaignController> _logger;

        public CampaignController(
            ICampaignService service,
            UserManager<IdentityUser> userManager,
            ILogger<CampaignController> logger)
        {
            _service = service;
            _userManager = userManager;
            _logger = logger;
        }

        [AllowAnonymous] // Allow anyone to view the list
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("User accessing campaign list");
                var campaigns = await _service.GetAllCampaignsAsync();
                _logger.LogInformation($"Retrieved {campaigns.Count} campaigns");
                return View(campaigns);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching campaigns");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("role") != "Admin")
            {
                _logger.LogWarning("Unauthorized user attempting to access campaign creation form.");
                return Forbid();
            }

            _logger.LogInformation($"User {User.Identity?.Name} accessing campaign creation form");
            return View(new Campaign { Status = "Draft" });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Campaign model)
        {
            try
            {
                if (HttpContext.Session.GetString("role") != "Admin")
                {
                    _logger.LogWarning("Unauthorized user attempting to access campaign creation form.");
                    return Forbid();
                }

                _logger.LogInformation("Starting campaign creation process");

                // 若你不再用 _userManager，可使用 claim 或直接設定 dummy OwnerId
                var userId = User.FindFirst("sub")?.Value ?? "Unknown"; // 或用 "nameidentifier"
                var userName = User.Identity?.Name ?? "Unknown";

                model.OwnerId = userId;
                ModelState.Remove("OwnerId");
                ModelState.Remove("Owner");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state when creating campaign");
                    foreach (var modelStateEntry in ModelState.Values)
                    {
                        foreach (var error in modelStateEntry.Errors)
                        {
                            _logger.LogWarning($"Validation error: {error.ErrorMessage}");
                        }
                    }
                    return View(model);
                }

                model.CreatedAt = DateTime.UtcNow;
                model.CollectedAmount = 0;
                model.Owner = null; // 或保留 null，看你 DB 設計是否允許

                var createdCampaign = await _service.CreateCampaignAsync(model);
                _logger.LogInformation($"Campaign created successfully. ID: {createdCampaign.Id}, Title: {createdCampaign.Title}");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating campaign: {model.Title}. Exception: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner exception: {ex.InnerException.Message}");
                }
                ModelState.AddModelError("", "建立專案時發生錯誤，請稍後再試。");
                return View(model);
            }
        }



        public async Task<IActionResult> Details(int id)
        {
            try
            {
                _logger.LogInformation($"User {User.Identity?.Name} accessing campaign details for ID: {id}");
                var campaign = await _service.GetCampaignByIdAsync(id);
                if (campaign == null)
                {
                    _logger.LogWarning($"Campaign with ID {id} not found");
                    return NotFound();
                }
                return View(campaign);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching campaign details for ID: {id}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                _logger.LogInformation($"User {User.Identity?.Name} accessing edit form for campaign ID: {id}");
                var campaign = await _service.GetCampaignByIdAsync(id);
                if (campaign == null)
                {
                    _logger.LogWarning($"Campaign with ID {id} not found");
                    return NotFound();
                }

                var user = await _userManager.GetUserAsync(User);
                if (user?.Id != campaign.OwnerId)
                {
                    _logger.LogWarning($"User {user?.UserName} attempted to edit campaign {id} owned by {campaign.Owner.UserName}");
                    return Forbid();
                }

                return View(campaign);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching campaign for edit, ID: {id}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Campaign model)
        {
            if (id != model.Id)
            {
                _logger.LogWarning($"ID mismatch in Edit action. URL ID: {id}, Model ID: {model.Id}");
                return NotFound();
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state when editing campaign");
                    return View(model);
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("User not found when editing campaign");
                    return Forbid();
                }

                var existingCampaign = await _service.GetCampaignByIdAsync(id);
                if (existingCampaign == null || existingCampaign.OwnerId != user.Id)
                {
                    _logger.LogWarning($"Campaign not found or user {user.UserName} is not the owner");
                    return NotFound();
                }

                // Preserve the original values that shouldn't be modified
                model.OwnerId = existingCampaign.OwnerId;
                model.Owner = existingCampaign.Owner;
                model.CreatedAt = existingCampaign.CreatedAt;

                await _service.UpdateCampaignAsync(model);
                _logger.LogInformation($"Campaign '{model.Title}' updated successfully by {user.UserName}");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating campaign ID: {id}");
                ModelState.AddModelError("", "An error occurred while updating the campaign. Please try again.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation($"User {User.Identity?.Name} attempting to delete campaign ID: {id}");
                var campaign = await _service.GetCampaignByIdAsync(id);
                if (campaign == null)
                {
                    _logger.LogWarning($"Campaign with ID {id} not found for deletion");
                    return NotFound();
                }

                var user = await _userManager.GetUserAsync(User);
                if (user?.Id != campaign.OwnerId)
                {
                    _logger.LogWarning($"User {user?.UserName} attempted to delete campaign {id} owned by {campaign.Owner.UserName}");
                    return Forbid();
                }

                if (campaign.Status != "Draft")
                {
                    _logger.LogWarning($"Attempt to delete non-draft campaign. Status: {campaign.Status}");
                    ModelState.AddModelError("", "Only draft campaigns can be deleted.");
                    return RedirectToAction("Index");
                }

                await _service.DeleteCampaignAsync(id);
                _logger.LogInformation($"Campaign ID {id} deleted successfully by {user.UserName}");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting campaign ID: {id}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

     
    }
}

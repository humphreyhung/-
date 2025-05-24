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
    public class CampaignController : Controller
    {
        private readonly ICampaignService _service; // Add this field
        public CampaignController(ICampaignService service) // Update constructor to inject the service
        {
            _service = service;
        }
        public async Task<IActionResult> Index()
        {
            var campaigns = await _service.GetAllCampaignsAsync();
            return View(campaigns);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Campaign model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _service.CreateCampaignAsync(model);
            return RedirectToAction("Index");
        }
        //public IActionResult Index()
        //{
        //    var campaigns = new Campaign;
        //    return View(model:campaigns);
        //}
    }
}

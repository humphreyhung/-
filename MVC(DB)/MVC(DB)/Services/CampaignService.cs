//using MVC_DB_.Dates;
using MVC_DB_.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using MVC_DB_.Data;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace MVC_DB_.Services
{
    public interface ICampaignService
    {
        Task<List<Campaign>> GetAllCampaignsAsync();
        Task<Campaign?> GetCampaignByIdAsync(int id);
        Task<Campaign> CreateCampaignAsync(Campaign campaign);
        Task<Campaign> UpdateCampaignAsync(Campaign campaign);
        Task DeleteCampaignAsync(int id);
    }

    public class CampaignService : ICampaignService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CampaignService> _logger;

        public CampaignService(ApplicationDbContext context, ILogger<CampaignService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<Campaign>> GetAllCampaignsAsync()
        {
            try
            {
                _logger.LogInformation("Starting GetAllCampaignsAsync");
                var campaigns = await _context.Campaigns
                    .Include(c => c.Owner)
                    .ToListAsync();
                _logger.LogInformation($"Retrieved {campaigns.Count} campaigns from database");
                foreach (var campaign in campaigns)
                {
                    _logger.LogDebug($"Campaign: Id={campaign.Id}, Title={campaign.Title}, OwnerId={campaign.OwnerId}");
                }
                return campaigns;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all campaigns");
                throw;
            }
        }

        public async Task<Campaign?> GetCampaignByIdAsync(int id)
        {
            try
            {
                var campaign = await _context.Campaigns
                    .Include(c => c.Owner)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (campaign == null)
                {
                    _logger.LogWarning($"Campaign with ID {id} was not found");
                }

                return campaign;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting campaign with ID {id}");
                throw;
            }
        }

        public async Task<Campaign> CreateCampaignAsync(Campaign campaign)
        {
            if (campaign == null)
            {
                throw new ArgumentNullException(nameof(campaign));
            }

            try
            {
                _logger.LogInformation($"Attempting to create campaign: Title={campaign.Title}, OwnerId={campaign.OwnerId}, Status={campaign.Status}");
                _logger.LogDebug($"Campaign details: Description={campaign.Description}, TargetAmount={campaign.TargetAmount}, CreatedAt={campaign.CreatedAt}");
                
                await _context.Campaigns.AddAsync(campaign);
                _logger.LogInformation("Campaign added to context, attempting to save changes");
                
                var result = await _context.SaveChangesAsync();
                _logger.LogInformation($"SaveChanges completed. {result} records affected. New Campaign ID: {campaign.Id}");
                
                // Verify the campaign was saved
                var savedCampaign = await _context.Campaigns
                    .Include(c => c.Owner)
                    .FirstOrDefaultAsync(c => c.Id == campaign.Id);
                
                if (savedCampaign != null)
                {
                    _logger.LogInformation($"Successfully verified campaign creation. ID: {savedCampaign.Id}, Title: {savedCampaign.Title}");
                }
                else
                {
                    _logger.LogWarning($"Campaign was saved but could not be retrieved with ID: {campaign.Id}");
                }
                
                return campaign;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, $"Database error occurred while creating campaign. Message: {dbEx.Message}");
                if (dbEx.InnerException != null)
                {
                    _logger.LogError($"Inner exception: {dbEx.InnerException.Message}");
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error occurred while creating campaign. Message: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public async Task<Campaign> UpdateCampaignAsync(Campaign campaign)
        {
            if (campaign == null)
            {
                throw new ArgumentNullException(nameof(campaign));
            }

            try
            {
                _context.Entry(campaign).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Campaign updated successfully. ID: {campaign.Id}");
                return campaign;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating campaign with ID {campaign.Id}");
                throw;
            }
        }

        public async Task DeleteCampaignAsync(int id)
        {
            try
            {
                var campaign = await _context.Campaigns.FindAsync(id);
                if (campaign != null)
                {
                    _context.Campaigns.Remove(campaign);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Campaign deleted successfully. ID: {id}");
                }
                else
                {
                    _logger.LogWarning($"Attempted to delete non-existent campaign with ID {id}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting campaign with ID {id}");
                throw;
            }
        }
    }
}

using MVC_DB_.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVC_DB_.Services
{
    public interface ICampaignService
    {
        Task<List<Campaign>> GetAllCampaignsAsync();
        Task<Campaign> GetCampaignByIdAsync(int id);
        Task CreateCampaignAsync(Campaign campaign);
        Task UpdateCampaignAsync(Campaign campaign);
        Task DeleteCampaignAsync(int id);
    }
}

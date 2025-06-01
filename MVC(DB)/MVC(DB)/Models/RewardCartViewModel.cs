using System.ComponentModel.DataAnnotations;

namespace MVC_DB_.Models
{
    public class RewardCartViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public string ProjectImage { get; set; }
        public List<RewardItem> Rewards { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
    }

    public class RewardItem
    {
        public int RewardId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string EstimatedDelivery { get; set; }
        public List<string> Benefits { get; set; }
        public List<string> EggRollFlavors { get; set; }
        public List<string> CookieFlavors { get; set; }
        public List<string> CakeFlavors { get; set; }
    }
} 
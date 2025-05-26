using System.ComponentModel.DataAnnotations;

namespace MVC_DB_.Models
{
    public class SponsorPlanViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public decimal ProjectTargetAmount { get; set; }
        public decimal ProjectCurrentAmount { get; set; }
        public List<SponsorPlan> Plans { get; set; }
    }

    public class SponsorPlan
    {
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public List<string> Benefits { get; set; }
        public bool IsFeatured { get; set; }
    }
} 
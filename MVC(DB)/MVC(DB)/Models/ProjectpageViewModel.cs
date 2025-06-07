using System.Collections.Generic;

namespace MVC_DB_.Models
{
    public class ProjectpageViewModel
    {
        public List<Projectpage> Projects { get; set; } = new List<Projectpage>();
    }

    public class Projectpage
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal CurrentAmount { get; set; }
        public decimal TargetAmount { get; set; }
        public int RemainingDays { get; set; }
        public int ProgressPercentage => (int)((CurrentAmount / TargetAmount) * 100);
    }
} 
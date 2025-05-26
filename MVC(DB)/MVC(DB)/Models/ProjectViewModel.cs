using System.Collections.Generic;

namespace MVC_DB_.Models
{
    public class ProjectViewModel
    {
        public List<Project> Projects { get; set; } = new List<Project>();
    }

    public class Project
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
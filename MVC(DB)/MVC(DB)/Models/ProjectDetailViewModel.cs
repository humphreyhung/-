using System;
using System.ComponentModel.DataAnnotations;

namespace MVC_DB_.Models
{
    public class ProjectDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ProposerName { get; set; }
        public string ProposerEmail { get; set; }
        public string ProposerPhone { get; set; }
        public string ProjectContent { get; set; }
        public string RelatedWebsite { get; set; }
        public string VideoUrl { get; set; }
        public string ImageUrl { get; set; }
    }
} 
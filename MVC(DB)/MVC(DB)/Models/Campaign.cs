using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_DB_.Models
{
    public class Campaign
    {
        [Required]
        public string Name { get; set; } = "Default Campaign String, Hum test";
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Range(0, 10000000)]
        public decimal TargetAmount { get; set; }

        public decimal CollectedAmount { get; set; } = 0;

        [Required]
        public string Status { get; set; } = "Draft";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string OwnerId { get; set; } = string.Empty;

        [Required]
        public IdentityUser Owner { get; set; } = null!;
    }
}

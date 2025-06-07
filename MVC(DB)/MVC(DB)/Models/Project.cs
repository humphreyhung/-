using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_DB_.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TargetAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal CurrentAmount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [StringLength(100)]
        public string ProposerName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string ProposerEmail { get; set; }

        [Phone]
        [StringLength(20)]
        public string ProposerPhone { get; set; }

        [Required]
        public string ProjectContent { get; set; }

        [Url]
        [StringLength(500)]
        public string RelatedWebsite { get; set; }

        [Url]
        [StringLength(500)]
        public string VideoUrl { get; set; }

        [StringLength(500)]
        public string ImageUrl { get; set; }

        public ICollection<Reward> Rewards { get; set; }
    }

    public class Reward
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [Required]
        public string Content { get; set; }

        public int ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
    }
} 
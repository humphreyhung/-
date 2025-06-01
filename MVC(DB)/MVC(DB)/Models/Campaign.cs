using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_DB_.Models
{
    public class Campaign
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "請輸入標題")]
        [StringLength(100, ErrorMessage = "標題不能超過{1}個字元")]
        [Display(Name = "標題")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入描述")]
        [MinLength(10, ErrorMessage = "描述至少需要{1}個字元")]
        [MaxLength(2000, ErrorMessage = "描述不能超過{1}個字元")]
        [Display(Name = "描述")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入目標金額")]
        [Range(1000, 10000000, ErrorMessage = "目標金額必須在{1:C0}到{2:C0}之間")]
        [DataType(DataType.Currency)]
        [Display(Name = "目標金額")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TargetAmount { get; set; }

        [Display(Name = "已募金額")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CollectedAmount { get; set; }

        [Required(ErrorMessage = "請選擇狀態")]
        [StringLength(50, ErrorMessage = "狀態不能超過{1}個字元")]
        [Display(Name = "狀態")]
        public string Status { get; set; } = "Draft";

        [Display(Name = "建立時間")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("Owner")]
        public string OwnerId { get; set; } = string.Empty;

        public virtual IdentityUser Owner { get; set; } = null!;
    }
}

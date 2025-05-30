using System.ComponentModel.DataAnnotations;

namespace MVC_DB_.Models
{
    public class SponsorViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public decimal ProjectTargetAmount { get; set; }
        public decimal ProjectCurrentAmount { get; set; }

        [Required(ErrorMessage = "請輸入贊助金額")]
        [Range(100, 1000000, ErrorMessage = "贊助金額必須在 100 到 1,000,000 之間")]
        [Display(Name = "贊助金額")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "請輸入姓名")]
        [Display(Name = "姓名")]
        public string Name { get; set; }

        [Required(ErrorMessage = "請輸入電子郵件")]
        [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址")]
        [Display(Name = "電子郵件")]
        public string Email { get; set; }

        [Required(ErrorMessage = "請輸入電話")]
        [RegularExpression(@"^09\d{8}$", ErrorMessage = "請輸入有效的手機號碼")]
        [Display(Name = "電話號碼")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "請選擇縣市")]
        public string City { get; set; }

        [Required(ErrorMessage = "請選擇鄉鎮市區")]
        public string District { get; set; }

        [Required(ErrorMessage = "請輸入詳細地址")]
        public string Address { get; set; }

        [Display(Name = "留言")]
        public string Message { get; set; }

        [Required(ErrorMessage = "請選擇付款方式")]
        [Display(Name = "付款方式")]
        public string PaymentMethod { get; set; }
    }
} 
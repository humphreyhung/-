using System.ComponentModel.DataAnnotations;

namespace MVC_DB_.Models
{
    public class SponsorViewModel
    {
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

        [Required(ErrorMessage = "請輸入電話號碼")]
        [Phone(ErrorMessage = "請輸入有效的電話號碼")]
        [Display(Name = "電話號碼")]
        public string Phone { get; set; }

        [Display(Name = "留言")]
        public string Message { get; set; }

        [Required(ErrorMessage = "請選擇付款方式")]
        [Display(Name = "付款方式")]
        public string PaymentMethod { get; set; }

        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public decimal ProjectTargetAmount { get; set; }
        public decimal ProjectCurrentAmount { get; set; }
    }
} 
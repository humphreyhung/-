using System.ComponentModel.DataAnnotations;

namespace MVC_DB_.Models
{
    public class SponsorConfirmViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public decimal Amount { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentMethodDisplay => PaymentMethod switch
        {
            "credit" => "信用卡",
            "atm" => "ATM轉帳",
            "line" => "LINE Pay",
            _ => PaymentMethod
        };
        public string TransactionId { get; set; }
        public DateTime TransactionTime { get; set; }
    }
} 
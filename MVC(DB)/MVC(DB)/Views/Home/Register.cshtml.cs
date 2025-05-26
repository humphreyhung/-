using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace MVC_DB_.Views.Home
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "請輸入電子郵件")]
        [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址")]
        public string Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "請輸入密碼")]
        [MinLength(6, ErrorMessage = "密碼長度至少為6個字符")]
        public string Password { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "請確認密碼")]
        [Compare("Password", ErrorMessage = "密碼和確認密碼不相符")]
        public string ConfirmPassword { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 這裡添加註冊邏輯
            // 例如：創建新用戶、保存到數據庫等

            // 註冊成功後重定向到登入頁面
            return RedirectToPage("/Login");
        }
    }
} 
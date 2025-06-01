using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MVC_DB_.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "請輸入電子郵件")]
            [EmailAddress(ErrorMessage = "電子郵件格式不正確")]
            [Display(Name = "電子郵件")]
            public string Email { get; set; }

            [Required(ErrorMessage = "請輸入密碼")]
            [StringLength(100, ErrorMessage = "{0} 必須至少包含 {2} 個字元，最多 {1} 個字元。", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "密碼")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "確認密碼")]
            [Compare("Password", ErrorMessage = "密碼和確認密碼不相符。")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            
            if (ModelState.IsValid)
            {
                _logger.LogInformation($"Attempting to register new user: {Input.Email}");

                // 检查用户是否已存在
                var existingUser = await _userManager.FindByEmailAsync(Input.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning($"User already exists: {Input.Email}");
                    ModelState.AddModelError(string.Empty, "此電子郵件已被註冊。");
                    return Page();
                }

                var user = new IdentityUser 
                { 
                    UserName = Input.Email, 
                    Email = Input.Email,
                    EmailConfirmed = true // 由于我们不需要邮件确认，直接设置为true
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                
                _logger.LogInformation($"User creation result for {Input.Email}: Succeeded={result.Succeeded}");
                if (result.Succeeded)
                {
                    _logger.LogInformation($"User created successfully: {user.Id}");

                    // 尝试立即登录
                    var signInResult = await _signInManager.PasswordSignInAsync(user, Input.Password, false, lockoutOnFailure: false);
                    
                    _logger.LogInformation($"Auto sign-in result for {Input.Email}: Succeeded={signInResult.Succeeded}");
                    if (signInResult.Succeeded)
                    {
                        _logger.LogInformation($"User automatically signed in after registration: {Input.Email}");
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to automatically sign in user after registration: {Input.Email}");
                        ModelState.AddModelError(string.Empty, "註冊成功但無法自動登入，請嘗試手動登入。");
                        return RedirectToPage("./Login");
                    }
                }

                foreach (var error in result.Errors)
                {
                    _logger.LogWarning($"Registration error for {Input.Email}: {error.Description}");
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                _logger.LogWarning("Invalid ModelState during registration attempt");
                foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning($"Model error: {modelError.ErrorMessage}");
                }
            }

            return Page();
        }
    }
} 
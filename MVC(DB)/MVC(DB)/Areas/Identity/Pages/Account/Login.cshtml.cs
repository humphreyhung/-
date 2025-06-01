using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MVC_DB_.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "請輸入電子郵件")]
            [EmailAddress(ErrorMessage = "電子郵件格式不正確")]
            [Display(Name = "電子郵件")]
            public string Email { get; set; }

            [Required(ErrorMessage = "請輸入密碼")]
            [DataType(DataType.Password)]
            [Display(Name = "密碼")]
            public string Password { get; set; }

            [Display(Name = "記住我")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                _logger.LogInformation($"Attempting to sign in user: {Input.Email}");

                // 检查用户是否存在
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    _logger.LogWarning($"User not found: {Input.Email}");
                    ModelState.AddModelError(string.Empty, "電子郵件或密碼不正確。");
                    return Page();
                }

                _logger.LogInformation($"User found: {user.Id}, Email confirmed: {user.EmailConfirmed}");

                // 尝试登录
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                
                _logger.LogInformation($"Sign in result for {Input.Email}: Succeeded={result.Succeeded}, RequiresTwoFactor={result.RequiresTwoFactor}, IsLockedOut={result.IsLockedOut}");

                if (result.Succeeded)
                {
                    _logger.LogInformation($"User logged in successfully: {Input.Email}");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning($"User account locked out: {Input.Email}");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    _logger.LogWarning($"Invalid login attempt for {Input.Email}");
                    ModelState.AddModelError(string.Empty, "電子郵件或密碼不正確。");
                    return Page();
                }
            }

            _logger.LogWarning("Invalid ModelState during login attempt");
            foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
            {
                _logger.LogWarning($"Model error: {modelError.ErrorMessage}");
            }

            return Page();
        }
    }
} 
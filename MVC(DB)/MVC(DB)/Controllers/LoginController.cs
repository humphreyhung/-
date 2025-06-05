using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace MVC_DB_.Controllers
{
    public class LoginController : Controller
    {
       

            [BindProperty]
            public bool RememberMe { get; set; }

            public void OnGet()
            {
            }
            public IActionResult Index()
        {

            return View();
        }

        string connectionString = "Server=tcp:finalprojectmvcdbserver20250520dbserver.database.windows.net,1433;Initial Catalog=finalProjectMVCDBserver20250520;Persist Security Info=False;User ID=humphreyhung;Password=Hum921026~;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";
        public IActionResult login(string username, string password, bool rememberMe)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return BadRequest("帳號或密碼不能空白");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT password, role, email FROM accountInformation WHERE userName = @u";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            return BadRequest("帳號不存在");

                        string storedHashedPassword = reader.GetString(0);
                        string role = reader.GetString(1);
                        string email = reader.GetString(2);

                        // 使用 BCrypt 驗證密碼
                        if (BCrypt.Net.BCrypt.Verify(password, storedHashedPassword))
                        {
                            // 儲存登入狀態與角色
                            HttpContext.Session.SetString("username", username);
                            HttpContext.Session.SetString("role", role);
                            HttpContext.Session.SetString("email", email);

                            // 如果選擇記住我，設置 Cookie
                            if (rememberMe)
                            {
                                var cookieOptions = new CookieOptions
                                {
                                    Expires = DateTime.Now.AddDays(30),
                                    HttpOnly = true,
                                    Secure = true,
                                    SameSite = SameSiteMode.Strict
                                };
                                Response.Cookies.Append("rememberedUser", username, cookieOptions);
                            }

                            // 根據角色導向
                            if (role == "Admin")
                                return RedirectToAction("AllAccounts", "Account");
                            else
                                return RedirectToAction("Index_R", "Home");
                        }
                        else
                        {
                            return BadRequest("密碼錯誤");
                        }
                    }
                }
            }
        }




        public IActionResult Signupp(string name, string username, string password, string email)
        {
            if (username.Length < 8 || username.Length > 20)
                return BadRequest("帳號需介於 8 至 20 字元");
            if (password.Length < 8 || password.Length > 20)
                return BadRequest("密碼需介於 8 至 20 字元");
            if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return BadRequest("Email 格式不正確");
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
                return BadRequest("欄位不能空白");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                
                // 檢查帳號是否已存在
                string checkSql = "SELECT COUNT(*) FROM accountInformation WHERE userName = @username";
                using (SqlCommand checkCmd = new SqlCommand(checkSql, conn))
                {
                    checkCmd.Parameters.AddWithValue("@username", username);
                    int exists = (int)checkCmd.ExecuteScalar();
                    if (exists > 0)
                        return BadRequest("帳號已被註冊");
                }

                // 插入新帳號
                string insertSql = "INSERT INTO accountInformation (userName, password, name, email, role) VALUES (@username, @password, @name, @email, @role)";
                using (SqlCommand cmd = new SqlCommand(insertSql, conn))
                {
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", hashedPassword);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@role", "Admin");

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        return RedirectToAction("Index", "Login");
                    }
                    else
                    {
                        return Content("fail");
                    }
                }
            }
        }
        public IActionResult Signup()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // 清除所有 Session
            return RedirectToAction("Index_R", "Home"); // 回到登入頁
        }


    }
}
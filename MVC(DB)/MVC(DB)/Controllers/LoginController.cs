using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace MVC_DB_.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        string connectionString = "Server=tcp:finalprojectmvcdbserver20250520dbserver.database.windows.net,1433;Initial Catalog=finalProjectMVCDBserver20250520;Persist Security Info=False;User ID=humphreyhung;Password=Hum921026~;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";
        public IActionResult login(string username, string password)
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
                string sql = "INSERT INTO accountInformation (userName, password,name,email,role) VALUES (@u, @p,@n,@e,@r)";

                string checkSql = "SELECT COUNT(*) FROM accountInformation WHERE userName = @u";
                SqlCommand checkCmd = new SqlCommand(checkSql, conn);
                checkCmd.Parameters.AddWithValue("@u", username);
                int exists = (int)checkCmd.ExecuteScalar();
                if (exists > 0)
                    return BadRequest("帳號已被註冊");


                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", hashedPassword);
                    cmd.Parameters.AddWithValue("@n", name);
                    cmd.Parameters.AddWithValue("@e", email);
                    cmd.Parameters.AddWithValue("@r", "User");
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
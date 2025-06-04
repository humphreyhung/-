using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;


namespace MVC_DB_.Controllers
{
    public class AccountController : Controller
    {
        string connectionString = "Server=tcp:finalprojectmvcdbserver20250520dbserver.database.windows.net,1433;Initial Catalog=finalProjectMVCDBserver20250520;Persist Security Info=False;User ID=humphreyhung;Password=Hum921026~;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";
        public IActionResult AllAccounts()
        {
            if (HttpContext.Session.GetString("role") != "Admin")
            {
                return Unauthorized("您沒有權限訪問此頁面");
            }
            var accountList = new List<AccountInfo>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT userName, name, email, role FROM accountInformation";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        accountList.Add(new AccountInfo
                        {
                            UserName = reader["userName"].ToString(),
                            Name = reader["name"].ToString(),
                            Email = reader["email"].ToString(),
                            Role = reader["role"].ToString()
                        });
                    }
                }
            }

            return View(accountList);
        }

        [HttpPost]
        public IActionResult UpdateRole(string username, string role)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
                return BadRequest("資料不完整");

            if (HttpContext.Session.GetString("role") != "Admin")
            {
                return Unauthorized("您沒有權限執行此操作");
            }


            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE accountInformation SET role = @r WHERE userName = @u";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@r", role);
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("AllAccounts");
        }

        [HttpPost]
        public IActionResult DeleteAccount(string username)
        {
            if (HttpContext.Session.GetString("role") != "Admin")
                return Unauthorized("您沒有權限執行此操作");

            string currentUser = HttpContext.Session.GetString("username");
            if (username == currentUser)
                return BadRequest("不能刪除目前登入的帳號");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "DELETE FROM accountInformation WHERE userName = @u";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("AllAccounts");
        }

        // 顯示個人資料頁面
        public IActionResult Profile()
        {
            string username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username)) return RedirectToAction("Login", "Home");

            var model = new UserProfileViewModel();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // 查主帳號資料
                string sqlUser = "SELECT id, email FROM accountInformation WHERE userName = @username";
                int accountId = 0;

                using (SqlCommand cmd = new SqlCommand(sqlUser, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model.UserName = username;
                            model.Email = reader["email"].ToString();
                            accountId = Convert.ToInt32(reader["id"]);
                        }
                    }
                }

                // 查聯絡資料
                string sqlContact = "SELECT phone, city, district, addressDetail FROM accountContact WHERE accountId = @accountId";
                using (SqlCommand cmd = new SqlCommand(sqlContact, conn))
                {
                    cmd.Parameters.AddWithValue("@accountId", accountId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model.Phone = reader["phone"]?.ToString();
                            model.City = reader["city"]?.ToString();
                            model.District = reader["district"]?.ToString();
                            model.DetailAddress = reader["addressDetail"]?.ToString();
                        }
                    }
                }
            }

            return View(model);
        }

        // 儲存個人資料
        [HttpPost]
        public IActionResult Profile(UserProfileViewModel model)
        {
            string username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username)) return RedirectToAction("Login", "Home");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // 查使用者 ID
                int accountId = 0;
                string sqlUser = "SELECT id, email FROM accountInformation WHERE userName = @username";
                using (SqlCommand cmd = new SqlCommand(sqlUser, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            accountId = Convert.ToInt32(reader["id"]);
                            model.UserName = username;
                            model.Email = reader["email"].ToString(); // ← 這裡填回 Email
                        }
                    }
                }

                if (accountId == 0) return NotFound("找不到帳號");

                // 判斷是否已有聯絡資料
                string checkSql = "SELECT COUNT(*) FROM accountContact WHERE accountId = @accountId";
                bool exists = false;

                using (SqlCommand cmd = new SqlCommand(checkSql, conn))
                {
                    cmd.Parameters.AddWithValue("@accountId", accountId);
                    exists = (int)cmd.ExecuteScalar() > 0;
                }

                if (exists)
                {
                    // 更新
                    string updateSql = @"UPDATE accountContact 
                                 SET phone = @phone, city = @city, district = @district, addressDetail = @detail 
                                 WHERE accountId = @accountId";
                    using (SqlCommand cmd = new SqlCommand(updateSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@phone", model.Phone ?? "");
                        cmd.Parameters.AddWithValue("@city", model.City ?? "");
                        cmd.Parameters.AddWithValue("@district", model.District ?? "");
                        cmd.Parameters.AddWithValue("@detail", model.DetailAddress ?? "");
                        cmd.Parameters.AddWithValue("@accountId", accountId);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // 新增
                    string insertSql = @"INSERT INTO accountContact (accountId, phone, city, district, addressDetail)
                                 VALUES (@accountId, @phone, @city, @district, @detail)";
                    using (SqlCommand cmd = new SqlCommand(insertSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@accountId", accountId);
                        cmd.Parameters.AddWithValue("@phone", model.Phone ?? "");
                        cmd.Parameters.AddWithValue("@city", model.City ?? "");
                        cmd.Parameters.AddWithValue("@district", model.District ?? "");
                        cmd.Parameters.AddWithValue("@detail", model.DetailAddress ?? "");
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            ViewBag.Message = "資料已更新成功";
            return View(model);
        }



    }



    public class AccountInfo
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }

    public class UserProfileViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }

        // 可填寫欄位
        public string Phone { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string DetailAddress { get; set; }
    }



}

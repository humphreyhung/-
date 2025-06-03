using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;


namespace MVC_DB_.Controllers
{
    public class AccountController : Controller
    {
        string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Database=account;User ID=don1;Password=Qw669668;Trusted_Connection=True";

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


    }



    public class AccountInfo
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }

    

}

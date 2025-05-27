using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Text;
using System.Security.Cryptography;
using BCrypt.Net;

namespace MVC_DB_.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Database=account;User ID=don1;Password=Qw669668;Trusted_Connection=True";
        public IActionResult login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return BadRequest("帳號或密碼不能空白");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT password FROM accountInformation WHERE userName = @u";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);

                    var result = cmd.ExecuteScalar();

                    if (result == null)
                    {
                        // 帳號不存在
                        return BadRequest("帳號不存在");
                    }

                    string storedHashedPassword = result.ToString();

                    // 使用 BCrypt 驗證密碼是否正確
                    if (BCrypt.Net.BCrypt.Verify(password, storedHashedPassword))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return BadRequest("密碼錯誤");
                    }
                }
            }
        }



        public IActionResult Signupp(string name,string username, string password )
        {
            
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(name))
                return BadRequest("欄位不能空白");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO accountInformation (userName, password,name,email) VALUES (@u, @p,@n,@e)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", hashedPassword); // ⚠️ 實務應改用雜湊密碼驗證
                    cmd.Parameters.AddWithValue("@n", name);
                    cmd.Parameters.AddWithValue("@e", name);//修改
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
        
    }
}

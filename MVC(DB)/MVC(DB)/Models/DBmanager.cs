using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_DB_.Models
{
    public class DBmanager
    {
        private readonly string connStr = "Server=tcp:finalprojectmvcdbserver20250520dbserver.database.windows.net,1433;Initial Catalog=finalProjectMVCDBserver20250520;Persist Security Info=False;User ID={your User ID};Password={your Password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";
        public List<account> getAccounts()
        {
            List<account> accounts = new List<account>();

            SqlConnection sqlConnection = new SqlConnection(connStr);
            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM accountInformation");
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    account account = new account
                    {
                        ID = reader.GetInt32(reader.GetOrdinal("id")),
                        userName = reader.GetString(reader.GetOrdinal("userName")),
                        passwd = reader.GetString(reader.GetOrdinal("passwd")),
                        name = reader.GetString(reader.GetOrdinal("name")),
                    };
                    accounts.Add(account);
                }
            }
            else
            {
                Console.WriteLine("資料庫為空！");
            }
            sqlConnection.Close();
            return accounts;
        }

        public void newAccount(account user)
        {
            SqlConnection sqlconnection = new SqlConnection(connStr);
            SqlCommand sqlcommand = new SqlCommand(@"INSERT INTO accountInformation(username,passwd,name) VALUES(@username,@passwd,@name)");
            sqlcommand.Connection = sqlconnection;
           
            sqlcommand.Parameters.Add(new SqlParameter("@username", user.userName));
            sqlcommand.Parameters.Add(new SqlParameter("@passwd", user.passwd));
            sqlcommand.Parameters.Add(new SqlParameter("@name", user.name));
            
            sqlconnection.Open();
            sqlcommand.ExecuteNonQuery();
            sqlconnection.Close();
        }
    }
}

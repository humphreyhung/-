using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MVC_DB_.Models
{
    public class DBmanager
    {
        private readonly string connStr;
        private readonly ILogger<DBmanager> _logger;

        public DBmanager(IConfiguration configuration, ILogger<DBmanager> logger)
        {
            _logger = logger;
            try
            {
                connStr = configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(connStr))
                {
                    throw new ArgumentException("Connection string 'DefaultConnection' not found in configuration.");
                }
                _logger.LogInformation("Connection string loaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading connection string");
                throw;
            }
        }

        public List<account> getAccounts()
        {
            List<account> accounts = new List<account>();

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connStr))
                {
                    _logger.LogInformation("Attempting to open database connection...");
                    sqlConnection.Open();
                    _logger.LogInformation("Database connection opened successfully");

                    using (SqlCommand sqlCommand = new SqlCommand("SELECT * FROM accountInformation", sqlConnection))
                    {
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
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
                                _logger.LogInformation($"Retrieved {accounts.Count} accounts from database");
                            }
                            else
                            {
                                _logger.LogWarning("No accounts found in database");
                            }
                        }
                    }
                }
                return accounts;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL Error occurred while getting accounts. Error Number: {ErrorNumber}, State: {State}, Class: {Class}", 
                    ex.Number, ex.State, ex.Class);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while getting accounts");
                throw;
            }
        }

        public void newAccount(account user)
        {
            try
            {
                using (SqlConnection sqlconnection = new SqlConnection(connStr))
                {
                    _logger.LogInformation("Attempting to open database connection for new account creation...");
                    sqlconnection.Open();
                    _logger.LogInformation("Database connection opened successfully");

                    using (SqlCommand sqlcommand = new SqlCommand(@"INSERT INTO accountInformation(username,passwd,name) VALUES(@username,@passwd,@name)", sqlconnection))
                    {
                        sqlcommand.Parameters.Add(new SqlParameter("@username", user.userName));
                        sqlcommand.Parameters.Add(new SqlParameter("@passwd", user.passwd));
                        sqlcommand.Parameters.Add(new SqlParameter("@name", user.name));
                        
                        sqlcommand.ExecuteNonQuery();
                        _logger.LogInformation($"Successfully created new account for user: {user.userName}");
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL Error occurred while creating new account. Error Number: {ErrorNumber}, State: {State}, Class: {Class}", 
                    ex.Number, ex.State, ex.Class);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while creating new account");
                throw;
            }
        }
    }
}

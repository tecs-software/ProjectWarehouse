using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WarehouseManagement.Database
{
    internal class DatabaseInitializer
    {
        private readonly string connectionString;

        public DatabaseInitializer(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<bool> CreateDatabaseIfNotExists(string databaseName)
        {
            using (DatabaseConnection connection = new(connectionString))
            {
                using SqlConnection? sqlConnection = await connection.OpenConnection();

                if(sqlConnection == null)
                {
                    return false;
                }

                using SqlCommand command = sqlConnection.CreateCommand();
                command.CommandText = $"SELECT database_id FROM sys.databases WHERE Name = '{databaseName}'";
                object? result = await command.ExecuteScalarAsync();

                if (result == null)
                {
                    command.CommandText = $"CREATE DATABASE [{databaseName}]";
                    await command.ExecuteNonQueryAsync();
                }

                // Execute the SQL script to create tables
                string scriptPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Components", "script.sql");
                string script = System.IO.File.ReadAllText(scriptPath);
                var batches = script.Split(new string[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

                using SqlTransaction transaction = sqlConnection.BeginTransaction();

                command.Transaction = transaction;
                try
                {
                    foreach (var batch in batches)
                    {
                        command.CommandText = batch;
                        await command.ExecuteNonQueryAsync();
                    }


                    transaction.Commit();

                    await InsertInitialAdminUser(databaseName);

                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Handle the exception or log the error
                    MessageBox.Show($"An error occurred while executing the SQL script: {ex.Message}");
                }
            }

            return false;
        }

        public async Task<bool> InsertInitialAdminUser(string databaseName)
        {
            try
            {
                using var connection = new SqlConnection($"{connectionString};Database={databaseName};");

                await connection.OpenAsync();


                SqlCommand checkCommand = connection.CreateCommand();
                checkCommand.CommandText = "SELECT COUNT(*) FROM tbl_user_access WHERE access_level = 'ADMIN'";
                int adminCount = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());

                if (adminCount > 0)
                {
                    return true;
                }

                SqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO tbl_users (first_name, middle_name, last_name, email, contact_number, authentication_code, username, password, status) VALUES (@first_name, @middle_name, @last_name, @email, @contact_number, @authentication_code, @username, @password_hash, @status)";
                command.Parameters.AddWithValue("@first_name", "Admin");
                command.Parameters.AddWithValue("@middle_name", "Admin");
                command.Parameters.AddWithValue("@last_name", "Admin");
                command.Parameters.AddWithValue("@email", "Admin@admin.com");
                command.Parameters.AddWithValue("@contact_number", "Admin");
                command.Parameters.AddWithValue("@authentication_code", "ADMIN");
                command.Parameters.AddWithValue("@username", "admin");
                command.Parameters.AddWithValue("@status", "Enabled");
                command.Parameters.AddWithValue("@password_hash", "e201065d0554652615c320c00a1d5bc8edca469d72c2790e24152d0c1e2b");

                await command.ExecuteNonQueryAsync();

                command.CommandText = "SELECT @@IDENTITY";
                int adminUserId = Convert.ToInt32(await command.ExecuteScalarAsync());

                command.CommandText = "INSERT INTO tbl_user_access (user_id, access_level) VALUES (@user_id, @access_level)";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@user_id", adminUserId);
                command.Parameters.AddWithValue("@access_level", "ADMIN");

                await command.ExecuteNonQueryAsync();

                return true;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Initial admin user insertion failed. Error code: " + ex.ErrorCode);
                return false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace WarehouseManagement.Database
{
    public class DatabaseConnection : IDisposable
    {
        private string? connectionString;
        private SqlConnection? connection;
        protected SqlTransaction? transaction;
        int retries = 3;
        int delayMilliseconds = 100;

        public DatabaseConnection(string? connectionString = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(connectionString))
                {
                    this.connectionString = connectionString;
                }
                else if (ConfigurationManager.ConnectionStrings["MyConnectionString"] != null)
                {
                    this.connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
                }
            }
            catch(Exception e)
            {

            }
        }

        public static async Task<bool> TestConnection(string testConnection)
        {
            int retries = 1;
            int delayMilliseconds = 100;

            for (int i = 0; i < retries; i++)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(testConnection))
                    {
                        await connection.OpenAsync();
                        return true;
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"Failed to connect to the database (attempt {i + 1} of {retries}): {ex.Message}");
                    delayMilliseconds *= 2;
                    await Task.Delay(delayMilliseconds);
                }
            }

            return false;
        }

        public async Task<SqlConnection?> OpenConnection()
        {
            for (int i = 0; i < retries; i++)
            {
                try
                {
                    connection = new SqlConnection(connectionString);
                    await connection.OpenAsync();
                    return connection;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"Failed to open database connection (attempt {i + 1} of {retries}): {ex.Message}");
                    delayMilliseconds *= 2;
                    await Task.Delay(delayMilliseconds);
                }
            }

            return null;
        }

        public SqlTransaction? BeginTransaction()
        {
            if (connection == null || connection.State != System.Data.ConnectionState.Open)
            {
                return null; // Return null if connection is not open
            }

            transaction = connection.BeginTransaction();
            return transaction;
        }

        public void CommitTransaction()
        {
            transaction?.Commit();
        }

        public void RollbackTransaction()
        {
            transaction?.Rollback();
        }

        public void CloseConnection()
        {
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public SqlCommand? CreateCommand()
        {
            if (connection == null || connection.State != System.Data.ConnectionState.Open)
            {
                return null; // Return null if connection is not open
            }

            SqlCommand command = connection.CreateCommand();

            if (command == null)
            {
                return null; // Return null if command object could not be created
            }

            return command;
        }

        public void Dispose()
        {
            connection?.Dispose();
        }
    }
}

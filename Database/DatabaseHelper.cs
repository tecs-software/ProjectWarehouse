using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WarehouseManagement.Database
{
    internal class DatabaseHelper : DatabaseConnection
    {
        public static async Task<bool> IsDataExistsAsync(string tableName, string columnName, string value)
        {
            bool result = false;
            try
            {
                using DatabaseConnection connection = new();
                await connection.OpenConnection();

                using SqlCommand? command = connection.CreateCommand();

                if (command == null)
                {
                    return false;
                }

                command.CommandText = $"SELECT COUNT(*) FROM {tableName} WHERE {columnName} = @value";
                command.Parameters.AddWithValue("@value", value);

                object? commandResult = await command.ExecuteScalarAsync();
                int count = commandResult != null ? (int)commandResult : 0;

                if (count > 0)
                {
                    result = true;
                }
            }
            catch (SqlException)
            {
                return false;
            }

            return result;
        }

        public static async Task<bool> AuthenticateUser(string username, string password)
        {
            try
            {
                using DatabaseConnection connection = new();

                await connection.OpenConnection();

                SqlCommand? command = connection.CreateCommand();

                if (command == null)
                {
                    return false;
                }

                command.CommandText = "SELECT COUNT(*) FROM tbl_users WHERE username = @username AND password = @password_hash AND status = 'Enabled'";
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password_hash", password);

                int count = Convert.ToInt32(await command.ExecuteScalarAsync());

                return count == 1;
            }
            catch (SqlException)
            {
                return false;
            }
        }

    }
}

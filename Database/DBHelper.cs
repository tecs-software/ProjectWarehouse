using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using WarehouseManagement.Models;

namespace WarehouseManagement.Database
{
    internal class DBHelper : DatabaseConnection
    {

        public async Task<DataTable?> GetTableData(string tableName, IEnumerable<string> columns)
        {
            using DatabaseConnection dbConnection = new();

            string columnsStr = string.Join(", ", columns);
            string query = $"SELECT {columnsStr} FROM {tableName}";

            SqlConnection? connection = await dbConnection.OpenConnection();

            try
            {
                using SqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@TableName", tableName);

                using SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
                DataTable dataTable = new();
                dataTable.Load(reader);
                return dataTable;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
            finally
            {
                CloseConnection();
            }
        }

        public async Task<bool> IsDataExistsAsync(string tableName, string columnName, string value)
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
            finally
            {
                CloseConnection();
            }

            return result;
        }

        public async Task<bool> InsertData(string tableName, string[] columns, string[] values)
        {
            using DatabaseConnection dbConnection = new();

            try
            {
                SqlConnection? connection = await dbConnection.OpenConnection();

                if(connection == null)
                {
                    return false;
                }

                using SqlCommand command = connection.CreateCommand();

                string query = $"INSERT INTO {tableName} ({string.Join(",", columns)}) " +
                               $"VALUES ({string.Join(",", values.Select((v, i) => $"@value{i}"))});";

                command.CommandText = query;

                for (int i = 0; i < values.Length; i++)
                {
                    command.Parameters.AddWithValue($"@value{i}", values[i]);
                }

                int result = await command.ExecuteNonQueryAsync();

                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inserting data: " + ex.Message);
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }

        public async Task<bool> UpdateData(string tableName, string[] columns, string[] values, string whereColumnName, string whereColumnValue)
        {
            using DatabaseConnection dbConnection = new();

            try
            {
                SqlConnection? connection = await dbConnection.OpenConnection();

                if (connection == null)
                {
                    return false;
                }

                using SqlCommand command = connection.CreateCommand();

                string setClause = string.Join(", ", columns.Select((c, i) => $"{c} = @value{i}"));
                string query = $"UPDATE {tableName} SET {setClause} WHERE {whereColumnName} = @whereColumnValue";

                command.CommandText = query;

                for (int i = 0; i < values.Length; i++)
                {
                    command.Parameters.AddWithValue($"@value{i}", values[i]);
                }

                command.Parameters.AddWithValue("@whereColumnValue", whereColumnValue);

                int result = await command.ExecuteNonQueryAsync();

                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating data: " + ex.Message);
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }

        public async Task<string?> InsertOrUpdateData(string tableName, string[] columns, string[] values, string idColumnName, string idValue)
        {
            using DatabaseConnection dbConnection = new();

            try
            {
                SqlConnection? connection = await dbConnection.OpenConnection();

                if (connection == null)
                {
                    return null;
                }

                using SqlCommand command = connection.CreateCommand();

                string query = $"IF EXISTS (SELECT 1 FROM {tableName} WHERE {idColumnName} = @idValue) " +
                               $"UPDATE {tableName} SET {string.Join(", ", columns.Select((c, i) => $"{c} = @value{i}"))} " +
                               $"WHERE {idColumnName} = @idValue " +
                               $"ELSE " +
                               $"INSERT INTO {tableName} ({idColumnName}, {string.Join(",", columns)}) " +
                               $"VALUES (@idValue, {string.Join(",", values.Select((v, i) => $"@value{i}"))})";

                command.CommandText = query;
                command.Parameters.AddWithValue("@idValue", idValue);

                for (int i = 0; i < values.Length; i++)
                {
                    command.Parameters.AddWithValue($"@value{i}", values[i]);
                }

                int result = await command.ExecuteNonQueryAsync();

                return result > 0 ? (result == 1 ? "inserted" : "updated") : null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inserting/updating data: " + ex.Message);
                return null;
            }
            finally
            {
                CloseConnection();
            }
        }

        public async Task<bool> AuthenticateUser(string username, string password)
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
            finally
            {
                CloseConnection();
            }
        }

        public async Task<bool> AuthenticationCheck(string tableName, string columnName, string valueToCheck)
        {
            try
            {
                using DatabaseConnection connection = new();

                await connection.OpenConnection();

                SqlCommand? command = connection.CreateCommand();

                if (command == null)
                {
                    MessageBox.Show("null");
                    return false;
                }

                command.CommandText = $"SELECT COUNT(*) FROM {tableName} WHERE {columnName} = @valueToCheck AND (username IS NULL OR username = '') AND (password IS NULL OR password = '')";
                command.Parameters.AddWithValue("@valueToCheck", valueToCheck);

                int count = Convert.ToInt32(command.ExecuteScalar());

                return count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public async Task<bool> RegisterUser(string tableName, string[] columns, string[] values, string idColumn, string idValue)
        {
            using DatabaseConnection connection = new();

            await connection.OpenConnection();

            SqlCommand? command = connection.CreateCommand();
            SqlTransaction? transaction = null;

            try
            {
                if (connection == null && command == null)
                {
                    return false;
                }

                transaction = connection.BeginTransaction();
                command.Transaction = transaction;

                // Build UPDATE query for tableName
                StringBuilder updateQuery = new StringBuilder();
                updateQuery.Append("UPDATE ").Append(tableName).Append(" SET ");
                for (int i = 0; i < columns.Length; i++)
                {
                    updateQuery.Append(columns[i]).Append("=@").Append(i);
                    if (i < columns.Length - 1)
                    {
                        updateQuery.Append(", ");
                    }
                    command.Parameters.AddWithValue("@" + i, values[i]);
                }
                updateQuery.Append(" WHERE ").Append(idColumn).Append("=@id");
                command.Parameters.AddWithValue("@id", idValue);

                command.CommandText = updateQuery.ToString();

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    transaction.Rollback();
                    return false;
                }

                // Query the updated row to get user_id
                string selectQuery = "SELECT user_id FROM " + tableName + " WHERE " + idColumn + "=@id";
                command.CommandText = selectQuery;

                int userId = Convert.ToInt32(command.ExecuteScalar());

                // Insert into tbl_active_users
                string activeUsersQuery = "INSERT INTO tbl_active_users (user_id) VALUES (@user_id)";
                command.CommandText = activeUsersQuery;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@user_id", userId);

                int activeUsersRowsAffected = command.ExecuteNonQuery();

                if (activeUsersRowsAffected == 0)
                {
                    transaction.Rollback();
                    return false;
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                MessageBox.Show("Error updating data: " + ex.Message);
                return false;
            }
        }

        public async Task<List<Dictionary<string, object>>?> GetRowsExcluded(string tableName, IEnumerable<string> columnNames, Dictionary<string, object> excludedFilters)
        {
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

            try
            {
                using (DatabaseConnection connection = new DatabaseConnection())
                {
                    await connection.OpenConnection();

                    if (connection != null)
                    {
                        string query = $"SELECT {string.Join(", ", columnNames)} FROM {tableName}";
                        if (excludedFilters != null && excludedFilters.Count > 0)
                        {
                            query += " WHERE ";
                            int i = 0;
                            foreach (KeyValuePair<string, object> filter in excludedFilters)
                            {
                                if (i > 0)
                                {
                                    query += " AND ";
                                }
                                query += $"{filter.Key} != @{filter.Key}";
                                i++;
                            }
                        }

                        using (SqlCommand? cmd = connection.CreateCommand())
                        {
                            if (cmd == null)
                            {
                                return null;
                            }

                            cmd.CommandText = query;

                            if (excludedFilters != null && excludedFilters.Count > 0)
                            {
                                foreach (KeyValuePair<string, object> filter in excludedFilters)
                                {
                                    cmd.Parameters.AddWithValue($"@{filter.Key}", filter.Value);
                                }
                            }

                            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    Dictionary<string, object> row = new Dictionary<string, object>();
                                    foreach (string columnName in columnNames)
                                    {
                                        row[columnName] = reader[columnName];
                                    }
                                    rows.Add(row);
                                }
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                return rows;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Dictionary<string, object>?> GetRow(string tableName, IEnumerable<string> columnNames, string whereColumn, object whereValue)
        {
            Dictionary<string, object> row = new Dictionary<string, object>();

            try
            {
                using (DatabaseConnection connection = new DatabaseConnection())
                {
                    await connection.OpenConnection();

                    if (connection != null)
                    {
                        string query = $"SELECT {string.Join(", ", columnNames)} FROM {tableName} WHERE {whereColumn} = @{whereColumn}";

                        using (SqlCommand? cmd = connection.CreateCommand())
                        {
                            if (cmd == null)
                            {
                                return null;
                            }

                            cmd.CommandText = query;
                            cmd.Parameters.AddWithValue($"@{whereColumn}", whereValue);

                            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    foreach (string columnName in columnNames)
                                    {
                                        row[columnName] = reader[columnName];
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                return row;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> InsertDataWithForeignKey(string firstTable, Dictionary<string, object> firstTableValues, string firstTableIdColumn, string secondTable, Dictionary<string, object> secondTableValues, string thirdTable, Dictionary<string, object> thirdTableValues)
        {
            using (DatabaseConnection connection = new DatabaseConnection())
            {
                await connection.OpenConnection();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        SqlCommand firstTableInsertCommand = connection.CreateCommand();
                        if (firstTableInsertCommand != null)
                        {
                            firstTableInsertCommand.Transaction = transaction;

                            // Build INSERT query for firstTable
                            StringBuilder firstTableInsertQuery = new StringBuilder();
                            firstTableInsertQuery.Append($"INSERT INTO {firstTable} ({string.Join(",", firstTableValues.Keys)}) OUTPUT INSERTED.{firstTableIdColumn} VALUES ({string.Join(",", firstTableValues.Keys.Select(key => "@" + key))})");
                            firstTableInsertCommand.CommandText = firstTableInsertQuery.ToString();
                            foreach (KeyValuePair<string, object> kvp in firstTableValues)
                            {
                                firstTableInsertCommand.Parameters.AddWithValue($"@{kvp.Key}", kvp.Value);
                            }
                            int generatedId = (int)await firstTableInsertCommand.ExecuteScalarAsync();

                            secondTableValues.Add(firstTableIdColumn, generatedId);
                            SqlCommand secondTableInsertCommand = connection.CreateCommand();
                            secondTableInsertCommand.Transaction = transaction;

                            // Build INSERT query for secondTable
                            StringBuilder secondTableInsertQuery = new StringBuilder();
                            secondTableInsertQuery.Append($"INSERT INTO {secondTable} ({string.Join(",", secondTableValues.Keys)}) VALUES ({string.Join(",", secondTableValues.Keys.Select(key => "@" + key))})");
                            secondTableInsertCommand.CommandText = secondTableInsertQuery.ToString();
                            foreach (KeyValuePair<string, object> kvp in secondTableValues)
                            {
                                secondTableInsertCommand.Parameters.AddWithValue($"@{kvp.Key}", kvp.Value);
                            }
                            await secondTableInsertCommand.ExecuteNonQueryAsync();

                            thirdTableValues.Add(firstTableIdColumn, generatedId);
                            SqlCommand thirdTableInsertCommand = connection.CreateCommand();
                            thirdTableInsertCommand.Transaction = transaction;

                            // Build INSERT query for thirdTable
                            StringBuilder thirdTableInsertQuery = new StringBuilder();
                            thirdTableInsertQuery.Append($"INSERT INTO {thirdTable} ({string.Join(",", thirdTableValues.Keys)}) VALUES ({string.Join(",", thirdTableValues.Keys.Select(key => "@" + key))})");
                            thirdTableInsertCommand.CommandText = thirdTableInsertQuery.ToString();
                            foreach (KeyValuePair<string, object> kvp in thirdTableValues)
                            {
                                thirdTableInsertCommand.Parameters.AddWithValue($"@{kvp.Key}", kvp.Value);
                            }
                            await thirdTableInsertCommand.ExecuteNonQueryAsync();

                            transaction.Commit();
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("Failed to create SqlCommand for the first table.");
                            transaction.Rollback();
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public async Task<string> GetValue(string tableName, string columnNameToGet, string whereColumnName, string whereColumnValue)
        {
            try
            {
                using DatabaseConnection connection = new();
                await connection.OpenConnection();

                using SqlCommand? command = connection.CreateCommand();

                if (command == null)
                {
                    return string.Empty;
                }

                command.CommandText = $"SELECT {columnNameToGet} FROM {tableName} WHERE {whereColumnName} = @whereColumnValue";
                command.Parameters.AddWithValue("@whereColumnValue", whereColumnValue);

                object? commandResult = await command.ExecuteScalarAsync();
                string value = commandResult?.ToString() ?? string.Empty;
                return value;
            }
            catch (SqlException)
            {
                return string.Empty;
            }
            finally
            {
                CloseConnection();
            }
        }

    }
}

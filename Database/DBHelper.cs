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
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;
using WWarehouseManagement.Database;

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

        public async Task<DataTable> GetOrdersFiltered(Dictionary<string, string>? orFilters, Dictionary<string, string>? andFilters)
        {
            string tableName = "tbl_orders";
            string[] columns = {
        "order_id", "courier", "username", "receiver_name", "item_name",
        "quantity", "total", "status", "remarks", "created_at"
    };

            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append($"SELECT {string.Join(", ", columns)} FROM {tableName} o");
            queryBuilder.Append(" LEFT JOIN tbl_users u ON o.user_id = u.user_id");
            queryBuilder.Append(" LEFT JOIN tbl_receiver r ON o.receiver_id = r.receiver_id");
            queryBuilder.Append(" LEFT JOIN tbl_products p ON o.product_id = p.product_id");

            if (orFilters != null && orFilters.Count > 0)
            {
                queryBuilder.Append(" WHERE ");
                List<string> orFilterConditions = new List<string>();

                foreach (var filter in orFilters)
                {
                    string filterColumn = filter.Key;
                    string filterValue = filter.Value;
                    orFilterConditions.Add($"p.{filterColumn} LIKE '%' + @{filterColumn} + '%' OR r.{filterColumn} LIKE '%' + @{filterColumn} + '%' OR u.{filterColumn} LIKE '%' + @{filterColumn} + '%'");
                }

                queryBuilder.Append(string.Join(" OR ", orFilterConditions));
            }

            if (andFilters != null && andFilters.Count > 0)
            {
                if (orFilters == null || orFilters.Count == 0)
                    queryBuilder.Append(" WHERE ");
                else
                    queryBuilder.Append(" AND ");

                List<string> andFilterConditions = new List<string>();

                foreach (var filter in andFilters)
                {
                    string filterColumn = filter.Key;
                    string filterValue = filter.Value;
                    andFilterConditions.Add($"p.{filterColumn} LIKE '%' + @{filterColumn} + '%' OR r.{filterColumn} LIKE '%' + @{filterColumn} + '%' OR u.{filterColumn} LIKE '%' + @{filterColumn} + '%'");
                }

                queryBuilder.Append(string.Join(" AND ", andFilterConditions));
            }

            queryBuilder.Append(" ORDER BY o.created_at DESC");

            try
            {
                using (DatabaseConnection dbConnection = new DatabaseConnection())
                {
                    using (SqlConnection connection = await dbConnection.OpenConnection())
                    {
                        using (SqlCommand command = new SqlCommand(queryBuilder.ToString(), connection))
                        {
                            if (orFilters != null && orFilters.Count > 0)
                            {
                                foreach (var filter in orFilters)
                                {
                                    string filterColumn = filter.Key;
                                    string filterValue = filter.Value;
                                    command.Parameters.AddWithValue($"@{filterColumn}", filterValue);
                                }
                            }

                            if (andFilters != null && andFilters.Count > 0)
                            {
                                foreach (var filter in andFilters)
                                {
                                    string filterColumn = filter.Key;
                                    string filterValue = filter.Value;
                                    command.Parameters.AddWithValue($"@{filterColumn}", filterValue);
                                }
                            }

                            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                            {
                                DataTable dataTable = new DataTable();
                                await Task.Run(() => adapter.Fill(dataTable));
                                return dataTable;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<DataTable> GetTableFiltered(string tableName, IEnumerable<string> columns, Dictionary<string, string>? filters)
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append($"SELECT {string.Join(", ", columns)} FROM {tableName}");

            if (filters != null && filters.Count > 0)
            {
                queryBuilder.Append(" WHERE ");
                List<string> filterConditions = new List<string>();

                foreach (var filter in filters)
                {
                    string filterColumn = filter.Key;
                    string filterValue = filter.Value;
                    filterConditions.Add($"{filterColumn} LIKE '%{filterValue}%'");
                }

                queryBuilder.Append(string.Join(" AND ", filterConditions));
            }

            try
            {
                using (DatabaseConnection dbConnection = new DatabaseConnection())
                {
                    using (SqlConnection connection = await dbConnection.OpenConnection())
                    {
                        using (SqlCommand command = new SqlCommand(queryBuilder.ToString(), connection))
                        {
                            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                            {
                                DataTable dataTable = new DataTable();
                                await Task.Run(() => adapter.Fill(dataTable));
                                return dataTable;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task<DataTable?> GetTable(string query)
        {
            using (DatabaseConnection dbConnection = new DatabaseConnection())
            {
                SqlConnection? connection = await dbConnection.OpenConnection();

                if (connection != null && connection.State == ConnectionState.Open)
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                        {
                            DataTable dataTable = new DataTable();
                            dataTable.Load(reader);
                            return dataTable;
                        }
                    }
                }
            }

            return null;
        }

        public async Task<bool> SavePayrollChanges()
        {
            bool modified = false;

            using DatabaseConnection dbConnection = new();

            SqlConnection? connection = await dbConnection.OpenConnection();

            SqlCommand modifyCommand = new SqlCommand("UPDATE tbl_commissions SET is_valid = 1 WHERE is_valid = 0; UPDATE tbl_overtime SET is_valid = 1 WHERE is_valid = 0; UPDATE tbl_reimbursement SET is_valid = 1 WHERE is_valid = 0; UPDATE tbl_deductions SET is_valid = 1 WHERE is_valid = 0;", connection);

            int rowsAffected = modifyCommand.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                modified = true;
            }

            CloseConnection();

            return modified;
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
                using (DatabaseConnection connection = new DatabaseConnection())
                {
                    SqlConnection? conn = await connection.OpenConnection();

                    SqlCommand? command = connection.CreateCommand();

                    if (command == null)
                    {
                        return false;
                    }

                    command.CommandText = "SELECT user_id FROM tbl_users WHERE username = @username AND password = @password_hash AND status = 'Enabled'";
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password_hash", password);

                    int userID = 0;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            userID = (int)reader["user_id"];
                        }
                    }

                    if (userID != 0)
                    {
                        // User authentication successful, retrieve user data
                        string query = "SELECT u.first_name, u.middle_name, u.last_name, ua.role_id " +
                                       "FROM tbl_users u " +
                                       "INNER JOIN tbl_access_level ua ON u.user_id = ua.user_id " +
                                       "WHERE u.user_id = @user_id";

                        using (SqlCommand userDataCommand = new SqlCommand(query, conn))
                        {
                            userDataCommand.Parameters.AddWithValue("@user_id", userID);

                            using (SqlDataReader reader = await userDataCommand.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    // Set the current user data
                                    CurrentUser.Instance.userID = userID;
                                    CurrentUser.Instance.firstName = (string)reader["first_name"];
                                    CurrentUser.Instance.middleName = (string)reader["middle_name"];
                                    CurrentUser.Instance.lastName = (string)reader["last_name"];
                                    CurrentUser.Instance.RoleId = (int)reader["role_id"];
                                }
                            }
                        }

                        // Retrieve role name
                        string roleNameQuery = "SELECT role_name FROM tbl_roles WHERE role_id = @role_id";
                        using (SqlCommand roleNameCommand = new SqlCommand(roleNameQuery, conn))
                        {
                            roleNameCommand.Parameters.AddWithValue("@role_id", CurrentUser.Instance.RoleId);

                            using (SqlDataReader roleNameReader = await roleNameCommand.ExecuteReaderAsync())
                            {
                                if (await roleNameReader.ReadAsync())
                                {
                                    CurrentUser.Instance.RoleName = (string)roleNameReader["role_name"];
                                }
                            }
                        }

                        // Retrieve module access list
                        string moduleAccessQuery = "SELECT module_name FROM tbl_module_access WHERE role_id = @role_id";
                        using (SqlCommand moduleAccessCommand = new SqlCommand(moduleAccessQuery, conn))
                        {
                            moduleAccessCommand.Parameters.AddWithValue("@role_id", CurrentUser.Instance.RoleId);

                            using (SqlDataReader moduleAccessReader = await moduleAccessCommand.ExecuteReaderAsync())
                            {
                                CurrentUser.Instance.ModuleAccessList = new List<string>();
                                while (await moduleAccessReader.ReadAsync())
                                {
                                    CurrentUser.Instance.ModuleAccessList.Add((string)moduleAccessReader["module_name"]);
                                }
                            }
                        }

                        return true;
                    }
                }

                return false;
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

        public async Task <List<User>?> GetUsers()
        {
            List<User> users = new List<User>();

            using DatabaseConnection connection = new();


            using (SqlConnection? conn = await connection.OpenConnection())
            {
                if (conn != null)
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT u.user_id, u.last_name, u.first_name " +
                                                           "FROM tbl_users u " +
                                                           "LEFT JOIN tbl_wage a ON u.user_id = a.user_id " +
                                                           "WHERE u.username IS NOT NULL " +
                                                           "AND u.username <> ''" +
                                                           "AND a.user_id IS NOT NULL", conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                User user = new User();
                                user.userID = reader.GetInt32(reader.GetOrdinal("user_id"));
                                string? firstName = reader["first_name"].ToString();
                                string? lastName = reader["last_name"].ToString();
                                user.name = $"{firstName} {lastName}";

                                users.Add(user);
                            }
                        }
                    }
                }
            }
            return users;


            try
            {
                
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<DataTable> GetPayrollData()
        {

            using DatabaseConnection? conn = new();

            using (SqlConnection? connection = await conn.OpenConnection())
            {
                DataTable payrollData = new DataTable();

                try
                {

                    // Get user IDs and names from tbl_users
                    SqlCommand userCommand = new SqlCommand("SELECT u.user_id, u.last_name, u.first_name, u.middle_name " +
                                                            "FROM tbl_users u " +
                                                            "LEFT JOIN tbl_wage a ON u.user_id = a.user_id " +
                                                            "WHERE u.username IS NOT NULL " +
                                                            "AND u.username <> '' " +
                                                            "AND u.password IS NOT NULL " +
                                                            "AND u.password <> '' " +
                                                            "AND a.user_id IS NOT NULL", connection);
                    SqlDataAdapter userAdapter = new SqlDataAdapter(userCommand);
                    DataTable userTable = new DataTable();
                    userAdapter.Fill(userTable);

                    // Calculate hours worked for each user
                    SqlCommand hoursCommand = new SqlCommand("SELECT user_id, SUM(hours_worked) AS hours_worked FROM tbl_work_hours WHERE issued = 0 GROUP BY user_id", connection);
                    SqlDataAdapter hoursAdapter = new SqlDataAdapter(hoursCommand);
                    DataTable hoursTable = new DataTable();
                    hoursAdapter.Fill(hoursTable);

                    // Calculate commissions for each user

                    SqlCommand commisionCommand = new SqlCommand("SELECT user_id, SUM(commission_amount) AS total_commision FROM tbl_commissions WHERE issued = 0 GROUP BY user_id", connection);
                    SqlDataAdapter commisionAdapter = new SqlDataAdapter(commisionCommand);
                    DataTable commisionTable = new DataTable();
                    commisionAdapter.Fill(commisionTable);

                    // Calculate commissions for each user
                    SqlCommand incentiveCommand = new SqlCommand("SELECT user_id, SUM(total_incentive) AS incentives FROM tbl_incentives WHERE issued = 0 AND is_valid = 1 GROUP BY user_id", connection);
                    SqlDataAdapter incentiveAdapter = new SqlDataAdapter(incentiveCommand);
                    DataTable incentiveTable = new DataTable();
                    incentiveAdapter.Fill(incentiveTable);

                    // Calculate overtime for each user
                    SqlCommand overtimeCommand = new SqlCommand("SELECT user_id, SUM(overtime) AS total_overtime FROM tbl_overtime WHERE issued = 0 GROUP BY user_id", connection);
                    SqlDataAdapter overtimeAdapter = new SqlDataAdapter(overtimeCommand);
                    DataTable overtimeTable = new DataTable();
                    overtimeAdapter.Fill(overtimeTable);

                    // Calculate reimbursement for each user
                    SqlCommand reimbursementCommand = new SqlCommand("SELECT user_id, SUM(amount) AS total_reimbursement FROM tbl_reimbursement WHERE issued = 0 GROUP BY user_id", connection);
                    SqlDataAdapter reimbursementAdapter = new SqlDataAdapter(reimbursementCommand);
                    DataTable reimbursementTable = new DataTable();
                    reimbursementAdapter.Fill(reimbursementTable);

                    // Calculate deductions for each user
                    SqlCommand deductionCommand = new SqlCommand("SELECT user_id, SUM(amount) AS total_deductions FROM tbl_deductions WHERE issued = 0 GROUP BY user_id", connection);
                    SqlDataAdapter deductionAdapter = new SqlDataAdapter(deductionCommand);
                    DataTable deductionTable = new DataTable();
                    deductionAdapter.Fill(deductionTable);

                    // Merge user, hours, and commission tables to get final payroll data
                    payrollData.Columns.Add("user_id");
                    payrollData.Columns.Add("name");
                    payrollData.Columns.Add("hours_worked");
                    payrollData.Columns.Add("additional_earnings");
                    payrollData.Columns.Add("commission");
                    payrollData.Columns.Add("incentives");
                    payrollData.Columns.Add("gross_pay");
                    payrollData.Columns.Add("overtime");
                    payrollData.Columns.Add("regular_pay");
                    payrollData.Columns.Add("overtime_pay");
                    payrollData.Columns.Add("reimbursement");
                    payrollData.Columns.Add("deductions");

                    foreach (DataRow userRow in userTable.Rows)
                    {
                        string userId = userRow["user_id"].ToString();
                        string lastName = userRow["last_name"].ToString();
                        string firstName = userRow["first_name"].ToString();
                        string middleName = userRow["middle_name"].ToString();

                        // Construct name string
                        string name = lastName + ", " + firstName;

                        if (middleName != "N/A" && middleName.Length > 0)
                        {
                            name += " " + middleName.Substring(0, 1) + ".";
                        }

                        // Get hours worked for user
                        DataRow[] hoursRows = hoursTable.Select("user_id = '" + userId + "'");
                        double hoursWorked = 0.0;

                        if (hoursRows.Length > 0 && hoursRows[0]["hours_worked"] != DBNull.Value)
                        {
                            hoursWorked = Convert.ToDouble(hoursRows[0]["hours_worked"]);
                        }
                        else
                        {
                            hoursWorked = 0.0;
                        }

                        DataRow[] overtimeRows = overtimeTable.Select("user_id = '" + userId + "'");
                        double overtime = 0.0;

                        if (overtimeRows.Length > 0)
                        {
                            overtime = Convert.ToDouble(overtimeRows[0]["total_overtime"]);
                        }

                        // Get commission for user
                        DataRow[] commissionRows = commisionTable.Select("user_id = '" + userId + "'");
                        double commission = 0.0;

                        if (commissionRows.Length > 0)
                        {
                            commission = Convert.ToDouble(commissionRows[0]["total_commision"]);
                        }

                        // Get incentives for user
                        DataRow[] incentiveRows = incentiveTable.Select("user_id = '" + userId + "'");
                        double incentive = 0.0;

                        if (incentiveRows.Length > 0)
                        {
                            incentive = Convert.ToDouble(incentiveRows[0]["incentives"]);
                        }

                        DataRow[] reimbursementnRows = reimbursementTable.Select("user_id = '" + userId + "'");
                        double reimbursement = 0.0;

                        if (reimbursementnRows.Length > 0)
                        {
                            reimbursement = Convert.ToDouble(reimbursementnRows[0]["total_reimbursement"]);
                        }

                        DataRow[] deductionRows = deductionTable.Select("user_id = '" + userId + "'");
                        double deduction = 0.0;

                        if (deductionRows.Length > 0)
                        {
                            deduction = Convert.ToDouble(deductionRows[0]["total_deductions"]);
                        }

                        // Calculate gross pay for user
                        double hourlyRate = 0.0;
                        SqlCommand wageCommand = new SqlCommand("SELECT hourly_rate FROM tbl_wage WHERE user_id = '" + userId + "'", connection);
                        object hourlyRateObj = wageCommand.ExecuteScalar();

                        if (hourlyRateObj != null && !Convert.IsDBNull(hourlyRateObj))
                        {
                            hourlyRate = Convert.ToDouble(hourlyRateObj);
                        }

                        double regularPay = (hoursWorked - overtime) * hourlyRate;
                        double overtimePay = overtime * 80;
                        double totalIncentive = incentive + commission;
                        double grossPay = ((hoursWorked - overtime) * hourlyRate) + totalIncentive + reimbursement + (overtime * 80) - deduction;
                        //double grossPay = hourlyRate * hoursWorked + commission + (overtime * hourlyRate) + reimbursement;

                        // Add data row to payroll data table
                        DataRow payrollRow = payrollData.NewRow();
                        payrollRow["user_id"] = userId;
                        payrollRow["name"] = name;
                        payrollRow["hours_worked"] = hoursWorked;
                        payrollRow["commission"] = 0;
                        payrollRow["gross_pay"] = string.Format("{0:N2}", grossPay);
                        payrollRow["additional_earnings"] = string.Format("{0:N2}", totalIncentive);
                        payrollRow["regular_pay"] = string.Format("{0:N2}", regularPay);
                        payrollRow["overtime_pay"] = string.Format("{0:N2}", overtimePay);
                        payrollRow["overtime"] = overtime;
                        payrollRow["reimbursement"] = string.Format("{0:N2}", reimbursement);
                        payrollRow["deductions"] = string.Format("{0:N2}", deduction);
                        payrollData.Rows.Add(payrollRow);
                    }
                }

                catch (SqlException ex)
                {
                    MessageBox.Show("Error retrieving payroll data: " + ex.Message);
                }

                return payrollData;
            }
        }

        public async void UpdateWorkHoursAndActiveUsers(int? userId, DateTime endTime)
        {
            try
            {
                using DatabaseConnection? conn = new();

                using (SqlConnection? connection = await conn.OpenConnection())
                {
                    string query = @"
                UPDATE tbl_work_hours
                SET end_time = @endTime, hours_worked = DATEDIFF(minute, start_time, @endTime) / 60.0
                WHERE user_id = @userId AND end_time IS NULL;
            
                UPDATE tbl_active_users
                SET logout_time = @endTime
                WHERE user_id = @userId AND logout_time IS NULL;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        command.Parameters.AddWithValue("@endTime", endTime);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }

        public async void InsertOrUpdateWorkHoursAndActiveUsers(int? userId, DateTime loginTime)
        {
            using DatabaseConnection? conn = new();

            using (SqlConnection? connection = await conn.OpenConnection())
            {
                string query = @"
                        INSERT INTO tbl_work_hours (user_id, start_time)
                        SELECT @userId, @loginTime
                        WHERE NOT EXISTS (
                            SELECT 1 FROM tbl_work_hours WHERE user_id = @userId AND end_time IS NULL
                        );

                        MERGE tbl_active_users AS target
                        USING (VALUES (@userId, @loginTime)) AS source(user_id, login_time)
                        ON target.user_id = source.user_id
                        WHEN MATCHED THEN
                            UPDATE SET target.login_time = source.login_time, target.logout_time = NULL
                        WHEN NOT MATCHED THEN
                        INSERT (user_id, login_time)
                        VALUES (source.user_id, source.login_time);";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@loginTime", loginTime);

                    command.ExecuteNonQuery();
                }
            }
        }

        public async Task<Dictionary<string, double>> GetReimbursementsForUser(string userId)
        {
            Dictionary<string, double> remimbursementData = new Dictionary<string, double>();
            using DatabaseConnection? conn = new();

            using (SqlConnection? connection = await conn.OpenConnection())
            {
                string query = "SELECT description, amount FROM tbl_reimbursement WHERE user_id = @UserId AND issued = 0";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string description = reader["description"].ToString();
                            double amount = Convert.ToDouble(reader["amount"]);

                            if (remimbursementData.ContainsKey(description))
                            {
                                remimbursementData[description] += amount;
                            }
                            else
                            {
                                remimbursementData.Add(description, amount);
                            }
                        }
                    }
                }
            }

            return remimbursementData;
        }

        public async Task <Dictionary<string, double>> GetCommissionDataForUser(string userId)
        {
            Dictionary<string, double> commissionData = new Dictionary<string, double>();

            using DatabaseConnection? conn = new();

            using (SqlConnection? connection = await conn.OpenConnection())
            {
                string query = "SELECT commission_name, commission_amount FROM tbl_commissions WHERE user_id = @UserId AND issued = 0";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string commissionType = reader["commission_name"].ToString();
                            double commissionAmount = Convert.ToDouble(reader["commission_amount"]);

                            if (commissionData.ContainsKey(commissionType))
                            {
                                commissionData[commissionType] += commissionAmount;
                            }
                            else
                            {
                                commissionData.Add(commissionType, commissionAmount);
                            }
                        }
                    }
                }
            }

            return commissionData;
        }

        public async Task<DataTable> GetUsersDataTable(string condition)
        {
            string query = $@"SELECT u.user_id, u.first_name, u.middle_name, u.last_name, u.email, u.username, u.contact_number, u.status, r.role_name
                  FROM tbl_users u
                  INNER JOIN tbl_active_users au ON u.user_id = au.user_id
                  LEFT JOIN tbl_wage w ON u.user_id = w.user_id  
                  LEFT JOIN tbl_access_level a ON u.user_id = a.user_id
                  LEFT JOIN tbl_roles r ON a.role_id = r.role_id
                  WHERE {condition} AND w.user_id IS NOT NULL
                  GROUP BY u.user_id, u.first_name, u.middle_name, u.last_name, u.email, u.username, u.contact_number, u.status, r.role_name";

            using DatabaseConnection? conn = new();

            using (SqlConnection? connection = await conn.OpenConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        try
                        {
                            DataTable dataTable = new DataTable();
                            dataTable.Load(reader);
                            return dataTable;
                        }
                        catch (Exception ex)
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public async Task<(int active, int inactive, int disabled)> GetUserCounts()
        {
            int activeCount = 0;
            int inactiveCount = 0;
            int disabledCount = 0;

            using DatabaseConnection? conn = new();

            using (SqlConnection? connection = await conn.OpenConnection())
            {
                string activeQuery = @"SELECT COUNT(*)
                      FROM tbl_active_users au
                      INNER JOIN tbl_users u ON au.user_id = u.user_id
                      INNER JOIN tbl_wage w ON u.user_id = w.user_id
                      WHERE u.status = 'Enabled'
                      AND au.login_time IS NOT NULL
                      AND au.logout_time IS NULL
                      AND u.user_id IS NOT NULL";

                SqlCommand activeCommand = new SqlCommand(activeQuery, connection);
                activeCount = (int)activeCommand.ExecuteScalar();

                string inactiveQuery = @"SELECT COUNT(*)
                        FROM tbl_active_users au
                        INNER JOIN tbl_users u ON au.user_id = u.user_id
                        INNER JOIN tbl_wage w ON u.user_id = w.user_id
                        WHERE u.status = 'Enabled'
                        AND ((au.login_time IS NOT NULL AND au.logout_time IS NOT NULL)
                        OR (au.login_time IS NULL AND au.logout_time IS NULL))";
                SqlCommand inactiveCommand = new SqlCommand(inactiveQuery, connection);
                inactiveCount = (int)inactiveCommand.ExecuteScalar();

                string disabledQuery = "SELECT COUNT(*) FROM tbl_users WHERE status = 'Disabled'";
                SqlCommand disabledCommand = new SqlCommand(disabledQuery, connection);
                disabledCount = (int)disabledCommand.ExecuteScalar();
            }

            return (activeCount, inactiveCount, disabledCount);
        }

        public async Task<(int discontinued, int lowStock, int outOfStock)> GetProductsCount()
        {
            int discontinuedCount = 0;
            int lowStockCount = 0;
            int outOfStockCount = 0;

            using DatabaseConnection? conn = new();

            using (SqlConnection? connection = await conn.OpenConnection())
            {
                string discontinuedQuery = "SELECT COUNT(*) FROM tbl_products WHERE status = 'DISCONTINUED'";
                SqlCommand discontinuedCommand = new SqlCommand(discontinuedQuery, connection);
                discontinuedCount = (int)discontinuedCommand.ExecuteScalar();

                string lowStockQuery = "SELECT COUNT(*) FROM tbl_products WHERE status = 'LOW-STOCK'";
                SqlCommand lowStockCommand = new SqlCommand(lowStockQuery, connection);
                lowStockCount = (int)lowStockCommand.ExecuteScalar();

                string outOfStockQuery = "SELECT COUNT(*) FROM tbl_products WHERE status = 'OUT OF STOCK'";
                SqlCommand outOfStockCommand = new SqlCommand(outOfStockQuery, connection);
                outOfStockCount = (int)outOfStockCommand.ExecuteScalar();
            }

            return (discontinuedCount, lowStockCount, outOfStockCount);
        }

        public async Task  <bool> CheckIfLogged(string tableName, string columnId, string columnIdValue, string columnValue)
        {
            using DatabaseConnection? conn = new();

            using (SqlConnection? connection = await conn.OpenConnection())
            {
                string query = $"SELECT COUNT(*) FROM {tableName} WHERE {columnId} = @{columnId} AND {columnValue} IS NULL";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue($"@{columnId}", columnIdValue);

                int count = (int)command.ExecuteScalar();

                return count > 0;
            }
        }

        public async Task <bool> CheckIfExists(string tableName, string columnId, string columnIdValue, string columnValue, string columnValueToCheck)
        {
            using DatabaseConnection conn = new();

            using (SqlConnection? connection = await conn.OpenConnection())
            {
                string query = $"SELECT COUNT(*) FROM {tableName} WHERE {columnId} = @{columnId} AND {columnValue} = @{columnValue}";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue($"@{columnId}", columnIdValue);
                command.Parameters.AddWithValue($"@{columnValue}", columnValueToCheck);

                int count = (int)command.ExecuteScalar();

                return count > 0;
            }
        }

        public async Task<bool> CheckRoleExistsInAccessLevel(string roleId)
        {
            using (DatabaseConnection dbConnection = new DatabaseConnection())
            {
                SqlConnection? connection = await dbConnection.OpenConnection();

                if (connection != null)
                {
                    SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM tbl_access_level WHERE role_id = @roleId;", connection);
                    command.Parameters.AddWithValue("@roleId", roleId);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    return count > 0;
                }

                CloseConnection();
            }

            return false;
        }

        public async Task <List<Dictionary<string, object>>> GetUserFinancialData(int userId)
        {
            List<Dictionary<string, object>> financialData = new List<Dictionary<string, object>>();

            using DatabaseConnection conn = new();

            using (SqlConnection? connection = await conn.OpenConnection())
            {
                string query = @"SELECT r.role_name
                FROM tbl_users u
                JOIN tbl_access_level al ON u.user_id = al.user_id
                JOIN tbl_roles r ON al.role_id = r.role_id
                WHERE u.user_id = @userId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Dictionary<string, object> accessLevelData = new Dictionary<string, object>();
                    accessLevelData.Add("role_name", reader["role_name"]);
                    financialData.Add(accessLevelData);
                }
                reader.Close();

                // Get hourly rate
                query = "SELECT hourly_rate FROM tbl_wage WHERE user_id = @userId";
                command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);

                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Dictionary<string, object> hourlyRateData = new Dictionary<string, object>();
                    hourlyRateData.Add("hourly_rate", reader["hourly_rate"]);
                    financialData.Add(hourlyRateData);
                }
                reader.Close();


                // Get sum of working hours
                query = "SELECT SUM(hours_worked) AS hours_worked_sum FROM tbl_work_hours WHERE issued = 0 AND user_id = @userId";
                command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);

                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Dictionary<string, object> workingHoursData = new Dictionary<string, object>();
                    workingHoursData.Add("hours_worked", reader["hours_worked_sum"]);
                    financialData.Add(workingHoursData);
                }
                reader.Close();

                // Get sum of overtime
                query = "SELECT SUM(overtime) AS overtime_sum FROM tbl_overtime WHERE issued = 0 AND user_id = @userId";
                command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);

                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Dictionary<string, object> overtimeData = new Dictionary<string, object>();
                    overtimeData.Add("overtime", reader["overtime_sum"]);
                    financialData.Add(overtimeData);
                }
                reader.Close();

                // Get sum of commissions
                query = "SELECT SUM(commission_amount) AS commission_sum FROM tbl_commissions WHERE issued = 0 AND user_id = @userId";
                command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);

                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Dictionary<string, object> commissionData = new Dictionary<string, object>();
                    commissionData.Add("commission", reader["commission_sum"]);
                    financialData.Add(commissionData);
                }
                reader.Close();

                // Get sum of incentives
                query = "SELECT SUM(total_incentive) AS incentive_sum FROM tbl_incentives WHERE issued = 0 AND is_valid = 1 AND user_id = @userId";
                command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);

                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Dictionary<string, object> incentiveData = new Dictionary<string, object>();
                    incentiveData.Add("incentive", reader["incentive_sum"]);
                    financialData.Add(incentiveData);
                }
                reader.Close();

                // Get sum of reimbursements
                query = "SELECT SUM(amount) AS reimbursement_sum FROM tbl_reimbursement WHERE issued = 0 AND user_id = @userId";
                command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);

                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Dictionary<string, object> reimbursementData = new Dictionary<string, object>();
                    reimbursementData.Add("reimbursement", reader["reimbursement_sum"]);
                    financialData.Add(reimbursementData);
                }
                reader.Close();

                query = "SELECT SUM(amount) AS deductions_sum FROM tbl_deductions WHERE issued = 0 AND user_id = @userId";
                command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);

                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Dictionary<string, object> deductionsData = new Dictionary<string, object>();
                    deductionsData.Add("deductions", reader["deductions_sum"]);
                    financialData.Add(deductionsData);
                }
                reader.Close();
            }

            return financialData;
        }

        public async Task<List<Roles>> GetRoles()
        {
            List<Roles> roles = new List<Roles>();


            using DatabaseConnection conn = new();

            using (SqlConnection? connection = await conn.OpenConnection())
            {
                {
                    string query = "SELECT role_id, role_name, hourly_rate FROM tbl_roles WHERE role_id != 1";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            Roles role = new Roles();
                            role.roleID = reader.GetInt32(0);
                            role.roleName = reader.GetString(1);
                            decimal? hourlyRateNullable = reader.IsDBNull(2) ? (decimal?)null : reader.GetDecimal(2);
                            role.hourlyRate = hourlyRateNullable ?? 0;

                            roles.Add(role);
                        }

                        reader.Close();
                    }
                }

                return roles;
            }
        }

        public async Task<bool> InsertOrUpdateRole(string roleName, decimal hourlyRate, List<string> moduleAccessList, string id = null)
        {
            using (DatabaseConnection conn = new DatabaseConnection())
            {
                try
                {
                    using (SqlConnection connection = await conn.OpenConnection())
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        SqlCommand command = connection.CreateCommand();
                        command.Transaction = transaction;

                        // Check if roleName already exists
                        string checkRoleQuery = "SELECT COUNT(*) FROM tbl_roles WHERE role_name = @roleName";
                        Dictionary<string, object> checkRoleParams = new Dictionary<string, object>
                {
                    { "@roleName", roleName }
                };

                        int roleCount = await conn.ExecuteScalar<int>(checkRoleQuery, checkRoleParams, transaction);

                        if (roleCount > 0 && string.IsNullOrEmpty(id))
                        {
                            MessageBox.Show("Role name already exists.");
                            return false;
                        }

                        if (string.IsNullOrEmpty(id))
                        {
                            // Insert into tbl_roles
                            string insertRoleQuery = "INSERT INTO tbl_roles (role_name, hourly_rate) OUTPUT INSERTED.role_id VALUES (@roleName, @hourlyRate)";
                            Dictionary<string, object> roleParams = new Dictionary<string, object>
                    {
                        { "@roleName", roleName },
                        { "@hourlyRate", hourlyRate }
                    };

                            command.CommandText = insertRoleQuery;
                            command.Parameters.Clear();
                            foreach (var param in roleParams)
                            {
                                command.Parameters.AddWithValue(param.Key, param.Value);
                            }

                            int roleId = Convert.ToInt32(await command.ExecuteScalarAsync());

                            // Insert into tbl_module_access
                            string insertModuleAccessQuery = "INSERT INTO tbl_module_access (role_id, module_name) VALUES (@roleId, @moduleName)";
                            foreach (string moduleName in moduleAccessList)
                            {
                                Dictionary<string, object> moduleAccessParams = new Dictionary<string, object>
                        {
                            { "@roleId", roleId },
                            { "@moduleName", moduleName }
                        };

                                command.CommandText = insertModuleAccessQuery;
                                command.Parameters.Clear();
                                foreach (var param in moduleAccessParams)
                                {
                                    command.Parameters.AddWithValue(param.Key, param.Value);
                                }
                                await command.ExecuteNonQueryAsync();
                            }
                        }
                        else
                        {
                            // Update tbl_roles
                            string updateRoleQuery = "UPDATE tbl_roles SET role_name = @roleName, hourly_rate = @hourlyRate WHERE role_id = @roleId";
                            Dictionary<string, object> roleParams = new Dictionary<string, object>
                    {
                        { "@roleName", roleName },
                        { "@hourlyRate", hourlyRate },
                        { "@roleId", id }
                    };

                            command.CommandText = updateRoleQuery;
                            command.Parameters.Clear();
                            foreach (var param in roleParams)
                            {
                                command.Parameters.AddWithValue(param.Key, param.Value);
                            }

                            await command.ExecuteNonQueryAsync();

                            // Delete existing module access for the role
                            string deleteModuleAccessQuery = "DELETE FROM tbl_module_access WHERE role_id = @roleId";
                            Dictionary<string, object> deleteModuleAccessParams = new Dictionary<string, object>
                    {
                        { "@roleId", id }
                    };

                            command.CommandText = deleteModuleAccessQuery;
                            command.Parameters.Clear();
                            foreach (var param in deleteModuleAccessParams)
                            {
                                command.Parameters.AddWithValue(param.Key, param.Value);
                            }

                            await command.ExecuteNonQueryAsync();

                            // Insert new module access for the role
                            string insertModuleAccessQuery = "INSERT INTO tbl_module_access (role_id, module_name) VALUES (@roleId, @moduleName)";
                            foreach (string moduleName in moduleAccessList)
                            {
                                Dictionary<string, object> moduleAccessParams = new Dictionary<string, object>
                        {
                            { "@roleId", id },
                            { "@moduleName", moduleName }
                        };

                                command.CommandText = insertModuleAccessQuery;
                                command.Parameters.Clear();
                                foreach (var param in moduleAccessParams)
                                {
                                    command.Parameters.AddWithValue(param.Key, param.Value);
                                }
                                await command.ExecuteNonQueryAsync();
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error inserting/updating role: " + ex.Message);
                    return false;
                }
            }
        }

        public async Task<Roles> GetRole(string roleId)
        {
            using (DatabaseConnection dbConnection = new DatabaseConnection())
            {
                SqlConnection connection = await dbConnection.OpenConnection();

                if (connection != null && connection.State == ConnectionState.Open)
                {
                    string query = "SELECT * FROM tbl_roles WHERE role_id = @RoleId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@RoleId", roleId);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            await reader.ReadAsync();
                            Roles role = new Roles
                            {
                                roleName = reader["role_name"].ToString(),
                                hourlyRate = reader.IsDBNull(reader.GetOrdinal("hourly_rate")) ? 0 : Convert.ToDecimal(reader["hourly_rate"])
                            };

                            return role;
                        }
                    }
                }
            }

            return null;
        }

        public async Task<List<string>> GetModuleAccess(string roleId)
        {
            List<string> moduleAccess = new List<string>();

            using (DatabaseConnection dbConnection = new DatabaseConnection())
            {
                SqlConnection connection = await dbConnection.OpenConnection();

                if (connection != null && connection.State == ConnectionState.Open)
                {
                    string query = "SELECT module_name FROM tbl_module_access WHERE role_id = @RoleId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@RoleId", roleId);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            moduleAccess.Add(reader["module_name"].ToString());
                        }
                    }
                }
            }

            return moduleAccess;
        }

        public async void DeleteInvalidRecords()
        {
            using (DatabaseConnection dbConnection = new DatabaseConnection())
            {
                SqlConnection? connection = await dbConnection.OpenConnection();

                if(connection != null)
                {
                    SqlCommand deleteCommand = new SqlCommand("DELETE FROM tbl_commissions WHERE is_valid = 0; DELETE FROM tbl_overtime WHERE is_valid = 0; DELETE FROM tbl_reimbursement WHERE is_valid = 0; DELETE FROM tbl_deductions WHERE is_valid = 0;", connection);

                    deleteCommand.ExecuteNonQuery();
                }
                
                CloseConnection();
            }  
        }

        public async Task<bool> DeleteRole(string roleId)
        {
            using (DatabaseConnection dbConnection = new DatabaseConnection())
            {
                SqlConnection? connection = await dbConnection.OpenConnection();

                if (connection != null)
                {
                    SqlTransaction? transaction = null;

                    try
                    {
                        // Start a new transaction
                        transaction = connection.BeginTransaction();

                        // Delete records from tbl_module_access
                        SqlCommand deleteModuleAccessCommand = new SqlCommand("DELETE FROM tbl_module_access WHERE role_id = @roleId;", connection, transaction);
                        deleteModuleAccessCommand.Parameters.AddWithValue("@roleId", roleId);
                        deleteModuleAccessCommand.ExecuteNonQuery();

                        // Update tbl_access_level to set role_id to null
                        SqlCommand updateAccessLevelCommand = new SqlCommand("UPDATE tbl_access_level SET role_id = NULL WHERE role_id = @roleId;", connection, transaction);
                        updateAccessLevelCommand.Parameters.AddWithValue("@roleId", roleId);
                        updateAccessLevelCommand.ExecuteNonQuery();

                        // Delete record from tbl_roles
                        SqlCommand deleteRoleCommand = new SqlCommand("DELETE FROM tbl_roles WHERE role_id = @roleId;", connection, transaction);
                        deleteRoleCommand.Parameters.AddWithValue("@roleId", roleId);
                        deleteRoleCommand.ExecuteNonQuery();

                        // Commit the transaction if all queries succeeded
                        transaction.Commit();

                        return true;
                    }
                    catch (Exception)
                    {
                        // Rollback the transaction if any error occurs
                        transaction?.Rollback();
                        return false;
                    }
                    finally
                    {
                        // Ensure the transaction and connection are closed
                        transaction?.Dispose();
                    }
                }

                CloseConnection();
            }

            return false;
        }

        public async Task <bool> HasInvalidPayrollRecords()
        {
            bool hasInvalidRecords = false;

            using (DatabaseConnection dbConnection = new DatabaseConnection())
            {
                SqlConnection? connection = await dbConnection.OpenConnection();

                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM tbl_commissions WHERE is_valid = 0 UNION ALL SELECT COUNT(*) FROM tbl_overtime WHERE is_valid = 0 UNION ALL SELECT COUNT(*) FROM tbl_reimbursement WHERE is_valid = 0;", connection);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int count = (int)reader[0];

                    if (count > 0)
                    {
                        hasInvalidRecords = true;
                        break;
                    }
                }

                reader.Close();
                CloseConnection();
            }


            return hasInvalidRecords;
        }

        public async void IssuePayslip(int userId)
        {
            using (DatabaseConnection dbConnection = new DatabaseConnection())
            {
                SqlConnection? connection = await dbConnection.OpenConnection();

                if (connection != null)
                {
                    // Update tbl_commissions
                    SqlCommand updateCommissionsCommand = new SqlCommand("UPDATE tbl_commissions SET issued = 1 WHERE user_id = @userId", connection);
                    updateCommissionsCommand.Parameters.AddWithValue("@userId", userId);
                    updateCommissionsCommand.ExecuteNonQuery();

                    // Update tbl_overtime
                    SqlCommand updateOvertimeCommand = new SqlCommand("UPDATE tbl_overtime SET issued = 1 WHERE user_id = @userId", connection);
                    updateOvertimeCommand.Parameters.AddWithValue("@userId", userId);
                    updateOvertimeCommand.ExecuteNonQuery();

                    // Update tbl_reimbursement
                    SqlCommand updateReimbursementCommand = new SqlCommand("UPDATE tbl_reimbursement SET issued = 1 WHERE user_id = @userId", connection);
                    updateReimbursementCommand.Parameters.AddWithValue("@userId", userId);
                    updateReimbursementCommand.ExecuteNonQuery();

                    // Update tbl_reimbursement
                    SqlCommand updateIncentiveCommand = new SqlCommand("UPDATE tbl_incentives SET issued = 1 WHERE user_id = @userId", connection);
                    updateIncentiveCommand.Parameters.AddWithValue("@userId", userId);
                    updateIncentiveCommand.ExecuteNonQuery();

                    // Update tbl_deductions
                    SqlCommand updateDeductionsCommand = new SqlCommand("UPDATE tbl_deductions SET issued = 1 WHERE user_id = @userId", connection);
                    updateDeductionsCommand.Parameters.AddWithValue("@userId", userId);
                    updateDeductionsCommand.ExecuteNonQuery();

                    // Update tbl_deductions
                    SqlCommand updateWorkHoursCommand = new SqlCommand("UPDATE tbl_work_Hours SET issued = 1 WHERE user_id = @userId", connection);
                    updateWorkHoursCommand.Parameters.AddWithValue("@userId", userId);
                    updateWorkHoursCommand.ExecuteNonQuery();
                }

                CloseConnection();
            }
        }

        public async Task <bool> InsertOrder(string[] receiverDataValues, string[] orderValues)
        {
            bool success = false;

            DatabaseConnection dbConnection = new DatabaseConnection();

            using (SqlConnection connection = await dbConnection.OpenConnection())
            {
                SqlTransaction transaction = null;

                try
                {
                    transaction = connection.BeginTransaction();

                    // Retrieve the newly inserted sender and receiver IDs
                    int receiverID;
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.Transaction = transaction;
                        command.CommandText = "INSERT INTO tbl_receiver (receiver_name, receiver_phone, receiver_address) " +
                                              "VALUES (@ReceiverName, @ReceiverPhone, @ReceiverAddress);" +
                                              "SELECT SCOPE_IDENTITY() AS ReceiverID";

                        command.Parameters.AddWithValue("@ReceiverName", receiverDataValues[0]);
                        command.Parameters.AddWithValue("@ReceiverPhone", receiverDataValues[1]);
                        command.Parameters.AddWithValue("@ReceiverAddress", receiverDataValues[2]);

                        receiverID = Convert.ToInt32(command.ExecuteScalar());
                        if (receiverID <= 0)
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }

                    // Insert into tbl_orders with sender and receiver IDs and status as "pending"
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.Transaction = transaction;
                        command.CommandText = "INSERT INTO tbl_orders (order_id, courier, user_id, receiver_id, product_id, quantity, total, remarks, status) " +
                                              "VALUES (@OrderID, @Courier, @UserID,  @ReceiverID, @ProductID, @Quantity, @Total, @Remarks, @Status )";

                        command.Parameters.AddWithValue("@OrderID", orderValues[0]);
                        command.Parameters.AddWithValue("@Courier", orderValues[1]);
                        command.Parameters.AddWithValue("@UserID", orderValues[2]);
                        command.Parameters.AddWithValue("@ReceiverID", receiverID);
                        command.Parameters.AddWithValue("@ProductID", orderValues[3]);
                        command.Parameters.AddWithValue("@Quantity", orderValues[4]);
                        command.Parameters.AddWithValue("@Total", orderValues[5]);
                        command.Parameters.AddWithValue("@Remarks", orderValues[6]);
                        command.Parameters.AddWithValue("@Status", "IN PROGRESS");
                       

                        command.ExecuteNonQuery();
                    }

                    // Update tbl_products with new quantity
                    int newQty = Convert.ToInt32(orderValues[4]);

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.Transaction = transaction;
                        command.CommandText = "UPDATE tbl_products SET unit_quantity = unit_quantity - @Quantity, status = @Status WHERE product_id = @ProductID";

                        command.Parameters.AddWithValue("@Quantity", newQty);
                        command.Parameters.AddWithValue("@Status", orderValues[7]);
                        command.Parameters.AddWithValue("@ProductID", orderValues[3]);

                        command.ExecuteNonQuery();
                    }


                    //insert tbl_incentive


                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.Transaction = transaction;

                        command.CommandText = "SELECT employee_commission FROM tbl_selling_expenses WHERE product_id = @ProductID";
                        command.Parameters.AddWithValue("@ProductID", orderValues[3]);
                        decimal employeeCommission = (decimal)command.ExecuteScalar();

                        // Calculate the totalIncentive by multiplying the employee_commission with the quantity
                        decimal quantity = Convert.ToDecimal(orderValues[4]);
                        decimal totalIncentive = employeeCommission * quantity;

                        // Insert into tbl_incentives
                        command.CommandText = "INSERT INTO tbl_incentives (user_id, incentive_for, quantity, total_incentive, is_valid) " +
                                              "VALUES (@UserID, @IncentiveFor, @Quantity,  @Total, @IsValid)";

                        command.Parameters.AddWithValue("@UserID", orderValues[2]);
                        command.Parameters.AddWithValue("@IncentiveFor", orderValues[3]);
                        command.Parameters.AddWithValue("@Quantity", quantity);
                        command.Parameters.AddWithValue("@Total", totalIncentive);
                        command.Parameters.AddWithValue("@IsValid", 1);

                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    success = true;
                }
                catch (SqlException ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
                finally
                {
                    CloseConnection();
                }
            }

            return success;
        }
    }
}

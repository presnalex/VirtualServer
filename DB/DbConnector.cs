using NLog;

namespace DB
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Data.SqlTypes;
    using System.Linq;


    /// <summary>
    /// Класс, выполняющий запросы к БД с указанными параметрами,
    /// и возвращающий результаты в виде списка словарей с именем поля в качестве ключа
    /// </summary>
    public sealed class DbConnector
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Строка подключения к БД
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        /// Creates instance of connector
        /// </summary>
        /// <param name="connString"></param>
        public DbConnector(string connString)
        {
            connectionString = connString;
        }

        /// <summary>
        /// Forbidden
        /// </summary>
        private DbConnector()
        {
        }

        /// <summary>
        /// Загрузка динамически типизированного значения из System.Object
        /// </summary>
        /// <typeparam name="T">Тип возвращаемого значения</typeparam>
        /// <param name="src">Значение</param>
        public static T GetValue<T>(object src)
        {
            var ttype = typeof(T);
            if (!Convert.IsDBNull(src))
            {
                if (ttype.IsGenericType && ttype.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    ttype = Nullable.GetUnderlyingType(ttype);
                }

                return (T)Convert.ChangeType(src, ttype);
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Запускает хранимую процедуру без возврата значений
        /// </summary>
        /// <param name="commandName">Имя хранимой процедуры</param>
        /// <param name="parameters">Параметры</param>
        /// <param name="timeout">Тайм-аут для команды в секундах (90 по-умолчанию)</param>
        /// <returns>Количество измененных/добавленных строк</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public int Execute(string commandName, QueryParams parameters, int timeout = 90)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(commandName, conn))
                {
                    cmd.CommandTimeout = timeout;
                    if (!IsRaw(commandName))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                    }

                    if (parameters != null)
                    {
                        try
                        {
                            foreach (var p in parameters.InParams)
                            {
                                cmd.Parameters.Add(p.AsSQL);
                            }
                        }
                        catch (SqlTypeException ex)
                        {
                            logger.Error(
                                "Failed to add one of the SQL parameters: " +
                                parameters.InParams.Aggregate(
                                    string.Empty,
                                    (str, x) => str += x.Name + ": " + x.Value.ToString() + ";"),
                                ex);
                            return -1;
                        }

                        logger.Info("Executing DB non-query command “{0}” with parameters {1}",
                            commandName,
                            parameters.InParams.Aggregate(
                                string.Empty,
                                (acc, x) => acc += string.Format("{0}: {1}; ",
                                x.Name, (x.Value ?? string.Empty).ToString().Replace("\n", " "))));
                    }
                    else
                    {
                        logger.Debug("Executing DB command “{0}” without parameters", commandName);
                    }

                    // Execute query
                    try
                    {
                        cmd.Connection.Open();
                        int n = cmd.ExecuteNonQuery();
                        if (parameters != null)
                        {
                            foreach (var item in parameters.InParams.Where(x => x.Direction != ParameterDirection.Input))
                            {
                                parameters.OutParams.Add(item.Name, cmd.Parameters[item.Name].Value);
                            }
                        }

                        return n;
                    }
                    catch (SqlException ex)
                    {
                        logger.Error("Stored procedure " + commandName + " returned an error with code " + ex.ErrorCode, ex);

                        // Try again if timeout, increased time to wait
                        if (ex.Number == -2 && timeout <= 210)
                        {
                            return Execute(commandName, parameters, timeout + timeout / 3);
                        }

                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Запускает указанную хранимую процедуру и получает строки со значениями
        /// </summary>
        /// <param name="commandName">Имя хранимой процедуры</param>
        /// <param name="parameters">Параметры для хранимой процедуры</param>
        /// <param name="logResult">Запись в лог полученных результатов</param>
        /// <param name="timeout">Тайм-аут команды</param>
        /// <returns>Список словарей с именем колонки таблицы в качестве ключа</returns>
        public List<Dictionary<string, object>> GetValues(
            string commandName,
            QueryParams parameters,
            bool logResult = false,
            byte timeout = 60)
        {
            var rows = new List<Dictionary<string, object>>();
            using (var conn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(commandName, conn))
                {
                    try
                    {
                        cmd.CommandTimeout = timeout;
                        if (!IsRaw(commandName))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                        }

                        if (parameters != null)
                        {
                            foreach (var p in parameters.InParams)
                            {
                                cmd.Parameters.Add(p.AsSQL);
                            }

                            logger.Info(
                                "Executing DB command “{0}” with parameters {1}",
                                commandName,
                                parameters.InParams.Aggregate(
                                    string.Empty,
                                    (acc, x) => acc += string.Format("{0}: {1};",
                                    x.Name,
                                    (x.Value ?? string.Empty).ToString().Replace("\n", " "))));
                        }
                        else
                        {
                            logger.Info("Executing DB command “{0}” without parameters", commandName);
                        }

                        cmd.Connection.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            // Get rows
                            while (reader.Read())
                            {
                                var entries = new Dictionary<string, object>(); // Словарь для значений в строке
                                object[] vals = new object[reader.FieldCount]; // Значения в строке
                                reader.GetValues(vals);
                                // Логгирование
                                if (logResult)
                                {
                                    logger.Info(
                                        "{1} got values: {0}",
                                        vals.Aggregate("", (acc, o) => acc += o.ToString() + "; "),
                                        commandName);
                                }

                                // Получение значений в строке
                                for (int i = 0; i < vals.Length; i++)
                                {
                                    entries.Add(reader.GetName(i), vals[i]);
                                }

                                rows.Add(entries);
                            }

                            if (logResult && !rows.Any())
                            {
                                logger.Debug(
                                    "No results returned from database by the command {0}",
                                    commandName);
                            }
                        }

                        if (parameters != null)
                        {
                            // Set output peremeters values
                            foreach (var item in parameters.InParams
                                .Where(x => x.Direction != ParameterDirection.Input))
                            {
                                parameters.OutParams.Add(item.Name, cmd.Parameters[item.Name].Value);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        logger.ErrorException("Database procedure " + commandName + " failed with error " + ex.Message, ex);

                        throw;
                    }
                }
            }

            return rows;
        }

        /// <summary>
        /// Check if the command is not a stored procedure
        /// </summary>
        /// <param name="commandName">Stored procedure name or command text</param>
        /// <returns>True if command is raw SQL expression</returns>
        private static bool IsRaw(string commandName)
        {
            return commandName.StartsWith("select", StringComparison.OrdinalIgnoreCase) ||
                commandName.StartsWith("insert", StringComparison.OrdinalIgnoreCase) ||
                commandName.StartsWith("update", StringComparison.OrdinalIgnoreCase);
        }
    }
}

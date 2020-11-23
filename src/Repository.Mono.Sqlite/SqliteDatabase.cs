using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Mono.Data.Sqlite;

namespace Repository.Mono.Sqlite
{
    public class SqLiteDatabase
    {
        readonly String databaseConnection;

        /// <summary>
        ///     Default Constructor for SQLiteDatabase Class.
        /// </summary>
        public SqLiteDatabase()
        {
            databaseConnection = "Data Source=database.s3db";
        }

        /// <summary>
        ///     Single Param Constructor for specifying the DB file.
        /// </summary>
        /// <param name="inputFile">The File containing the DB</param>
        public SqLiteDatabase(String inputFile)
        {
            //const string connectionString = "URI=file:SqliteTest1.db";
            databaseConnection = String.Format("Data Source={0}", inputFile);
            if (!new FileInfo(inputFile).Exists)
            {
                SqliteConnection.CreateFile(inputFile);
            }
        }



        /// <summary>
        ///     Single Param Constructor for specifying advanced connection options.
        /// </summary>
        /// <param name="connectionOpts">A dictionary containing all desired options and their values</param>
        public SqLiteDatabase(Dictionary<String, String> connectionOpts)
        {
            String str = "";
            foreach (KeyValuePair<String, String> row in connectionOpts)
            {
                str += String.Format("{0}={1}; ", row.Key, row.Value);
            }
            str = str.Trim().Substring(0, str.Length - 1);
            databaseConnection= str;
        }


        /// <summary>
        ///     Allows the programmer to run a query against the Database.
        /// </summary>
        /// <param name="sql">The SQL to run</param>
        /// <returns>A DataTable containing the result set.</returns>
        public DataTable GetDataTable(string sql)
        {
            return ExecuteQuery(sql);
        }

        public bool DoesTableExist(string name)
        {
            var dataRowCollection = ExecuteQuery("SELECT name FROM sqlite_master WHERE name='" + name + "'");
            return dataRowCollection != null && dataRowCollection.Rows.Count > 0;
        }

        private SqliteCommand CreateCommandFromString(SqliteConnection connection, string sql)
        {
            return new SqliteCommand(connection) { CommandText = sql };
        }

        private DataTable ExecuteQuery(string query)
        {
            try
            {
                var dataTable = new DataTable();
//                var colEvent = dataTable.Columns.Add("event", typeof(string));
//                var colStart = dataTable.Columns.Add("start", typeof(string));
//                var colDuration = dataTable.Columns.Add("duration", typeof(string));
                using (var connection = new SqliteConnection(databaseConnection))
                {
                    connection.Open();
                    using (var dataReader = CreateCommandFromString(connection, query).ExecuteReader())
                    {
                        dataTable.Load(dataReader);
//                        ConsoleWriter.Log("{0} contained {1} records ", databaseConnection, dataTable.Rows.Count);
                    }
                }
                return dataTable;
            }
            catch (SqliteException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        ///     Allows the programmer to interact with the database for purposes other than a query.
        /// </summary>
        /// <param name="sql">The SQL to be run.</param>
        /// <returns>An Integer containing the number of rows updated.</returns>

        public int ExecuteNonQuery(string sql)
        {
            try
            {
                int rowsUpdated = 0;
                using (SqliteConnection connection = new SqliteConnection(databaseConnection))
                {
                    connection.Open();
                    rowsUpdated = CreateCommandFromString(connection, sql).ExecuteNonQuery();
                    connection.Close();
                }

                return rowsUpdated;
            }
            catch (SqliteException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        ///     Allows the programmer to easily update rows in the DB.
        /// </summary>
        /// <param name="tableName">The table to update.</param>
        /// <param name="data">A dictionary containing Column names and their new values.</param>
        /// <param name="where">The where clause for the update statement.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Update(String tableName, Dictionary<String, String> data, String where)
        {
            String vals = "";
            Boolean returnCode = true;
            if (data.Count >= 1)
            {
                vals = data.Aggregate(vals, (current, val) => current + String.Format(" {0} = '{1}',", val.Key.ToString(), val.Value.ToString()));
                vals = vals.Substring(0, vals.Length - 1);
            }
            try
            {
                ExecuteNonQuery(String.Format("update {0} set {1} where {2};", tableName, vals, where));
            }
            catch
            {
                returnCode = false;
            }
            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily delete rows from the DB.
        /// </summary>
        /// <param name="tableName">The table from which to delete.</param>
        /// <param name="where">The where clause for the delete.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Delete(String tableName, String where)
        {
            Boolean returnCode = true;
            try
            {
                ExecuteNonQuery(String.Format("delete from {0} where {1};", tableName, where));
            }
            catch (SqliteException fail)
            {
//                ConsoleWriter.Log(fail.Message);
                returnCode = false;
            }
            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily insert into the DB
        /// </summary>
        /// <param name="tableName">The table into which we insert the data.</param>
        /// <param name="data">A dictionary containing the column names and data for the insert.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Insert(String tableName, Dictionary<String, String> data)
        {
            String columns = "";
            String values = "";
            Boolean returnCode = true;
            foreach (KeyValuePair<String, String> val in data)
            {
                columns += String.Format(" {0},", val.Key);
                values += String.Format(" '{0}',", val.Value);
            }
            columns = columns.Substring(0, columns.Length - 1);
            values = values.Substring(0, values.Length - 1);
            try
            {
                ExecuteNonQuery(String.Format("insert into {0}({1}) values({2});", tableName, columns, values));
            }
            catch (SqliteException fail)
            {
//                ConsoleWriter.Log(fail.Message);
                returnCode = false;
            }
            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily delete all data from the DB.
        /// </summary>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool ClearDb()
        {
            try
            {
                DataTable tables = GetDataTable("select NAME from SQLITE_MASTER where type='table' order by NAME;");
                foreach (DataRow table in tables.Rows)
                {
                    ClearTable(table["NAME"].ToString());
                }
                return true;
            }
            catch (SqliteException ex)
            {
//                ConsoleWriter.Log(ex.Message);
                return false;
            }
        }

        /// <summary>
        ///     Allows the user to easily clear all data from a specific table.
        /// </summary>
        /// <param name="table">The name of the table to clear.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool ClearTable(String table)
        {
            try
            {

                ExecuteNonQuery(String.Format("delete from {0};", table));
                return true;
            }
            catch (SqliteException ex)
            {
//                ConsoleWriter.Log(ex.Message);
                return false;
            }
        }
    }
}

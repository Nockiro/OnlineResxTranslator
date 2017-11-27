using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

/// <summary>
/// Helps with SQL queries
/// </summary>
public class SQLHelper {

    private static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
    private SqlConnection conn = new SqlConnection(connectionString);

    public bool connectionOpen = false;

    public SQLHelper()
    {

    }

    public SQLHelper OpenConnection()
    {
        try
        {
            conn.Open();
            connectionOpen = true;

            return this;
        }
        catch (Exception)
        {
            conn.Close();
            connectionOpen = false;

            return this;
        }
    }

    public void CloseConnection()
    {
        conn.Close();
    }


    /// <summary>
    /// creates table with given columns
    /// </summary>
    /// <param name="table">table to create</param>
    /// <param name="column">columns as keyvaluepair (name, type)</param>
    /// <returns>true if success</returns>
    public bool CreateTable(string table, params KeyValuePair<string, string>[] columns)
    {
        string cmd = "Create Table " + table + "( ";
        try
        {
            foreach (KeyValuePair<string, string> col in columns)
                cmd += col.Key + " " + col.Value + ",";

            cmd = cmd.Remove(cmd.LastIndexOf(","), 1) + ");";

            var createTable = new SqlCommand(cmd, conn);

            return createTable.ExecuteNonQuery() != -1;
        }
        catch (Exception e)
        {
            CloseConnection();
            throw e;
        }
    }

    /// <summary>
    /// Selects data from Table
    /// </summary>
    /// <param name="table">table to read from</param>
    /// <param name="columns">columns to read</param>
    /// <param name="where">adds search restrictions (Like "col1 LIKE bla")</param>
    /// <param name="addInfo">additional information (e.g. joins)</param>
    /// <returns>DataTable with returned data</returns>
    public DataTable SelectFromTable(string table, string[] columns, string where, string addInfo = "")
    {
        try
        {
            DataTable dataTable = new DataTable();

            string command = "SELECT " + string.Join(",", columns) + " FROM " + table;

            if (addInfo != "")
                command += " " + addInfo;

            if (where != "")
                command += " WHERE " + where;

            SqlCommand read = new SqlCommand(command, conn);

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(read);

            DataTable dtable = new DataTable();
            da.Fill(dtable);

            return dtable;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            CloseConnection();
            throw ex;
        }
    }

    /// <summary>
    /// Selects data from Table
    /// </summary>
    /// <param name="table">table to read from</param>
    /// <param name="columns">columns to read</param>
    /// <returns>DataTable with returned data</returns>
    public DataTable SelectFromTable(string table, params string[] columns)
    {
        return SelectFromTable(table, columns, "", "");
    }

    /// <summary>
    /// Selects data from Tables with inner join
    /// </summary>
    /// <param name="table">table to read from</param>
    /// <param name="addInfo">additional information (e.g. joins)</param>
    /// <param name="columns">columns to read</param>
    /// <returns>DataTable with returned data</returns>
    public DataTable SelectFromTables(string table, string addInfo, params string[] columns)
    {
        return SelectFromTable(table, columns, "", addInfo);
    }

    /// <summary>
    /// Inserts one row of data into a given table
    /// </summary>
    /// <param name="table">name of table</param>
    /// <param name="values">list of keyvaluepairs (key, value)</param>
    /// <returns>amount of affected rows</returns>
    public int InsertIntoTable(string table, params KeyValuePair<string, string>[] pairs)
    {
        try
        {
            string command = "insert into " + table + "(";

            string keys = string.Join(",", pairs.ToList().Select(t => t.Key));
            string atkeys = string.Join(",", pairs.ToList().Select(t => "@" + t.Key));
            command += keys + ") values (" + atkeys + ")";

            SqlCommand insert = new SqlCommand(command, conn);

            foreach (var kvp in pairs)
                insert.Parameters.AddWithValue("@" + kvp.Key, kvp.Value);

            return insert.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            CloseConnection();
            throw e;
        }
    }

    /// <summary>
    /// checks if table exists (obviously)
    /// </summary>
    /// <param name="table"></param>
    /// <returns>true if exists</returns>
    public bool DoesTableExist(string table)
    {
        try
        {
            // ANSI SQL way.  Works in PostgreSQL, MSSQL, MySQL.  
            var cmd = new SqlCommand(
              "select case when exists((select * from information_schema.tables where table_name = '" + table + "')) then 1 else 0 end", conn);

            object scal = cmd.ExecuteScalar();

            return (int)scal == 1;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// updates table with given entrie
    /// </summary>
    /// <param name="table">table to update</param>
    /// <param name="where">parameters which entries are to update</param>
    /// <param name="columns">entries as keyvaluepair (column, value)</param>
    /// <returns>true if success</returns>
    public bool CreateRow(string table, string where, params KeyValuePair<string, string>[] columns)
    {
        try
        {
            string cmd = "Update " + table + " set ";

            foreach (KeyValuePair<string, string> col in columns)
            {
                cmd += col.Key + "='" + col.Value + "',";
            }
            cmd = cmd.Remove(cmd.LastIndexOf(","), 1);

            if (where != "")
                cmd += " where " + where;

            var createTable = new SqlCommand(cmd, conn);

            return createTable.ExecuteNonQuery() != -1;
        }
        catch (Exception e)
        {
            CloseConnection();
            throw e;
        }
    }

    /// <summary>
    /// updates table with given entry
    /// </summary>
    /// <param name="table">table to update</param>
    /// <param name="where">parameters which entries are to update</param>
    /// <param name="columns">entries as keyvaluepair (column, value)</param>
    /// <returns>true if success</returns>
    public bool UpdateTable(string table, string where, params KeyValuePair<string, string>[] columns)
    {
        try
        {
            string cmd = "Update " + table + " set ";

            foreach (KeyValuePair<string, string> col in columns)
            {
                cmd += col.Key + "='" + col.Value + "',";
            }
            cmd = cmd.Remove(cmd.LastIndexOf(","), 1);

            if (where != "")
                cmd += " where " + where;

            var createTable = new SqlCommand(cmd, conn);

            return createTable.ExecuteNonQuery() != -1;
        }
        catch (Exception)
        {
            CloseConnection();
            throw;
        }
    }

    /// <summary>
    /// delete entry from table
    /// </summary>
    /// <param name="table">table to update</param>
    /// <param name="where">parameters which entries are to delete</param>
    /// <returns>true if success</returns>
    public bool DeleteRow(string table, string where)
    {
        try
        {
            string cmd = "delete from " + table + " where " + where;
            var createTable = new SqlCommand(cmd, conn);

            return createTable.ExecuteNonQuery() != -1;

        }
        catch (Exception)
        {
            CloseConnection();
            throw;
        }
    }
}
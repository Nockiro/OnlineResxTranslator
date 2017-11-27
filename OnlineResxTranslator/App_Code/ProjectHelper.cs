using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Contains all methods regarding the translation projects (not the ones working with the XML files)
/// </summary>
public class ProjectHelper {
    public ProjectHelper()
    {

    }


    /// <summary>
    /// Creates a list with projectinfo objects upon a datatable
    /// </summary>
    public static List<ProjectInfo> getProjectListFromDataTable(DataTable projectTable)
    {
        List<ProjectInfo> list = new List<ProjectInfo>();

        DataTable projectList = projectTable;
        foreach (DataRow r in projectList.Rows)
            list.Add(new ProjectInfo() { ID = (int)r["id"], Name = (string)r["project"], Folder = (string)r["folder"] });

        return list;
    }

    /// <summary>
    /// Gets a list with all currently registered projects
    /// </summary>
    /// <param name="UserID">if given, only projects for that user will be returned</param>
    public static List<ProjectInfo> getProjects(string UserID = "")
    {
        try
        {
            SQLHelper sqlhelper = new SQLHelper();
            sqlhelper.OpenConnection();
            List<ProjectInfo> list;

            if (UserID == "")
                list = getProjectListFromDataTable(sqlhelper.SelectFromTable("TrProjects", new string[] { "id", "project", "folder" }));
            else
                list = getProjectListFromDataTable(sqlhelper.SelectFromTable("TrProjects, TrUserProjects",
                    new string[] { "TrProjects.id as id", "TrProjects.project as project", "TrProjects.folder as folder" },
                    "TrUserProjects.UserID = '" + UserID + "' AND TrUserProjects.ProjID = TrProjects.id"));

            sqlhelper.CloseConnection();
            return list;
        }
        catch (SqlException) // probably the table doesn't exist - but since this method is called on every page call, it'd cost a bit of resources, so only catch it if needed
        {

            createProjectTable();
            return getProjects(UserID);
        }
    }

    public static void createProjectTable()
    {
        SQLHelper sqlhelper = new SQLHelper();
        sqlhelper.OpenConnection();
        if (!sqlhelper.DoesTableExist("TrProjects"))
            sqlhelper.CreateTable("TrProjects",
                new KeyValuePair<string, string>("id", "int NOT NULL IDENTITY (0,1) PRIMARY KEY"),
                new KeyValuePair<string, string>("project", "varchar(255) NOT NULL"),
                new KeyValuePair<string, string>("folder", "varchar(255) NOT NULL"));

        sqlhelper.CloseConnection();
    }

    public class ProjectInfo {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Folder { get; set; }
    }

    public class ProjectFileShortSummary {
        public string LangFile { get; set; }
        public String LangCode { get; set; }
        public Double Percentage { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
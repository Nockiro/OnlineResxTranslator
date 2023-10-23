using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using Identity;

/// <summary>
/// Contains all methods regarding the translation projects (not the ones working with the XML files)
/// </summary>
public class ProjectHelper
{
    public const int NOPROJID = -1;

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
    /// Creates a list with FTPTarget objects upon a datatable
    /// </summary>
    public static List<FTPTarget> getFTPTargetsFromDataTable(DataTable targetTable)
    {
        List<FTPTarget> list = new List<FTPTarget>();

        DataTable projectList = targetTable;
        foreach (DataRow r in projectList.Rows)
            list.Add(new FTPTarget()
            {
                ID = (int)r["id"],
                Server = (string)r["server"],
                Username = (string)r["username"],
                Password = (string)r["password"],
                Path = (string)r["path"]
            });

        return list;
    }

    /// <summary>
    /// Gets a list with (all) currently registered projects
    /// </summary>
    /// <param name="UserID">if given, only projects for that user will be returned</param>
    public static List<ProjectInfo> getProjects(string UserID = "")
    {
        try
        {
            SqlManager sqlManager = new SqlManager();
            sqlManager.OpenConnection();

            if (sqlManager.connectionOpen)
            {
                List<ProjectInfo> list;

                if (UserID == "")
                    list = getProjectListFromDataTable(sqlManager.SelectFromTable("TrProjects", new string[] { "id", "project", "folder" }));
                else
                    list = getProjectListFromDataTable(sqlManager.SelectFromTable("TrProjects, TrUserProjects",
                        new string[] { "TrProjects.id as id", "TrProjects.project as project", "TrProjects.folder as folder" },
                        "TrUserProjects.UserID = '" + UserID + "' AND TrUserProjects.ProjID = TrProjects.id"));

                sqlManager.CloseConnection();
                return list;
            }
            else return new List<ProjectInfo>();
        }
        catch (Exception e) // probably the table doesn't exist - but since this method is called on every page call, it'd cost a bit of resources, so only catch it if needed
        {
            if ((e is SqlException && !e.Message.Contains("syntax")) || e is InvalidOperationException)
            {
                if (createMainTables()) return getProjects(UserID);
            }
            else throw;

            return new List<ProjectInfo>();
        }
    }

    /// <summary>
    /// Gets a list with (all) currently registered ftp targets
    /// </summary>
    /// <param name="UserID">if given, only targets for that project will be returned</param>
    public static List<FTPTarget> getFTPTargets(ProjectInfo project = null)
    {
        SqlManager sqlManager = new SqlManager();
        sqlManager.OpenConnection();

        if (sqlManager.connectionOpen)
        {
            List<FTPTarget> list;

            if (project == null)
                list = getFTPTargetsFromDataTable(sqlManager.SelectFromTable("TrFTPTargets", new string[] { "id", "username", "password", "path", "server", "ssl" }));
            else
                list = getFTPTargetsFromDataTable(sqlManager.SelectFromTable("TrFTPTargets, TrProjectFTPTargets",
                    new string[] { "TrFTPTargets.id as id", "TrFTPTargets.username as username", "TrFTPTargets.password as password","TrFTPTargets.path as path", "TrFTPTargets.server as server",
                        "TrFTPTargets.ssl as TrFTPTargets", "TrProjectFTPTargets.ProjID as ProjID"},
                        "ProjID = " + project.ID));

            sqlManager.CloseConnection();
            return list;
        }
        else return new List<FTPTarget>();
    }

    /// <summary>
    /// Creates all additional tables not automatically created by the entity framework
    /// </summary>
    /// <returns>True if main tables were created successfully</returns>
    public static bool createMainTables()
    {
        Boolean success = false;
        SqlManager sqlManager = new SqlManager();
        sqlManager.OpenConnection();

        // we can't create the tables until the automatic creation process for the identity tables hasn't begun, so skip it in that case
        if (sqlManager.DoesTableExist("AspNetUsers"))
        {
            if (!sqlManager.DoesTableExist("TrProjects"))
                sqlManager.CreateTable("TrProjects",
                    new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("id", "int NOT NULL IDENTITY (0,1) PRIMARY KEY"),
                new KeyValuePair<string, string>("project", "varchar(255) NOT NULL"),
                new KeyValuePair<string, string>("folder", "varchar(255) NOT NULL")
                    }
                );

            if (!sqlManager.DoesTableExist("TrUserProjects"))
                sqlManager.CreateTable("TrUserProjects",
                    new KeyValuePair<string, string>[]
                    {
                new KeyValuePair<string, string>("UserID", "nvarchar(128) NOT NULL"),
                new KeyValuePair<string, string>("ProjID", "int NOT NULL")
                    },
                    new string[]
                     {
                     @" FK_PROJECT_USERS FOREIGN KEY (UserID)
                        REFERENCES AspNetUsers (Id)
                        ON DELETE CASCADE
                        ON UPDATE CASCADE",
                     @" FK_PROJECT_Projects FOREIGN KEY (ProjID)
                        REFERENCES TrProjects (id)
                        ON DELETE CASCADE
                        ON UPDATE CASCADE",
                     }
                );

            if (!sqlManager.DoesTableExist("TrUserLanguages"))
                sqlManager.CreateTable("TrUserLanguages",
                    new KeyValuePair<string, string>[]
                    {
                new KeyValuePair<string, string>("UserID", "nvarchar(128) NOT NULL"),
                new KeyValuePair<string, string>("Language", "varchar(128) NOT NULL")
                    },
                    new string[]
                     {
                     @" FK_PROJECTLANG_USERS FOREIGN KEY (UserID)
                        REFERENCES AspNetUsers (Id)
                        ON DELETE CASCADE
                        ON UPDATE CASCADE"
                     }
                );

            if (!sqlManager.DoesTableExist("TrFTPTargets"))
                sqlManager.CreateTable("TrFTPTargets",
                    new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("id", "int NOT NULL IDENTITY (0,1) PRIMARY KEY"),
                new KeyValuePair<string, string>("server", "varchar(255) NOT NULL"),
                new KeyValuePair<string, string>("username", "varchar(255) NOT NULL"),
                new KeyValuePair<string, string>("password", "varchar(255) NOT NULL"),
                new KeyValuePair<string, string>("path", "varchar(1023) NOT NULL"),
                new KeyValuePair<string, string>("ssl", "bit NOT NULL")
                    }
                );

            if (!sqlManager.DoesTableExist("TrProjectFTPTargets"))
                sqlManager.CreateTable("TrProjectFTPTargets",
                    new KeyValuePair<string, string>[]
                    {
                new KeyValuePair<string, string>("ProjID", "int NOT NULL"),
                new KeyValuePair<string, string>("TargetID", "int NOT NULL")
                    },
                    new string[]
                     {
                     @" FK_FTPS_PROJECT FOREIGN KEY (ProjID)
                        REFERENCES TrProjects (id)
                        ON DELETE CASCADE
                        ON UPDATE CASCADE",
                     @" FK_FTPS_TARGET FOREIGN KEY (TargetID)
                        REFERENCES TrFTPTargets (id)
                        ON DELETE CASCADE
                        ON UPDATE CASCADE",
                     }
                );
            success = true;
        }

        sqlManager.CloseConnection();
        return success;
    }

    /// <summary>
    /// Gets a list with all currently registered languages from that user
    /// </summary>
    /// <param name="UserID">only languages for that user will be returned</param>
    public static List<CultureInfo> getLanguages(IIdentity User)
    {
        if (User.GetUserId() == null)
            return new List<CultureInfo>();

        SqlManager sqlManager = new SqlManager();
        sqlManager.OpenConnection();
        List<CultureInfo> list = new List<CultureInfo>();

        if (User.GetDefaultLanguage() != "")
            list.Add(new CultureInfo(User.GetDefaultLanguage()));

        // take the string out of the database, split it by comma and create cultureinfos from it
        DataRowCollection tableRows = sqlManager.SelectFromTable("TrUserLanguages", new string[] { "language" }, "TrUserLanguages.UserID = '" + User.GetUserId() + "'").Rows;
        if (tableRows.Count > 0 && !String.IsNullOrEmpty((string)tableRows[0]["language"]))
            list.AddRange(((string)tableRows[0]["language"]).Split(',').Select(s => new CultureInfo(s.Trim())));

        sqlManager.CloseConnection();
        return list;
    }

    public static bool FTPUploadEnabled(ProjectInfo project)
    {
        return getFTPTargets(project).Count > 0;
    }

    public static bool FTPUpload(ProjectInfo project, string fullFilename, string language)
    {
        foreach (FTPTarget ftpTarget in getFTPTargets(project))
        {
            {
                string UploadPath = ftpTarget.Path;
                if (UploadPath.Length > 0)
                {
                    UploadPath = UploadPath.Replace("%LANG%", language);

                    if (!UploadPath.EndsWith("/")) UploadPath += "/";
                }
                else UploadPath = "/";

                // Get the object used to communicate with the server.
                FTP myFtp = new FTP(ftpTarget.Server, ftpTarget.Username, ftpTarget.Password, ftpTarget.SSL);

                List<string> ExistingFiles;
                try
                {
                    ExistingFiles = myFtp.directoryListDetailed(UploadPath).ToList();
                }
                catch (Exception e)
                {
                    myFtp.createDirectory(UploadPath);
                    ExistingFiles = myFtp.directoryListDetailed(UploadPath).ToList();
                }

                /*  if (ExistingFiles.Contains(Filename))
                      myFtp.delete(UploadPath + Filename);*/

                myFtp.upload(UploadPath + Path.GetFileName(fullFilename), fullFilename);
            }
        }
        return false;
    }

    public class ProjectInfo
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Folder { get; set; }
    }

    public class FTPTarget
    {
        public int ID { get; set; }
        private string _server;
        public string Server
        { get { return _server.StartsWith("ftp://") ? _server : "ftp://" + _server; } set { _server = value; } }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Path { get; set; }
        public Boolean SSL { get; set; }
    }

    public class ProjectFileShortSummary
    {
        public string LangFile { get; set; }
        public String LangCode { get; set; }
        public Double Percentage { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
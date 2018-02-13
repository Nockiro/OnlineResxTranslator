using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Globalization;

/// <summary>
/// Contains all methods regarding the translation projects (not the ones working with the XML files)
/// </summary>
public class ProjectHelper
{
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
        catch (Exception e) // probably the table doesn't exist - but since this method is called on every page call, it'd cost a bit of resources, so only catch it if needed
        {
            if (e is SqlException || e is InvalidOperationException)
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

    /// <summary>
    /// Gets a list with all currently registered languages from that user
    /// </summary>
    /// <param name="UserID">only languages for that user will be returned</param>
    public static List<CultureInfo> getLanguages(string UserID)
    {
        if (UserID == null)
            return new List<CultureInfo>();

        SQLHelper sqlhelper = new SQLHelper();
        sqlhelper.OpenConnection();
        List<CultureInfo> list;
        try
        {
            // take the string out of the database, split it by comma and create cultureinfos from it
            list = ((string)sqlhelper.SelectFromTable("TrUserLanguages", new string[] { "language" }, "TrUserLanguages.UserID = '" + UserID + "'")
                .Rows[0]["language"]).Split(',').Select(s => new CultureInfo(s.Trim())).ToList();
        }
        catch (IndexOutOfRangeException e)
        {
            throw new Exception("UserID " + UserID + " not found", e);
        }

        sqlhelper.CloseConnection();
        return list;
    }

    public static bool FTPUploadEnabled(ProjectInfo project)
    {
        string Filename = ConfigurationManager.AppSettings["ProjectDirectory"].ToString() + project.Name + ".xml";

        if (File.Exists(Filename))
        {
            XmlDocument ProjectFile = XMLFile.GetXMLDocument(Filename);
            if (ProjectFile.SelectNodes("/project/ftp/server").Count == 0)
                return false;

            else return true;
        }
        else return false;

    }

    public static bool FTPUpload(ProjectInfo project, string fullFilename)
    {
        string XMLFilename = ConfigurationManager.AppSettings["ProjectDirectory"].ToString() + project.Name + ".xml";

        XmlDocument ProjectFile = XMLFile.GetXMLDocument(XMLFilename);

        string Language = fullFilename.Split(".".ToCharArray())[fullFilename.Split(".".ToCharArray()).GetUpperBound(0) - 1];
        string Filename = fullFilename.Split("\\".ToCharArray())[fullFilename.Split("\\".ToCharArray()).GetUpperBound(0)];

        foreach (XmlNode FTPNode in ProjectFile.SelectNodes("/project/ftp"))
        {
            if (FTPNode.SelectSingleNode("server") == null)
            {
                throw new Exception("FTP-Upload not possible: Server node in XML file not found!");
            }
            else
            {
                string Servername = FTPNode.SelectSingleNode("server").InnerText;
                if (Servername.Length == 0)
                    throw new Exception("FTP-Upload not possible: Server node in XML file not properly defined!");
                else
                {
                    string UploadPath = FTPNode.SelectSingleNode("path").InnerText;
                    if (UploadPath.Length > 0)
                    {
                        if (UploadPath.Contains("%LANG%"))
                            UploadPath = UploadPath.Replace("%LANG%", Language);

                        if (!UploadPath.EndsWith("/")) UploadPath += "/";
                    }
                    else UploadPath = "/";

                    string Username = FTPNode.SelectSingleNode("username").InnerText;
                    string Password = FTPNode.SelectSingleNode("password").InnerText;


                    // Get the object used to communicate with the server.
                    FTP myFtp = new FTP(Servername, Username, Password);

                    List<string> ExistingFiles;
                    try
                    {
                        ExistingFiles = myFtp.directoryListDetailed(UploadPath).ToList();
                    }
                    catch (Exception)
                    {
                        myFtp.createDirectory(UploadPath);
                        ExistingFiles = myFtp.directoryListDetailed(UploadPath).ToList();
                    }

                    if (ExistingFiles.Contains(Filename))
                        myFtp.delete(UploadPath + Filename);

                    myFtp.upload(UploadPath + Filename, fullFilename);
                }
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

    public class ProjectFileShortSummary
    {
        public string LangFile { get; set; }
        public String LangCode { get; set; }
        public Double Percentage { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
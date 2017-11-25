﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// XMLFile: Manages XML files
/// </summary>
public class XMLFile {
    public XMLFile()
    {
    }

    /// <summary>
    /// Get XML Document
    /// </summary>
    /// <param name="filename">Full Filepath</param>
    /// <returns>XMLDoc or nothing</returns>
    /// <remarks></remarks>
    public static XmlDocument GetXMLDocument(string filename)
    {
        if (File.Exists(filename))
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            return doc;
        }
        else
        {
            // file does not exist
            return null;
        }
    }

    /// <summary>
    /// Gets value of give node
    /// </summary>
    /// <param name="xmlDoc">Document to be checked</param>
    /// <param name="nodeName">Name of the node which values should be returned</param>
    /// <returns>Value of the node. If node is nothing returns empty string</returns>
    /// <remarks></remarks>
    public static string GetXMLNodeValue(XmlDocument xmlDoc, string nodeName)
    {
        XmlNode ChildNode = default(XmlNode);
        if (xmlDoc == null)
        {
            return "";
        }
        else
        {
            ChildNode = xmlDoc.SelectSingleNode(nodeName);
            if (ChildNode == null)
            {
                return "";
            }
            else
            {
                return ChildNode.InnerText;
            }
        }
    }


    /// <summary>
    /// Refresh completed percentage
    /// </summary>
    /// <param name="project">ProjectInfo of the current project</param>
    /// <param name="language">shortcut of language, e.g. de</param>
    /// <param name="filename">File which was updated, e.g. beta.aspx. Or nothing to check all files</param>
    /// <returns>Percentage as integer</returns>
    /// <remarks></remarks>
    public static int ComputePercentage(ProjectInfo project, string language, string filename)
    {
        int Percentage = 0;

        string projDir = ConfigurationManager.AppSettings["ProjectDirectory"] + project.Folder + "\\";

        // Language file does not exist, so create new language file in a potential new folder
        if (!File.Exists(projDir + language + ".xml"))
        {
            // Now write the main chart xml for Form Update
            XmlTextWriter writer = new XmlTextWriter(projDir + language + ".xml", System.Text.Encoding.UTF8);

            var _with1 = writer;
            _with1.Formatting = Formatting.Indented;
            _with1.Indentation = 3;
            _with1.WriteStartDocument();
            _with1.WriteComment("Created on " + DateTime.Now.ToString());

            // <files>
            _with1.WriteStartElement("files");
            _with1.WriteAttributeString("language", language);

            string ResXFile = null;
            string ShortName = null;
            foreach (string ResXFile_loopVariable in Directory.GetFiles(projDir, "*.resx", SearchOption.TopDirectoryOnly))
            {
                // <file>
                ResXFile = ResXFile_loopVariable;
                _with1.WriteStartElement("file");
                ShortName = ResXFile.Substring(projDir.Length).Replace(".resx", "");

                _with1.WriteElementString("name", ShortName);
                _with1.WriteElementString("percentcompleted", "0");
                _with1.WriteElementString("lastchange", DateTime.Now.ToShortDateString());
                _with1.WriteEndElement();
                // </file>
            }

            _with1.WriteEndElement();
            // </files>

            _with1.WriteEndDocument();
            _with1.Close();
        }

        XmlDocument LanguageXML = XMLFile.GetXMLDocument(projDir + language + ".xml");

        // get all files that are registered in that language file
        XmlNodeList AllFiles = LanguageXML.SelectNodes("/files/file");

        bool SummaryUpdated = false;


        if (!Directory.Exists(projDir + language))
            Directory.CreateDirectory(projDir + language);

        foreach (XmlNode SingleFile in AllFiles)
        {
            string CurrentFile = CurrentFile = SingleFile.SelectSingleNode("name").InnerText;

            if (CurrentFile != null && CurrentFile != filename)
            {
                XmlDocument EnglishDoc = XMLFile.GetXMLDocument(projDir + CurrentFile + ".resx");

                // if null, english source file was not found
                if (EnglishDoc != null)
                {
                    XmlDocument TranslatedDoc = XMLFile.GetXMLDocument(projDir + language + "\\" + CurrentFile + "." + language + ".resx");

                    // Is translated language file not there?
                    if (TranslatedDoc == null)
                    {
                        // Create empty translation file

                        foreach (XmlNode Node in EnglishDoc.SelectNodes("/root/data/value"))
                            Node.InnerText = string.Empty;

                        // save the "emptied" english source file to the translated file name
                        EnglishDoc.Save(projDir + language + "\\" + CurrentFile + "." + language + ".resx");

                        SingleFile.SelectSingleNode("percentcompleted").InnerText = "0";
                    }
                    else // if the english source file is there
                    {
                        int FileElements = 0;
                        int TranslatedFileElements = 0;

                        // get through each node in the english doc ..
                        foreach (XmlNode EnglishNode in EnglishDoc.SelectNodes("root/data"))
                        {
                            string NodeName = EnglishNode.Attributes["name"].InnerXml;

                            Array NodePoints = default(Array);
                            NodePoints = NodeName.Split('.');

                            // set the not checked items
                            string[] NotArgs = new string[] { "Icon", "Size", "ImageStream", "Image", "Width", "Location", "ImeMode", "TabIndex", "TextAlign",
                                "ToolTip", "Dock", "ClientSize", "Enabled", "Groups", "ThousandsSeparator", "AutoSize", "BackgroundImage" };

                            bool CanBeAdded = true;

                            for (int i = 0; i <= NotArgs.Length - 1; i++)
                                if (NodeName.Contains("." + NotArgs[i])) CanBeAdded = false;


                            if (CanBeAdded)
                            {
                                FileElements += 1;

                                XmlNode TranslatedNode = TranslatedDoc.SelectSingleNode("root/data[@name='" + NodeName + "']");

                                // if translated node was null, it wasn't there
                                if (TranslatedNode != null)
                                {
                                    TranslatedNode = TranslatedNode.SelectSingleNode("value");

                                    // if value was null or text empty, the node was empty and therefore not translated
                                    if (TranslatedNode != null && (TranslatedNode.InnerText).Trim().Length > 0)
                                        TranslatedFileElements += 1;
                                }
                            }
                        }

                        if (FileElements == 0)
                            Percentage = 100;
                        else
                            Percentage = Convert.ToInt32(TranslatedFileElements / FileElements * 100);

                        // Logger.Write("Percent completed: " + Percentage.ToString + " %.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, logProperties);
                        //TotalElements += FileElements
                        //TranslatedElements += TranslatedFileElements
                    }
                }

                // Check whether percentage is changed - if this condition was true, the percentage was already stored correct
                if (Convert.ToInt32(SingleFile.SelectSingleNode("percentcompleted").InnerText) != Percentage)
                {

                    SingleFile.SelectSingleNode("percentcompleted").InnerText = Percentage.ToString();
                    SingleFile.SelectSingleNode("lastchange").InnerText = DateTime.Now.ToString();

                    if (!SummaryUpdated)
                        SummaryUpdated = true;
                }
            }
        }

        // Save configuration file if it was changed
        if (SummaryUpdated)
            LanguageXML.Save(projDir + language + ".xml");

        return Percentage;
    }

    /// <summary>
    /// Compute a summary for given project containing languages and percentages
    /// </summary>
    /// <param name="project">ProjectInfo of the project to be checked</param>
    /// <param name="maxPerc">only returns the objects in the list where the completed percentage is below gived value</param>
    /// <param name="minPerc">only returns the objects in the list where the completed percentage is above gived value</param>
    /// <returns>A List of translated Languages as ProjectFileShortSummary object</returns>
    public static List<ProjectFileShortSummary> ComputeSummary(ProjectInfo project, double minPerc = 0, double maxPerc = 99.999)
    {
        List<ProjectFileShortSummary> functionReturnValue = new List<ProjectFileShortSummary>();

        string ProjDirectory = ConfigurationManager.AppSettings["ProjectDirectory"] + project.Folder;

        if (Directory.Exists(ProjDirectory))
        {
            // Logger.Write("Directory '" + Directory + "' exists.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, LogProperties);

            string[] AllLanguageFiles = Directory.GetFiles(ProjDirectory, "*.xml", SearchOption.TopDirectoryOnly);

            foreach (string LanguageFilename in AllLanguageFiles)
            {
                XmlDocument LanguageXML = XMLFile.GetXMLDocument(LanguageFilename);

                ProjectFileShortSummary pss = new ProjectFileShortSummary();

                // if there is something wrong with the xml file, it will raise an exception here for the first time
                try
                {
                    pss.LangCode = LanguageXML.SelectSingleNode("files").Attributes["language"].Value;
                }
                catch (Exception e) { throw new Exception("Your language file " + LanguageFilename + " is damaged!", e); }

                double Percentage = 0.0;
                DateTime LastUpdate = DateTime.MinValue;

                foreach (XmlNode FileNode in LanguageXML.SelectNodes("/files/file"))
                {
                    Percentage += Convert.ToDouble(FileNode["percentcompleted"].InnerText);
                    try
                    {
                        DateTime LastChange = Convert.ToDateTime(FileNode["lastchange"].InnerText);
                        if (LastChange > LastUpdate)
                            LastUpdate = LastChange;
                    }
                    catch (FormatException) { }
                }

                Percentage = Percentage / LanguageXML.SelectNodes("/files/file").Count;
                pss.Percentage = Percentage;
                pss.LastUpdate = LastUpdate;
                pss.LangFile = LanguageFilename;

                if (Percentage >= minPerc && Percentage <= maxPerc)
                    functionReturnValue.Add(pss);
            }
        }
        return functionReturnValue;
    }

    public static List<ProjectInfo> getProjects()
    {
        List<ProjectInfo> list = new List<ProjectInfo>();
        SQLHelper sqlhelper = new SQLHelper();
        sqlhelper.OpenConnection();

        try
        {
            DataTable projectList = sqlhelper.SelectFromTable("TrProjects", "id", "project", "folder");
            foreach (DataRow r in projectList.Rows)
                list.Add(new ProjectInfo() { ID = (int)r["id"], Name = (string)r["project"], Folder = (string)r["folder"] });

            sqlhelper.CloseConnection();

            return list;
        }
        catch (SqlException) // probably the table doesn't exist - but since this method is called on every page call, it'd cost a bit of resources, so only catch it if needed
        {

            sqlhelper.OpenConnection();
            if (!sqlhelper.DoesTableExist("TrProjects"))
                sqlhelper.CreateTable("TrProjects",
                    new KeyValuePair<string, string>("id", "int NOT NULL IDENTITY (0,1) PRIMARY KEY"),
                    new KeyValuePair<string, string>("project", "varchar(255) NOT NULL"),
                    new KeyValuePair<string, string>("folder", "varchar(255) NOT NULL"));

            sqlhelper.CloseConnection();

            return getProjects();
        }

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
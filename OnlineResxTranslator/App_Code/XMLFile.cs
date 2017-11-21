using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Xml;
using System.IO;

/// <summary>
/// Zusammenfassungsbeschreibung für XMLFile
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
            //  // Logger.Write("File does not exist: '" + filename + "'.", Category, 200, 0, Diagnostics.TraceEventType.Warning, LogTitle);
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
    /// <param name="project">(Directory) name of the current project</param>
    /// <param name="language">shortcut of language, e.g. de</param>
    /// <param name="filename">File which was updated, e.g. beta.aspx. Or nothing to check all files</param>
    /// <remarks></remarks>
    public static int ComputePercentage(string project, string language, string filename)
    {
        int Percentage = 0;
        XmlDocument LanguageXML = default(XmlDocument);
        //     // Logger.Write(LogTitle + " -> Start [" + project + " - " + language + " - " + filename + "]", Category, 10, 0, Diagnostics.TraceEventType.Start, LogTitle, logProperties);
        string projDir = ConfigurationManager.AppSettings["ProjectDirectory"] + project + "\\";
        if (!File.Exists(projDir + language + ".xml"))
        {
            //  // Logger.Write("Language file '" + Directory + language + ".xml' does not exist.", Category, 100, 0, Diagnostics.TraceEventType.Information, LogTitle, logProperties);
            //Create new language file
            //Now write the main chart xml for Form Update
            XmlTextWriter writer = new XmlTextWriter(projDir + language + ".xml", System.Text.Encoding.UTF8);
            //  // Logger.Write("Creating '" + Directory + language + ".xml", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, logProperties);
            var _with1 = writer;
            _with1.Formatting = Formatting.Indented;
            _with1.Indentation = 3;
            _with1.WriteStartDocument();
            _with1.WriteComment("Created on " + DateTime.Now.ToString());
            _with1.WriteStartElement("files");
            _with1.WriteAttributeString("language", language);

            string ResXFile = null;
            string ShortName = null;
            foreach (string ResXFile_loopVariable in Directory.GetFiles(projDir, "*.resx", SearchOption.TopDirectoryOnly))
            {
                ResXFile = ResXFile_loopVariable;
                _with1.WriteStartElement("file");
                ShortName = ResXFile.Substring(projDir.Length).Replace(".resx", "");
                //  // Logger.Write("Adding '" + ShortName + "'.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, logProperties);
                _with1.WriteElementString("name", ShortName);
                _with1.WriteElementString("percentcompleted", "0");
                _with1.WriteElementString("lastchange", DateTime.Now.ToShortDateString());
                _with1.WriteEndElement();
                //</file>
            }

            _with1.WriteEndElement();
            //</files>
            // // Logger.Write("All files added.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, logProperties);
            _with1.WriteEndDocument();
            _with1.Close();
            //   // Logger.Write("Created '" + Directory + language + ".xml successfully.", Category, 100, 0, Diagnostics.TraceEventType.Information, LogTitle, logProperties);
        }
        LanguageXML = XMLFile.GetXMLDocument(projDir + language + ".xml");
        XmlNodeList AllFiles = LanguageXML.SelectNodes("/files/file");
        //     // Logger.Write("Language File '" + Directory + language + ".xml' contains " + AllFiles.Count + " Resource files.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, logProperties);

        bool Skip = false;
        string CurrentFile = null;
        string NodeName = null;
        bool SummaryUpdated = false;
        int FileElements = 0;
        int TranslatedFileElements = 0;
        if (!Directory.Exists(projDir + language))
        {
            // Logger.Write("Creating new directory '" + Directory + language + "' now.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, logProperties);
            Directory.CreateDirectory(projDir + language);
        }
        foreach (XmlNode SingleFile in AllFiles)
        {
            CurrentFile = SingleFile.SelectSingleNode("name").InnerText;
            if ((filename == null) || (CurrentFile == filename))
                Skip = false;
            else
                Skip = true;

            if (!Skip)
            {
                // Logger.Write("Checking '" + CurrentFile + "'.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, logProperties);
                XmlDocument EnglishDoc = XMLFile.GetXMLDocument(projDir + CurrentFile + ".resx");
                if (EnglishDoc == null)
                {
                    // Logger.Write("Could not find '" + Directory + CurrentFile + ".resx'!", Category, 200, 0, Diagnostics.TraceEventType.Warning, LogTitle, logProperties);
                }
                else
                {
                    XmlDocument TranslatedDoc = XMLFile.GetXMLDocument(projDir + language + "\\" + CurrentFile + "." + language + ".resx");
                    if (TranslatedDoc == null)
                    {
                        // Logger.Write("Could not find '" + Directory + language + "\\" + CurrentFile + "." + language + ".resx'!", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, logProperties);
                        //Create empty translation file

                        foreach (XmlNode Node in EnglishDoc.SelectNodes("/root/data/value"))
                        {
                            Node.InnerText = string.Empty;
                        }
                        EnglishDoc.Save(projDir + language + "\\" + CurrentFile + "." + language + ".resx");
                        // Logger.Write("Saved empty '" + Directory + language + "\\" + CurrentFile + "." + language + ".resx'.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, logProperties);
                        SingleFile.SelectSingleNode("percentcompleted").InnerText = "0";
                    }
                    else
                    {
                        FileElements = 0;
                        TranslatedFileElements = 0;
                        foreach (XmlNode EnglishNode in EnglishDoc.SelectNodes("root/data"))
                        {
                            NodeName = EnglishNode.Attributes["name"].InnerXml;

                            Array NodePoints = default(Array);
                            NodePoints = NodeName.Split('.');
                            //Festlegen, welche Arten von Items nicht zur Übersetzung angeboten werden
                            string[] NotArgs = new string[] { "Icon", "Size", "ImageStream", "Image", "Width", "Location", "ImeMode", "TabIndex", "TextAlign",
                                "ToolTip", "Dock", "ClientSize", "Enabled", "Groups", "ThousandsSeparator", "AutoSize", "BackgroundImage" };

                            bool CanBeAdded = true;

                            for (int i = 0; i <= NotArgs.Length - 1; i++)
                            {
                                if (NodeName.Contains("." + NotArgs[i]))
                                    CanBeAdded = false;
                            }

                            if (CanBeAdded)
                            {
                                FileElements += 1;

                                XmlNode TranslatedNode = TranslatedDoc.SelectSingleNode("root/data[@name='" + NodeName + "']");
                                if (TranslatedNode == null)
                                {
                                    // Logger.Write("Item '" + NodeName + "' is missing.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, logProperties);
                                }
                                else
                                {
                                    TranslatedNode = TranslatedNode.SelectSingleNode("value");
                                    if (TranslatedNode == null)
                                    {
                                        // Logger.Write("Item '" + NodeName + "' has no node 'value'.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, logProperties);
                                    }
                                    else
                                    {
                                        if ((TranslatedNode.InnerText).Trim().Length > 0)
                                        {
                                            TranslatedFileElements += 1;
                                        }
                                        else
                                        {
                                            // Logger.Write("Existing node '" + NodeName + "' is not yet translated (value is empty).", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, logProperties);
                                        }
                                    }
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

                //Check whether percentage is changed
                if (Convert.ToInt32(SingleFile.SelectSingleNode("percentcompleted").InnerText) == Percentage)
                {
                    // Logger.Write("Completed percentage (" + Percentage.ToString + "%) is already stored correct.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, logProperties);
                }
                else
                {
                    // Logger.Write("Update currently stored percentage (" + SingleFile.SelectSingleNode("percentcompleted").InnerText + " %).", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, logProperties);
                    SingleFile.SelectSingleNode("percentcompleted").InnerText = Percentage.ToString();
                    SingleFile.SelectSingleNode("lastchange").InnerText = DateTime.Now.ToString();
                    if (!SummaryUpdated)
                        SummaryUpdated = true;

                }
            }
        }
        if (SummaryUpdated)
            // Logger.Write("Percentage was changed. Saving configuration file.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, logProperties);
            LanguageXML.Save(projDir + language + ".xml");
        // Logger.Write(Directory + language + ".xml updated.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, logProperties);

        return Percentage;
        // Logger.Write(LogTitle + " -> End.", Category, 10, 0, Diagnostics.TraceEventType.Stop, LogTitle, logProperties);
    }

    /// <summary>
    /// Compute a summary for given project containing languages and percentages
    /// </summary>
    /// <param name="project">name of the project to be checked</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static List<ProjectShortSummary> ComputeSummary(string project, double minPerc = 0, double maxPerc = 99.999)
    {
        List<ProjectShortSummary> functionReturnValue = new List<ProjectShortSummary>();

        string ProjDirectory = ConfigurationManager.AppSettings["ProjectDirectory"] + project + "\\";
        if (!Directory.Exists(ProjDirectory))
        {
            // Logger.Write("Directory '" + Directory + "' does not exist.", Category, 100, 0, Diagnostics.TraceEventType.Information, LogTitle, LogProperties);
        }
        else
        {
            // Logger.Write("Directory '" + Directory + "' exists.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, LogProperties);

            XmlDocument LanguageXML = default(XmlDocument);
            string[] AllLanguageFiles = Directory.GetFiles(ProjDirectory, "*.xml", SearchOption.TopDirectoryOnly);

            DateTime LastUpdate = default(DateTime);
            DateTime LastChange = default(DateTime);

            foreach (string LanguageFilename in AllLanguageFiles)
            {
                LanguageXML = XMLFile.GetXMLDocument(LanguageFilename);
                ProjectShortSummary pss = new ProjectShortSummary();

                // if there is something wrong with the xml file, it will raise an exception here for the first time
                try
                {
                    pss.LangCode = LanguageXML.SelectSingleNode("files").Attributes["language"].Value;
                }
                catch (Exception e)
                {
                    throw new Exception("Your language file " + LanguageFilename + " is damaged!", e);
                }

                double Percentage = 0.0;
                LastUpdate = DateTime.MinValue;
                foreach (XmlNode FileNode in LanguageXML.SelectNodes("/files/file"))
                {
                    Percentage += Convert.ToDouble(FileNode["percentcompleted"].InnerText);
                    try
                    {
                        LastChange = Convert.ToDateTime(FileNode["lastchange"].InnerText);
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

    public class ProjectShortSummary {
        public string LangFile { get; set; }
        public String LangCode { get; set; }
        public Double Percentage { get; set; }
        public DateTime LastUpdate { get; set; }
    }

}
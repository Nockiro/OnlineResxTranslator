using System.Xml;
using System.IO;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;

/// <summary>
/// XMLFile: Manages XML files
/// Original work Copyright (C) 2010 Kai Wilzer
/// Modified by Nockiro 2017-2018 in order to improve and document the code 
/// and adapt it to the rest of the project
/// </summary>
public class XMLFile
{
    // set the not checked items
    public static readonly string[] NotArgs = new string[] { "Icon", "Size", "ImageStream", "Image", "Width", "Location", "ImeMode", "TabIndex", "TextAlign",
                                "ToolTip", "Dock", "ClientSize", "Enabled", "Visible", "Groups", "ThousandsSeparator", "AutoSize", "BackgroundImage", "Type", "ZOrder", "Parent", "Name",
                                "Padding", "Anchor", "AutoScaleDimensions", "Multiline", "Font", "TextImageRelation", "SplitterDistance","FlatStyle", "ColumnCount", "RowCount", "LayoutSettings", "CheckAlign",
                                "Item", "RightToLeft", "Nodes"};


    public XMLFile()
    {
    }

    /// <summary>
    /// Refresh completed percentage
    /// </summary>
    /// <param name="project">ProjectInfo of the current project</param>
    /// <param name="language">shortcut of language, e.g. de</param>
    /// <param name="filename">File which was updated, e.g. beta.aspx. Or null to check all files</param>
    /// <param name="sourceLang">Source language the file is compared to</param>
    /// <returns>Percentage as integer</returns>
    /// <remarks>Creates info file if not existing</remarks>
    public static double ComputePercentage(ProjectHelper.ProjectInfo project, string language, string filename, string sourceLang)
    {
        double Percentage = 0;

        string ProjectDirectory = ConfigurationManager.AppSettings["ProjectDirectory"] + project.Folder + "\\";
        if (!Directory.Exists(ProjectDirectory))
            Directory.CreateDirectory(ProjectDirectory);

        string[] allMainProjectFiles = Directory.GetFiles(ProjectDirectory, "*.resx", SearchOption.TopDirectoryOnly);

        if (allMainProjectFiles.Length == 0)
            throw new Exception("No Resource files in the project directory " + ProjectDirectory);

        // Language file does not exist, so create new language file in a potential new folder
        if (!File.Exists(ProjectDirectory + language + ".xml"))
        {
            // Now write the main chart xml for Form Update
            XmlTextWriter writer = new XmlTextWriter(ProjectDirectory + language + ".xml", System.Text.Encoding.UTF8)
            {
                Formatting = Formatting.Indented,
                Indentation = 3
            };
            writer.WriteStartDocument();
            writer.WriteComment("Created on " + DateTime.Now.ToString());

            // <files>
            writer.WriteStartElement("files");
            writer.WriteAttributeString("language", language);

            string ResXFile = null;
            string ShortName = null;
            foreach (string ResXFile_loopVariable in allMainProjectFiles)
            {
                // <file>
                ResXFile = ResXFile_loopVariable;
                writer.WriteStartElement("file");
                ShortName = ResXFile.Substring(ProjectDirectory.Length).Replace(".resx", "");

                writer.WriteElementString("name", ShortName);
                writer.WriteElementString("percentcompleted", "0");
                writer.WriteElementString("caption", "");
                writer.WriteElementString("lastchange", DateTime.Now.ToShortDateString());
                writer.WriteEndElement();
                // </file>
            }

            writer.WriteEndElement();
            // </files>

            writer.WriteEndDocument();
            writer.Close();
        }

        XmlDocument LanguageXML = XMLFile.GetXMLDocument(ProjectDirectory + language + ".xml");

        // get all files that are registered in that language file
        XmlNodeList AllFiles = LanguageXML.SelectNodes("/files/file");

        bool SummaryUpdated = false;


        if (!Directory.Exists(ProjectDirectory + language))
            Directory.CreateDirectory(ProjectDirectory + language);

        foreach (XmlNode SingleFile in AllFiles)
        {
            string CurrentFile = SingleFile.SelectSingleNode("name").InnerText;

			// if the current file in the directoy is not invalid and either no file was specified to test (== all files) or the file matches the given one
            if (CurrentFile != null && (filename == null || CurrentFile == filename))
            {
                XmlDocument SourceFile;

                if (Directory.Exists(Path.Combine(ProjectDirectory, sourceLang)))
                    SourceFile = XMLFile.GetXMLDocument(Path.Combine(ProjectDirectory, sourceLang, CurrentFile + "." + sourceLang + ".resx"));
                else
                    SourceFile = XMLFile.GetXMLDocument(ProjectDirectory + CurrentFile + ".resx");

                // if not null, english source file was found
                if (SourceFile != null)
                {
                    XmlDocument TranslatedDoc = XMLFile.GetXMLDocument(Path.Combine(ProjectDirectory, language, (CurrentFile + "." + language + ".resx")));

                    // Is translated language file not there?
                    if (TranslatedDoc == null)
                    {
                        // Create empty translation file
                        foreach (XmlNode Node in SourceFile.SelectNodes("/root/data/value"))
                            Node.InnerText = string.Empty;

                        // save the "emptied" english source file to the translated file name
                        SourceFile.Save(Path.Combine(ProjectDirectory, language, (CurrentFile + "." + language + ".resx")));

                        SingleFile.SelectSingleNode("percentcompleted").InnerText = "0";
                    }
                    else // if the translation file
                    {
                        double FileElements = 0;
                        double TranslatedFileElements = 0;

                        // get through each node in the english doc ..
                        foreach (XmlNode SourceNode in SourceFile.SelectNodes("root/data"))
                        {
                            string NodeName = SourceNode.Attributes["name"].InnerText;

                            Array NodePoints = default(Array);
                            NodePoints = NodeName.Split('.');


                            bool CanBeAdded = true;

                            for (int i = 0; i <= NotArgs.Length - 1; i++)
                                if (NodeName.Contains("." + NotArgs[i])
									|| String.IsNullOrEmpty(SourceNode.SelectSingleNode("value")?.InnerText)) CanBeAdded = false;


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
                            Percentage = (TranslatedFileElements / FileElements) * 100;
	
                    }
                }
				
				SingleFile.SelectSingleNode("caption").InnerText = "";

                // Check whether percentage is changed - if this condition was true, the percentage was already stored correct
                if (Convert.ToDouble(SingleFile.SelectSingleNode("percentcompleted").InnerText.Replace(",", "."), CultureInfo.InvariantCulture) != Percentage)
                {

                    SingleFile.SelectSingleNode("percentcompleted").InnerText = Percentage.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                    SingleFile.SelectSingleNode("lastchange").InnerText = DateTime.Now.ToString();

                    if (!SummaryUpdated)
                        SummaryUpdated = true;
                }
				
            }
        }

        // Save configuration file if it was changed
        if (SummaryUpdated)
            LanguageXML.Save(ProjectDirectory + language + ".xml");

        return Percentage;
    }

    /// <summary>
    /// Compute a summary for given project containing languages and percentages
    /// </summary>
    /// <param name="project">ProjectInfo of the project to be checked</param>
    /// <param name="maxPerc">only returns the objects in the list where the completed percentage is below gived value</param>
    /// <param name="minPerc">only returns the objects in the list where the completed percentage is above gived value</param>
    /// <returns>A List of translated Languages as ProjectFileShortSummary object</returns>
    public static List<ProjectHelper.ProjectFileShortSummary> ComputeSummary(ProjectHelper.ProjectInfo project, double minPerc = 0, double maxPerc = 99.999)
    {
        List<ProjectHelper.ProjectFileShortSummary> functionReturnValue = new List<ProjectHelper.ProjectFileShortSummary>();

        string ProjDirectory = ConfigurationManager.AppSettings["ProjectDirectory"] + project.Folder;

        if (Directory.Exists(ProjDirectory))
        {
            // Logger.Write("Directory '" + Directory + "' exists.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, LogProperties);

            string[] AllLanguageFiles = Directory.GetFiles(ProjDirectory, "*.xml", SearchOption.TopDirectoryOnly);

            foreach (string LanguageFilename in AllLanguageFiles)
            {
                XmlDocument LanguageXML = XMLFile.GetXMLDocument(LanguageFilename);

                ProjectHelper.ProjectFileShortSummary pss = new ProjectHelper.ProjectFileShortSummary();

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
                    Percentage += Convert.ToDouble(FileNode["percentcompleted"].InnerText, CultureInfo.InvariantCulture);
                    try
                    {
                        DateTime LastChange = DateTime.Parse(FileNode["lastchange"].InnerText);
                        if (LastChange > LastUpdate)
                            LastUpdate = LastChange;
                    }
                    catch (FormatException) { }
                }

                Percentage = Math.Round(Percentage / LanguageXML.SelectNodes("/files/file").Count, 4);
                pss.Percentage = Percentage;
                pss.LastUpdate = LastUpdate;
                pss.LangFile = LanguageFilename;

                if (Percentage >= minPerc && Percentage <= maxPerc)
                    functionReturnValue.Add(pss);
            }
        }
        return functionReturnValue;
    }

    /// <summary>
    /// Get XML Document
    /// </summary>
    /// <param name="filename">Full Filepath</param>
    /// <returns>XMLDoc or nothing</returns>
    /// <remarks></remarks>
    public static XmlDocument GetXMLDocument(string filename)
    {
        try
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
        catch (FileNotFoundException) // in my case File.Exists didn't completely work on symlinks
        {
            return null;
        }
		catch (XmlException e)
		{
            throw new Exception($"File {filename} has invalid data.", e);
		}
    }

    /// <summary>
    /// Gets value of give node
    /// </summary>
    /// <param name="xmlDoc">Document to be checked</param>
    /// <param name="nodeName">Name of the node which values should be returned</param>
    /// <returns>Value of the node. If node is nothing returns empty string</returns>
    public static string GetXMLNodeValue(XmlDocument xmlDoc, string nodeName) => xmlDoc?.SelectSingleNode(nodeName)?.InnerText ?? "";

}
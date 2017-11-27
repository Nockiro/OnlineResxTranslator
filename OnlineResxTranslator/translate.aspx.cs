
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Xml;
using System.Web.UI;

partial class _Translate : Page {

    const string Category = "Translate.aspx";
    protected void Page_Load(object sender, System.EventArgs e)
    {/*

        if (!Page.IsPostBack)
        {

            const string LogTitle = "Page_Load";
            Dictionary<string, object> LogProperties = default(Dictionary<string, object>);
            // Logger.Write(LogTitle + " -> Start.", Category, 10, 0, Diagnostics.TraceEventType.Start, LogTitle);

            if (Session["User"] == null)
            {
                // Logger.Write("Session expited.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle);
                Session["ErrorMessage"] = "Session expired. Please login!";
                Response.Redirect("Error.aspx");
            }
            else
            {
                System.Xml.XmlNodeList AllProjects = default(System.Xml.XmlNodeList);
                AllProjects = XMLFile.GetUserFile.SelectNodes("/users/user[@email=\"" + UserAccount.Text.ToLower + "\"]/project");
                if (AllProjects.Count == 0)
                {
                    // Logger.Write("No project returned for user '" + UserAccount.Text + "'.", Category, 100, 0, Diagnostics.TraceEventType.Information, LogTitle, LogProperties);
                    Session["ErrorMessage"] = "You are not registered for any project.";
                    Response.Redirect("Error.aspx");
                }
                else
                {
                    // Logger.Write("Found " + AllProjects.Count + " projects for user '" + UserAccount.Text + "'.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, LogProperties);
                    string Project = Convert.ToString(Session["Project"]);
                    string Languages = Convert.ToString(Session["Languages"]);
                    if (Project == null)
                    {
                        // Logger.Write("No project selected so far.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, LogProperties);
                        Project = AllProjects(0).InnerText;
                        Session["Project"] = Project;
                        Languages = AllProjects(0).Attributes("lang").InnerText;
                        Session["Languages"] = Languages;
                        // Logger.Write("Set first project '" + Project + "' as current project.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, LogProperties);
                    }
                    SelectedProject.Text = Project + " files";
                    var _with1 = MyProjects;
                    _with1.DataSource = AllProjects;
                    _with1.DataBind();

                    string Directory = ConfigurationManager.AppSettings("ProjectDirectory").ToString + Project + "\\";
                    if (Languages.Contains(","))
                    {
                        Languages = Strings.Left(Languages, Strings.InStr(Languages, ",") - 1);
                    }
                    // Logger.Write("Getting '" + Directory + Languages + ".xml'.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, LogProperties);
                    System.Xml.XmlDocument LanguageXML = default(System.Xml.XmlDocument);
                    if (!IO.File.Exists(Directory + Languages + ".xml"))
                    {
                        // Logger.Write("Language file '" + Directory + Languages + ".xml' does not exist.", Category, 100, 0, Diagnostics.TraceEventType.Information, LogTitle, LogProperties);
                        XMLFile.ComputePercentage(Project, Languages, null, LogProperties);
                    }
                    if (!IO.File.Exists(Directory + Languages + ".xml"))
                    {
                        // Logger.Write("Language file '" + Directory + Languages + ".xml' does not exist.", Category, 200, 0, Diagnostics.TraceEventType.Error, LogTitle, LogProperties);
                        Session["ErrorMessage"] = "Language file for '" + Languages + "' does not exist!";
                        Response.Redirect("Error.aspx");
                    }
                    else
                    {
                        LanguageXML = XMLFile.GetXMLDocument(Directory + Languages + ".xml");
                        System.Xml.XmlNodeList AllFiles = default(System.Xml.XmlNodeList);
                        AllFiles = LanguageXML.SelectNodes("/files[@language=\"" + Languages + "\"]/file");
                        Data.DataSet oDs = new Data.DataSet();
                        oDs.ReadXml(Directory + Languages + ".xml");
                        var _with2 = FileList;
                        _with2.DataSource = oDs.Tables(1);
                        _with2.DataBind();

                        //// Logger.Write(AllFiles.Count & " files defined for '" & Languages & "' in Project '" & Project & "'.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, LogProperties)
                        //If AllFiles.Count = 0 Then
                        //    Session["ErrorMessage"] = "No files defined for '" & Languages & "' in Project '" & Project & "'."
                        //    Response.Redirect("Error.aspx")
                        //Else
                        //    With FileList
                        //        .DataSource = AllFiles
                        //        .DataBind()
                        //    End With
                        //    Dim SingleFile As System.Xml.XmlNode
                        //    For Each SingleFile In AllFiles

                        //    Next
                        //End If
                    }


                    //Check whether a file is selected
                    if (Session["SelectedFilename"] == null | object.ReferenceEquals(Session["SelectedFilename"], "Summary"))
                    {
                        // Logger.Write("No file selected currently.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, LogProperties);
                        //Show summary
                        //SelectedFile = String.Empty
                        Save.Visible = false;
                        Session["SelectedFilename"] = "Summary";
                        Data.DataTable Summary = new Data.DataTable();
                        Summary = XMLFile.ComputeSummary(Convert.ToString(Session["Project"]));
                        // Logger.Write("Summary for project '" + Convert.ToString(Session["Project"]) + " contains " + Summary.Rows.Count.ToString + " rows.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, LogProperties);
                        var _with3 = SummaryRepeater;
                        _with3.DataSource = Summary;
                        _with3.DataBind();
                        _with3.Visible = true;
                    }
                    else
                    {
                        SummaryRepeater.Visible = false;
                        CurrentFile.Text = "Selected file: " + Convert.ToString(Session["SelectedFilename"]);
                        // Logger.Write("Selected filename: '" + Convert.ToString(Session["SelectedFilename"]) + "'.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, LogProperties);
                        System.Xml.XmlDocument EnglishFile = default(System.Xml.XmlDocument);
                        System.Xml.XmlDocument TranslatedFile = default(System.Xml.XmlDocument);
                        EnglishFile = XMLFile.GetXMLDocument(Directory + Convert.ToString(Session["SelectedFilename"]) + ".resx");
                        TranslatedFile = XMLFile.GetXMLDocument(Directory + Languages + "\\" + Convert.ToString(Session["SelectedFilename"]) + "." + Languages + ".resx");
                        Data.DataTable Table = new Data.DataTable();
                        Table.Columns.Add("TextName");
                        Table.Columns.Add("English");
                        Table.Columns.Add("Translation");
                        Data.DataRow Row = default(Data.DataRow);
                        System.Xml.XmlNode Text = default(System.Xml.XmlNode);
                        System.Xml.XmlNode Translated = default(System.Xml.XmlNode);
                        foreach (Text in EnglishFile.SelectNodes("/root/data"))
                        {
                            Row = Table.NewRow;
                            Row.Item("TextName") = Text.Attributes("name").InnerText;
                            Row.Item("English") = Text.SelectSingleNode("value").InnerText;
                            Translated = TranslatedFile.SelectSingleNode("/root/data[@name=\"" + Row.Item("Textname").ToString + "\"]/value");
                            if (Translated == null)
                            {
                                Row.Item("Translation") = string.Empty;
                            }
                            else
                            {
                                Row.Item("Translation") = Translated.InnerText;
                                //HttpUtility.HtmlEncode(Translated.InnerText)
                            }
                            Array RowPoints = default(Array);
                            RowPoints = Row.Item("TextName").ToString().Split(".");

                            //Festlegen, welche Arten von Items nicht zur Übersetzung angeboten werden
                            string[] NotArgs = new string[16];
                            NotArgs(0) = "Icon";
                            NotArgs(1) = "Size";
                            NotArgs(2) = "ImageStream";
                            NotArgs(3) = "Image";
                            NotArgs(4) = "Width";
                            NotArgs(5) = "Location";
                            NotArgs(6) = "ImeMode";
                            NotArgs(7) = "TabIndex";
                            NotArgs(8) = "TextAlign";
                            NotArgs(9) = "ToolTip";
                            NotArgs(10) = "Dock";
                            NotArgs(11) = "ClientSize";
                            NotArgs(12) = "Enabled";
                            NotArgs(13) = "Groups";
                            NotArgs(14) = "ThousandsSeparator";
                            NotArgs(15) = "AutoSize";
                            bool CanBeAdded = true;

                            for (int i = 0; i <= NotArgs.Length - 1; i++)
                            {
                                if (RowPoints(RowPoints.Length - 1).ToString().Contains(NotArgs(i)) & Row.Item("TextName").ToString().Contains("." + NotArgs(i)))
                                {
                                    CanBeAdded = false;
                                }
                            }

                            if (CanBeAdded & (!object.ReferenceEquals(Row.Item("English").ToString(), "")))
                            {
                                Table.Rows.Add(Row);
                            }
                        }
                        TextElements.DataSource = Table;
                        TextElements.DataBind();
                        Save.Visible = true;
                    }




                    //For Each SingleProject In AllProjects
                    //    FileList.DataSource = AllProjects

                    //    AddProject = New ListItem(SingleProject.InnerText)
                    //    If Project = SingleProject.InnerText Then
                    //        AddProject.Selected = True
                    //    End If
                    //    UserProjects.Items.Add(AddProject)
                    //Next
                }



            }
        }*/
    }

    protected void FileList_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        /* if (e.CommandName.CompareTo("EditFile") == 0)
         {
             const string LogTitle = "EditFile";

             Dictionary<string, object> LogProperties = default(Dictionary<string, object>);
             // Logger.Write(LogTitle + " -> Start.", Category, 10, 0, Diagnostics.TraceEventType.Start, LogTitle);

             if (Convert.ToString(Session["User"]).Length == 0)
             {
                 // Logger.Write("Session expired.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle);
                 Session["ErrorMessage"] = "Session expired. Please login!";
                 Response.Redirect("Error.aspx");
             }
             else
             {
                 LogProperties = General.CreateLogProperties(Convert.ToString(Session["User"]));
                 string Filename = null;
                 try
                 {
                     Filename = FileList.DataKeys(Convert.ToInt32(e.CommandArgument)).Value.ToString;
                     if (Filename.Length == 0)
                     {
                         // Logger.Write("No file selected.", Category, 200, 0, Diagnostics.TraceEventType.Error, LogTitle, LogProperties);
                         Session["ErrorMessage"] = "No file selected";
                     }
                 }
                 catch (Exception ex)
                 {
                     // Logger.Write("Error computing selected filename: " + ex.Message, Category, 200, 0, Diagnostics.TraceEventType.Error, LogTitle, LogProperties);
                     Session["ErrorMessage"] = ex.Message;
                 }
                 if (Filename.Length == 0)
                 {
                     Response.Redirect("Error.aspx");
                 }
                 else
                 {
                     // Logger.Write("Selected filename: '" + Filename + "'.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, LogProperties);
                     Session["SelectedFilename"] = Filename;
                     XMLFile.ComputePercentage(Convert.ToString(Session["Project"]), Convert.ToString(Session["Languages"]), Convert.ToString(Session["SelectedFilename"]), LogProperties);
                     Response.Redirect("Translate.aspx");
                 }

             }
         }*/
    }

    protected void MyProjects_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        /*
        if (e.CommandName.CompareTo("SwitchProject") == 0)
        {

            Dictionary<string, object> LogProperties = default(Dictionary<string, object>);
            // Logger.Write(LogTitle + " -> Start.", Category, 10, 0, Diagnostics.TraceEventType.Start, LogTitle);

            if (Convert.ToString(Session["User"]).Length == 0)
            {
                // Logger.Write("Session expired.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle);
                Session["ErrorMessage"] = "Session expired. Please login!";
                Response.Redirect("Error.aspx");
            }
            else
            {
                string ProjectName = null;
                try
                {
                    ProjectName = MyProjects.DataKeys(Convert.ToInt32(e.CommandArgument)).Value.ToString;
                    if (ProjectName.Length == 0)
                    {
                        // Logger.Write("No project selected.", Category, 200, 0, Diagnostics.TraceEventType.Error, LogTitle, LogProperties);
                        Session["ErrorMessage"] = "No project selected";
                    }
                }
                catch (Exception ex)
                {
                    // Logger.Write("Error computing selected projectname: " + ex.Message, Category, 200, 0, Diagnostics.TraceEventType.Error, LogTitle, LogProperties);
                    Session["ErrorMessage"] = ex.Message;
                }
                if (ProjectName.Length == 0)
                {
                    Response.Redirect("Error.aspx");
                }
                else
                {
                    // Logger.Write("Selected projectname: '" + ProjectName + "'.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, LogProperties);
                    Session["Project"] = ProjectName;
                    Session.Remove("SelectedFilename");
                    Response.Redirect("Translate.aspx");
                }

            }
        }*/
    }

    protected void Save_Click(object sender, System.EventArgs e)
    {
        /*  const string LogTitle = "Save";
          Dictionary<string, object> LogProperties = default(Dictionary<string, object>);
          // Logger.Write(LogTitle + " -> Start.", Category, 10, 0, Diagnostics.TraceEventType.Start, LogTitle);
          if (Session["User"] == null)
          {
              // Logger.Write("No Session.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle);
              Session["ErrorMessage"] = "Session expired. Please login!";
              Response.Redirect("Error.aspx");
          }
          if (Convert.ToString(Session["User"]).Length == 0)
          {
              // Logger.Write("Session expired.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle);
              Session["ErrorMessage"] = "Session expired. Please login!";
              Response.Redirect("Error.aspx");
          }
          else
          {
              LogProperties = General.CreateLogProperties(Convert.ToString(Session["User"]));


              RepeaterItem Item = default(RepeaterItem);
              TextBox TB = default(TextBox);
              Label LB = default(Label);
              System.Xml.XmlDocument TranslatedFile = default(System.Xml.XmlDocument);
              System.Xml.XmlDocument EnglishFile = default(System.Xml.XmlDocument);
              string Project = Convert.ToString(Session["Project"]);
              if (Project == null)
              {
                  // Logger.Write("Could not find Sessionvalue for selected Project!", Category, 200, 0, Diagnostics.TraceEventType.Error, LogTitle, LogProperties);
                  Session["ErrorMessage"] = "Session expired. Could not read Project. Please login!";
                  Response.Redirect("Error.aspx");
              }
              string Languages = Convert.ToString(Session["Languages"]);
              if (Languages == null)
              {
                  // Logger.Write("Could not find Sessionvalue for selected Language!", Category, 200, 0, Diagnostics.TraceEventType.Error, LogTitle, LogProperties);
                  Session["ErrorMessage"] = "Session expired. Could not read Language. Please login!";
                  Response.Redirect("Error.aspx");
              }
              string ElementName = null;
              string NewValue = null;
              string CurrentValue = null;
              System.Xml.XmlNode Node = default(System.Xml.XmlNode);
              int Updates = 0;
              string Directory = ConfigurationManager.AppSettings("ProjectDirectory").ToString + Project + "\\";

              EnglishFile = XMLFile.GetXMLDocument(Directory + Convert.ToString(Session["SelectedFilename"]) + ".resx");
              string TargetFilename = Directory + Languages + "\\" + Convert.ToString(Session["SelectedFilename"]) + "." + Languages + ".resx";
              string TargetFileNameForGen = Directory + Languages + "\\Download" + "\\" + Convert.ToString(Session["SelectedFilename"]) + "." + Languages + ".resx";
              string TargetFileNameForGen1 = Directory + Languages + "\\Download" + "\\" + Convert.ToString(Session["SelectedFilename"]) + "." + Languages + ".resources";

              TranslatedFile = XMLFile.GetXMLDocument(TargetFilename);
              string Filename = Convert.ToString(Session["SelectedFilename"]);
              // Logger.Write("Checking for changes in '" + Filename + "'...", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, LogProperties);
              foreach (Item in TextElements.Items)
              {
                  LB = Item.FindControl("Element");
                  TB = Item.FindControl("TranslatedText");
                  ElementName = General.HTMLDecode(LB.Text);
                  Node = TranslatedFile.SelectSingleNode("/root/data[@name=\"" + ElementName + "\"]/value");
                  if (Node == null)
                  {
                      // Logger.Write("Node '" + ElementName + "' does not exist in translation file yet. Adding...", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, LogProperties);
                      Node = EnglishFile.SelectSingleNode("/root/data[@name=\"" + ElementName + "\"]");
                      System.Xml.XmlNode rootnode = TranslatedFile.SelectSingleNode("/root");
                      System.Xml.XmlNode CopiedNode = TranslatedFile.ImportNode(Node, true);
                      CopiedNode.SelectSingleNode("value").InnerText = TB.Text;

                      rootnode.AppendChild(CopiedNode);
                      Updates += 1;

                      // Logger.Write("Added value to empty node '" + ElementName + "'.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, LogProperties);
                  }
                  else
                  {
                      CurrentValue = Node.InnerText;
                      NewValue = TB.Text;
                      if (CurrentValue == NewValue)
                      {
                          // Logger.Write("Value of '" + ElementName + "' did not change.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, LogProperties);
                      }
                      else
                      {
                          // Logger.Write("Updated value of '" + ElementName + "'.", Category, 100, 0, Diagnostics.TraceEventType.Information, LogTitle, LogProperties);
                          Node.InnerText = NewValue;
                          Updates += 1;
                      }
                  }
              }

              try
              {
                  int bereitstranslated = Convert.ToInt32(XMLFile.GetTranslatedStrings(Convert.ToString(Session["User"])));
                  int newtrans = Updates;
                  XMLFile.UpdateTLStrings(Convert.ToString(Session["User"]), (bereitstranslated + newtrans));
              }
              catch (Exception ex)
              {
                  // Logger.Write("Can't load or save trnsl. strings\\r\\nUser:" + Convert.ToString(Session["User"]) + ex.Message, Category, 200, 0, Diagnostics.TraceEventType.Error, LogTitle, LogProperties);
              }

              if (Updates == 0)
              {
                  // Logger.Write("No updates made. No need to save '" + TargetFilename + "'.", Category, 100, 0, Diagnostics.TraceEventType.Information, LogTitle, LogProperties);
                  Session["GlobalMessage"] = "No updates made. No need to save file.";
                  XMLFile.ComputePercentage(Project, Languages, Convert.ToString(Session["SelectedFilename"]), LogProperties);
              }
              else
              {
                  // Logger.Write(Updates.ToString + " updates made. Saving file '" + TargetFilename + "'.", Category, 100, 0, Diagnostics.TraceEventType.Information, LogTitle, LogProperties);

                  TranslatedFile.Save(TargetFilename);
                  // Logger.Write("File saved successfully.", Category, 100, 0, Diagnostics.TraceEventType.Information, LogTitle, LogProperties);
                  if (General.CreateBackup(TranslatedFile, TargetFilename, LogProperties))
                  {
                      // Logger.Write("Created backup file successfully.", Category, 100, 0, Diagnostics.TraceEventType.Information, LogTitle, LogProperties);
                  }
                  else
                  {
                      // Logger.Write("Backup file was not created! Please check.", Category, 200, 0, Diagnostics.TraceEventType.Warning, LogTitle, LogProperties);
                  }
                  XMLFile.ComputePercentage(Project, Languages, Convert.ToString(Session["SelectedFilename"]), LogProperties);
                  // Logger.Write("Updated Percentages of '" + Convert.ToString(Session["SelectedFilename"]) + "'. ", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, LogProperties);
                  Session["GlobalMessage"] = "File '" + TargetFilename.Split("\\".ToCharArray)(TargetFilename.Split("\\".ToCharArray).GetUpperBound(0)) + "' saved sucessfully!";
                  if (General.FTPUploadEnabled(Project, LogProperties))
                  {
                      try
                      {
                          General.FTPUpload(Project, TargetFilename, LogProperties);
                          // Logger.Write("FTP Uploade successfull.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, LogProperties);
                      }
                      catch (Exception ex)
                      {
                          // Logger.Write("Error while FTP Upload: " + ex.Message, Category, 300, 0, Diagnostics.TraceEventType.Error, LogTitle, LogProperties);
                          Session["GlobalMessage"] += "Error while uploading file via FTP!";
                      }
                  }

                  TranslatedFile.Save(TargetFileNameForGen);

                  Response.Redirect("Translate.aspx");
                  Session.Remove("SelectedFilename");
                  // Logger.Write("Removed Session 'SelectedFilename'.", Category, 10, 0, Diagnostics.TraceEventType.Verbose, LogTitle, LogProperties);

              }


          }
          */
    }

    /// <summary>
    /// Colorize Background to differ between translated and not-translated text easily
    /// </summary>
    /// <param name="translationText"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public string IsTranslatedCSS(object translationText)
    {
        if (Convert.ToString(translationText) == string.Empty)
        {
            return "nottranslated";
        }
        else
        {
            return "translated";
        }
    }
    public _Translate()
    {
        Load += Page_Load;
    }


}

//=======================================================
//Service provided by Telerik (www.telerik.com)
//Conversion powered by NRefactory.
//Twitter: @telerik
//Facebook: facebook.com/telerik
//=======================================================

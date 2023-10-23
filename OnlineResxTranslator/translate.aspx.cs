using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using Identity;

public partial class _Translate : PageBase
{
    public static List<ProjectHelper.ProjectInfo> projects = ProjectHelper.getProjects();

    protected override void Page_LoadBegin(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (!User.Identity.IsAuthenticated)
                showError("You are not logged in. Please login!", "/Account/Login", "/translate");
            else if (User.Identity.getUserProjects().Count == 0)
                showError("You are not registered for any project."); return;
        }
    }

    protected override void OnPreRenderComplete(EventArgs e)
    {
        if ((!Page.IsPostBack || Page.Request?.Form?["__EVENTTARGET"]?.Contains("rpt_languages") == true) && Session["CurrentlyChosenProject"] != null)
            initTranslationTable();

        base.OnPreRenderComplete(e);
    }

    protected void initTranslationTable()
    {
        // if none was chosen, on site.master.cs the first one will be selected as default
        ProjectHelper.ProjectInfo Project = (ProjectHelper.ProjectInfo)Session["CurrentlyChosenProject"];
        string ProjDirectory = ConfigurationManager.AppSettings["ProjectDirectory"].ToString() + Project.Folder + "\\";
        string Language = User.Identity.getUserLanguage(Session);
        string SourceLang = User.Identity.GetSourceLanguage();

        SelectedProject.Text = Project.Name + " files";

        // Getting Directory + Language + ".xml" - if it doesn't exist, it will automatically be created on percentage calculation
        if (!File.Exists(ProjDirectory + Language + ".xml"))
            XMLFile.ComputePercentage(Project, Language, null, SourceLang);

        if (!File.Exists(ProjDirectory + Language + ".xml"))
        {
            showError("Language file for '" + Language + "' could not be created!"); return;
        }
        else
        {
            XmlDocument LanguageXML = XMLFile.GetXMLDocument(ProjDirectory + Language + ".xml");

            XmlNodeList AllFiles = LanguageXML.SelectNodes("/files[@language=\"" + Language + "\"]/file");

            DataSet oDs = new DataSet();
            oDs.ReadXml(ProjDirectory + Language + ".xml");

            if (oDs.Tables.Count >= 2)
                FileList.DataSource = oDs.Tables[1];

            FileList.DataBind();
        }

        //Check if a file is selected
        if (Session["SelectedFilename"] != null)
        {
            CurrentFile.Text = "Selected file: " + Convert.ToString(Session["SelectedFilename"]);
            Save.Visible = true;

            XmlDocument SourceFile;

            if (Directory.Exists(Path.Combine(ProjDirectory, SourceLang)))
                SourceFile = XMLFile.GetXMLDocument(Path.Combine(ProjDirectory, SourceLang, Convert.ToString(Session["SelectedFilename"]) + "." + SourceLang + ".resx"));
            else
                SourceFile = XMLFile.GetXMLDocument(ProjDirectory + Convert.ToString(Session["SelectedFilename"]) + ".resx");

            if (SourceFile == null)
            {
                Session["SelectedFilename"] = null;
                Response.Redirect("/Account/Default.aspx"); // redirect to homepage, as this selected file does not longer seem to exist
                return;
            }

            XmlDocument TranslatedFile = XMLFile.GetXMLDocument(ProjDirectory + Language + "\\" + Convert.ToString(Session["SelectedFilename"]) + "." + Language + ".resx");
            DataTable Table = new DataTable();
            Table.Columns.Add("TextName");
            Table.Columns.Add("English");
            Table.Columns.Add("Translation");
            Table.Columns.Add("Comment");

            foreach (XmlNode Text in SourceFile.SelectNodes("/root/data"))
            {
                DataRow Row = Table.NewRow();
                Row["TextName"] = Text.Attributes["name"].InnerText;
                Row["English"] = Server.HtmlEncode(Text.SelectSingleNode("value")?.InnerText);
                Row["Comment"] = TranslatedFile?.SelectSingleNode("/root/data[@name=\"" + Row["Textname"].ToString() + "\"]/comment")?.InnerText ?? "";

                XmlNode Translated = TranslatedFile?.SelectSingleNode("/root/data[@name=\"" + Row["Textname"].ToString() + "\"]/value");
                if (Translated == null)
                {
                    Row["Translation"] = string.Empty;
                }
                else
                {
                    Row["Translation"] = Server.HtmlEncode(Translated.InnerText);
                }

                bool CanBeAdded = true;

                foreach (String notToCheck in XMLFile.NotArgs)
                    if (Row["TextName"].ToString().Contains("." + notToCheck)
                        || String.IsNullOrEmpty(Row["English"].ToString())) CanBeAdded = false;

                if (CanBeAdded && (!cb_showOnlyUntr.Checked || String.IsNullOrEmpty(Row["Translation"].ToString())))
                    Table.Rows.Add(Row);
            }
            TextElements.DataSource = Table;
            TextElements.DataBind();
            Save.Visible = true;
        }
        else
            Save.Visible = false;
    }

    protected void FileList_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (!User.Identity.IsAuthenticated)
        {
            showError("You are not logged in. Please login!", "/Account/Login", "/translate");
        }
        else
        {
            string Filename = null;
            Filename = FileList.DataKeys[Convert.ToInt32(e.CommandArgument)].Value.ToString();

            if (Filename.Length == 0)
            {
                // No filename selected
                showError("You didn't select any filename."); return;
            }
            else
            {
                Session["SelectedFilename"] = Filename;
                XMLFile.ComputePercentage((ProjectHelper.ProjectInfo)Session["CurrentlyChosenProject"], User.Identity.getUserLanguage(Session), Convert.ToString(Session["SelectedFilename"]), User.Identity.GetSourceLanguage());
                initTranslationTable();
            }
        }
    }

    protected void cb_showOnlyUntr_CheckedChanged(object sender, EventArgs e)
    {
        initTranslationTable();
    }

    protected void Save_Click(object sender, System.EventArgs e)
    {
        if (!User.Identity.IsAuthenticated)
        {
            showError("You are not logged in. Please login!", "/Account/Login", "/translate");
        }
        else
        {
            ProjectHelper.ProjectInfo Project = (ProjectHelper.ProjectInfo)Session["CurrentlyChosenProject"];
            string Language = User.Identity.getUserLanguage(Session);
            string ProjectDirectory = ConfigurationManager.AppSettings["ProjectDirectory"].ToString() + Project.Folder + "\\";
            string SourceLang = User.Identity.GetSourceLanguage();

            if (Project == null || Language == null)
            {
                showError("Session expired. Could not read Project or Language. Please login!", "/Account/Login", "/translate");
                // TODO: Save Values here for relog (like HttpCookie myCookie = new HttpCookie("savedValues");)
            }

            int Updates = 0;

            XmlDocument SourceFile;

            if (Directory.Exists(Path.Combine(ProjectDirectory, SourceLang)))
                SourceFile = XMLFile.GetXMLDocument(Path.Combine(ProjectDirectory, SourceLang, Convert.ToString(Session["SelectedFilename"]) + "." + SourceLang + ".resx"));
            else
                SourceFile = XMLFile.GetXMLDocument(ProjectDirectory + Convert.ToString(Session["SelectedFilename"]) + ".resx");

            string TargetFilename = ProjectDirectory + Language + "\\" + Convert.ToString(Session["SelectedFilename"]) + "." + Language + ".resx";
            string TargetFileNameForGen = ProjectDirectory + Language + "\\Download" + "\\" + Convert.ToString(Session["SelectedFilename"]) + "." + Language + ".resx";

            // if download directory does not exist, create it
            if (!Directory.Exists(ProjectDirectory + Language + "\\Download"))
                Directory.CreateDirectory(ProjectDirectory + Language + "\\Download");

            XmlDocument TranslatedFile = XMLFile.GetXMLDocument(TargetFilename);
            string Filename = Convert.ToString(Session["SelectedFilename"]);

            foreach (RepeaterItem Item in TextElements.Items)
            {
                Label LB = Item.FindControl("Element") as Label;
                TextBox TB = Item.FindControl("TranslatedText") as TextBox;
                string TComment = (Item.FindControl("TranslateComment") as TextBox).Text;
                string ElementName = HttpUtility.HtmlDecode(LB.Text);

                XmlNode Node = TranslatedFile.SelectSingleNode("/root/data[@name=\"" + ElementName + "\"]/value");

                // Node does not exist in translation file, so add it
                if (Node == null)
                {
                    Node = SourceFile.SelectSingleNode("/root/data[@name=\"" + ElementName + "\"]");
                    XmlNode rootnode = TranslatedFile.SelectSingleNode("/root");
                    XmlNode CopiedNode = TranslatedFile.ImportNode(Node, true);
                    CopiedNode.SelectSingleNode("value").InnerText = TB.Text;

                    rootnode.AppendChild(CopiedNode);
                    Updates += 1;
                }
                else
                {
                    string CurrentValue = Node.InnerText;
                    string NewValue = TB.Text;

                    // if value changed..
                    if (CurrentValue != NewValue)
                    {
                        Node.InnerText = NewValue;
                        Updates += 1;
                    }
                }

                XmlNode CommentNode = TranslatedFile.SelectSingleNode("/root/data[@name=\"" + ElementName + "\"]/comment");
                // Node does not exist in translation file, so add it
                if (CommentNode == null)
                {
                    CommentNode = TranslatedFile.SelectSingleNode("/root/data[@name=\"" + ElementName + "\"]");
                    XmlNode rootnode = TranslatedFile.SelectSingleNode("/root");

                    //Create a new comment node.
                    XmlElement elem = TranslatedFile.CreateElement("comment");
                    elem.InnerText = TComment;
                    CommentNode.AppendChild(elem);

                    rootnode.AppendChild(CommentNode);
                    Updates += 1;
                }
                else
                {
                    string CurrentValue = CommentNode.InnerText;

                    // if value changed..
                    if (CurrentValue != TComment)
                    {
                        Updates += 1;
                        CommentNode.InnerText = TComment;
                    }
                }
            }

            int alreadyTranslated = Convert.ToInt32(User.Identity.GetTranslatedStrings());
            int updateCount = Updates;

            User.Identity.SetTranslatedStrings(alreadyTranslated + updateCount);

            // No updates made. No need to save file, but recalculate the percentage in case the file changed externally
            if (Updates == 0)
                XMLFile.ComputePercentage(Project, Language, Convert.ToString(Session["SelectedFilename"]), SourceLang);
            else
            {
                TranslatedFile.Save(TargetFilename);

                Utils.CreateBackup(TranslatedFile, TargetFilename);

                XMLFile.ComputePercentage(Project, Language, Convert.ToString(Session["SelectedFilename"]), SourceLang);

                //Session["GlobalMessage"] = "File '" + TargetFilename.Split("\\".ToCharArray())[TargetFilename.Split("\\".ToCharArray()).GetUpperBound(0)] + "' saved sucessfully!";

                TranslatedFile.Save(TargetFileNameForGen);

                if (ProjectHelper.FTPUploadEnabled(Project))
                    try
                    {
                        ProjectHelper.FTPUpload(Project, TargetFilename, Language);
                    }
                    catch (Exception ex)
                    {
                        showError("Error: " + ex.Message);
                    }

                Response.Redirect("Translate.aspx");
                Session.Remove("SelectedFilename");
            }
        }
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
            return "warning";
        }
        else
        {
            return "success";
        }
    }

    public _Translate()
    {
        Load += Page_Load;
    }
}
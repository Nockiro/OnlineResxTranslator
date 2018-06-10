using ASP;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

public partial class Admin_Info : PageBase
{

    private SQLHelper sqlhelper = new SQLHelper();

    public static DataTable projectList = new DataTable();
    public DataTable userList = new DataTable();
    public DataTable ftpList = new DataTable();

    /// <summary>
    /// Save the ID a new entry would get so we can handle it separately later
    /// </summary>
    public int newIDif = 0;

    protected override void Page_LoadBegin(object sender, EventArgs e)
    {
        if (!User.Identity.IsAuthenticated || !User.IsInRole("admin"))
        {
            Response.Redirect("/Account/Login.aspx?ReturnUrl=/Admin/Info&re=403");
        }

        if (!IsPostBack)
        {
            ShowUserData();
            ShowProjectData();
            ShowFTPData();
        }
    }

    protected void ShowUserData()
    {
        sqlhelper.OpenConnection();

        if (!sqlhelper.connectionOpen)
            return;

        userList = sqlhelper.SelectFromTables("AspNetUsers", "Left Join TrUserLanguages ON TrUserLanguages.UserID = AspNetUsers.id",
            "AspNetUsers.id as UserID",
            "AspNetUsers.UserName as UserName",
            "AspNetUsers.Email as UserMail",
            "AspNetUsers.DefaultLanguage as UserDefaultLanguage",
            "AspNetUsers.SourceLanguage as UserSourceLanguage",
            "TrUserLanguages.Language as UserLanguages",

            @"UserProjects = STUFF((

                SELECT ', ' + TrProjects.project
                FROM TrProjects, TrUserProjects
                WHERE TrUserProjects.UserID = AspNetUsers.id AND TrUserProjects.ProjID = TrProjects.id

            FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '')");

        gvUsers.DataSource = userList;
        gvUsers.DataBind();

        sqlhelper.CloseConnection();
    }

    protected void gvUsers_RowEditing(object sender, GridViewEditEventArgs e)
    {
        // NewEditIndex property used to determine the index of the row being edited.  
        gvUsers.EditIndex = e.NewEditIndex;

        ShowUserData();
    }

    protected void gvUsers_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        // Finding the controls from Gridview for the row which is going to update  
        String id = (gvUsers.Rows[e.RowIndex].FindControl("lbl_UserID") as Label).Text;
        String userName = (gvUsers.Rows[e.RowIndex].FindControl("tb_UserName") as TextBox).Text;
        String userMail = (gvUsers.Rows[e.RowIndex].FindControl("tb_UserMail") as TextBox).Text;
        String userProjects = (gvUsers.Rows[e.RowIndex].FindControl("tb_Projects") as TextBox).Text;
        String userLanguage = (gvUsers.Rows[e.RowIndex].FindControl("tb_UserLang") as TextBox).Text;
        String userDefaultLanguage = (gvUsers.Rows[e.RowIndex].FindControl("tb_UserDefLang") as TextBox).Text;
        String userSourceLanguage = (gvUsers.Rows[e.RowIndex].FindControl("tb_UserSrcLang") as TextBox).Text;

        sqlhelper.OpenConnection();
        // update user table
        sqlhelper.UpdateTable("AspNetUsers", "id = '" + id + "'",
         new KeyValuePair<string, string>("UserName", userName),
         new KeyValuePair<string, string>("Email", userMail),
         new KeyValuePair<string, string>("DefaultLanguage", userDefaultLanguage),
         new KeyValuePair<string, string>("SourceLanguage", userSourceLanguage));

        // update language table
        sqlhelper.UpdateOrInsertIntoTable("TrUserLanguages", new KeyValuePair<string, string>("UserID", id),
         new KeyValuePair<string, string>("Language", userLanguage));

        // update project table
        // its faster code to delete all projects of that user and readd the ones given in the list, really.
        sqlhelper.DeleteRow("TrUserProjects", "UserID = '" + id + "'");
        if (userProjects != "")
            foreach (string proj in userProjects.Split(','))
            {
                string projID = ((projectList.AsEnumerable().Where(r => r.Field<string>("project") == proj.Trim())).FirstOrDefault()["id"]).ToString();
                sqlhelper.InsertIntoTable("TrUserProjects", new KeyValuePair<string, string>("UserID", id), new KeyValuePair<string, string>("ProjID", projID));
            }

        sqlhelper.CloseConnection();


        // Setting the EditIndex property to -1 to cancel the Edit mode in Gridview  
        gvUsers.EditIndex = -1;

        // Update User Data
        ShowUserData();
        UpdateUserPanel.Update();
    }


    protected void gvUsers_RowCancelingEdit(object sender, System.Web.UI.WebControls.GridViewCancelEditEventArgs e)
    {
        //Setting the EditIndex property to -1 to cancel the Edit mode in Gridview  
        gvUsers.EditIndex = -1;
        ShowUserData();
    }

    protected void gvUsers_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        String id = (gvUsers.Rows[e.RowIndex].FindControl("lbl_UserID") as Label).Text;

        sqlhelper.OpenConnection();
        sqlhelper.DeleteRow("AspNetUsers", "ID = '" + id + "'");
        sqlhelper.CloseConnection();

        ShowUserData();
    }
    #region Projects
    protected void ShowProjectData()
    {
        sqlhelper.OpenConnection();

        if (!sqlhelper.connectionOpen)
            return;

        projectList = sqlhelper.SelectFromTable("TrProjects", "id", "project", "folder",
                            @"ftps = STUFF((

                                SELECT CONVERT(NVARCHAR, ', ')  + CONVERT(NVARCHAR,TrProjectFTPTargets.TargetID)
								 
                                FROM TrProjectFTPTargets
                                WHERE TrProjects.id = TrProjectFTPTargets.ProjID 

                                FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '')");

        if (projectList.Rows.Count > 0)
            newIDif = (int)projectList.Rows[projectList.Rows.Count - 1]["id"];

        // get ID for a possible new row we are preparing here
        newIDif = (newIDif > projectList.Rows.Count) ? newIDif + 1 : projectList.Rows.Count;


        int newRowIndex = projectList.Rows.IndexOf(projectList.Rows.Add(new object[] { newIDif, "", "" }));

        // if currently there is no row being edited, set the "new" possible row in edit state
        if (gvProjects.EditIndex == -1)
            gvProjects.EditIndex = newRowIndex;

        gvProjects.DataSource = projectList;
        gvProjects.DataBind();

        sqlhelper.CloseConnection();
    }

    protected void gvProjects_RowEditing(object sender, GridViewEditEventArgs e)
    {
        // NewEditIndex property used to determine the index of the row being edited.  
        gvProjects.EditIndex = e.NewEditIndex;
        ShowProjectData();
    }

    protected void gvProjects_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        // Finding the controls from Gridview for the row which is going to update  
        Int32 id = Convert.ToInt32((gvProjects.Rows[e.RowIndex].FindControl("lbl_ID") as Label).Text);
        String project_name = (gvProjects.Rows[e.RowIndex].FindControl("tb_project") as TextBox).Text;
        String project_folder = (gvProjects.Rows[e.RowIndex].FindControl("tb_folder") as TextBox).Text;
        String ftp = (gvProjects.Rows[e.RowIndex].FindControl("tb_ftps") as TextBox).Text;

        sqlhelper.OpenConnection();

        // if it's a new entry - check here for the id given in the table since we can't be sure if the newID variable is still correct
        if (id == Convert.ToInt32((gvProjects.Rows[gvProjects.Rows.Count - 1].FindControl("lbl_ID") as Label).Text))
        {
            sqlhelper.InsertIntoTable("TrProjects", new KeyValuePair<string, string>("project", project_name),
            new KeyValuePair<string, string>("folder", project_folder));

        }
        else
        { // otherwise update it
            sqlhelper.UpdateTable("TrProjects", "id = " + id,
            new KeyValuePair<string, string>("project", project_name),
            new KeyValuePair<string, string>("folder", project_folder));
        }

        // its faster code to delete all targets of that projecs and read the ones given in the list
        sqlhelper.DeleteRow("TrProjectFTPTargets", "ProjID = " + id);
        // update ftp target assignment table
        if (ftp != "")
            foreach (string ftpTarget in ftp.Split(','))
            {
                // check if entered number is actually valid number
                if (int.TryParse(ftpTarget, out int ftpTargetID))
                    sqlhelper.InsertIntoTable("TrProjectFTPTargets", new KeyValuePair<string, string>("TargetID", ftpTargetID.ToString()), new KeyValuePair<string, string>("ProjID", id.ToString()));
            }

        sqlhelper.CloseConnection();

        // Setting the EditIndex property to -1 to cancel the Edit mode in Gridview  
        gvProjects.EditIndex = -1;

        // Call ShowData method for displaying updated data  
        ShowProjectData();
        // Update User Data too since the project names could have changed
        ShowUserData();
        UpdateUserPanel.Update();
    }

    protected void gvProjects_RowCancelingEdit(object sender, System.Web.UI.WebControls.GridViewCancelEditEventArgs e)
    {
        //Setting the EditIndex property to -1 to cancel the Edit mode in Gridview  
        gvProjects.EditIndex = -1;
        ShowProjectData();
    }

    protected void gvProjects_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Int32 id = Convert.ToInt32((gvProjects.Rows[e.RowIndex].FindControl("lbl_ID") as Label).Text);

        sqlhelper.OpenConnection();
        sqlhelper.DeleteRow("TrProjects", "ID = " + id);
        sqlhelper.CloseConnection();

        ShowProjectData();
    }
    #endregion
    #region FTP
    protected void ShowFTPData()
    {
        sqlhelper.OpenConnection();

        if (!sqlhelper.connectionOpen)
            return;

        ftpList = sqlhelper.SelectFromTable("TrFTPTargets", "id", "server", "username", "password", "path", "ssl");

        if (ftpList.Rows.Count > 0)
            newIDif = (int)ftpList.Rows[ftpList.Rows.Count - 1]["id"];

        // get ID for a possible new row we are preparing here
        newIDif = (newIDif > ftpList.Rows.Count) ? newIDif + 1 : ftpList.Rows.Count;


        int newRowIndex = ftpList.Rows.IndexOf(ftpList.Rows.Add(new object[] { newIDif, "", "" }));

        // if currently there is no row being edited, set the "new" possible row in edit state
        if (gvFtps.EditIndex == -1)
            gvFtps.EditIndex = newRowIndex;

        gvFtps.DataSource = ftpList;
        gvFtps.DataBind();

        sqlhelper.CloseConnection();
    }

    protected void gvFtps_RowEditing(object sender, GridViewEditEventArgs e)
    {
        // NewEditIndex property used to determine the index of the row being edited.  
        gvFtps.EditIndex = e.NewEditIndex;
        ShowFTPData();
    }

    protected void gvFtps_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        // Finding the controls from Gridview for the row which is going to update  
        Int32 id = Convert.ToInt32((gvFtps.Rows[e.RowIndex].FindControl("lbl_FTPID") as Label).Text);
        String server_name = (gvFtps.Rows[e.RowIndex].FindControl("tb_server") as TextBox).Text;
        String username = (gvFtps.Rows[e.RowIndex].FindControl("tb_user") as TextBox).Text;
        String password = (gvFtps.Rows[e.RowIndex].FindControl("tb_pass") as TextBox).Text;
        String path = (gvFtps.Rows[e.RowIndex].FindControl("tb_path") as TextBox).Text;
        Boolean ssl = (gvFtps.Rows[e.RowIndex].FindControl("cb_ssl") as CheckBox).Checked;

        sqlhelper.OpenConnection();

        // if it's a new entry - check here for the id given in the table since we can't be sure if the newID variable is still correct
        if (id == Convert.ToInt32((gvFtps.Rows[gvFtps.Rows.Count - 1].FindControl("lbl_FTPID") as Label).Text))
        {
            sqlhelper.InsertIntoTable("TrFTPTargets",
                new KeyValuePair<string, string>("server", server_name),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("path", path),
                new KeyValuePair<string, string>("ssl", ssl ? "1" : "0")
            );

        }
        else
        {
            List<KeyValuePair<string, string>> valuePairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("server", server_name),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("path", path),
                new KeyValuePair<string, string>("ssl", ssl ? "1" : "0")
            };
            if (password != "")
                valuePairs.Add(new KeyValuePair<string, string>("password", password.ToString()));

            // otherwise update it
            sqlhelper.UpdateTable("TrFTPTargets", "id = " + id, valuePairs.ToArray());
        }


        sqlhelper.CloseConnection();

        // gvFtps the EditIndex property to -1 to cancel the Edit mode in Gridview  
        gvFtps.EditIndex = -1;

        // Call ShowData method for displaying updated data  
        ShowFTPData();
    }

    protected void gvFtps_RowCancelingEdit(object sender, System.Web.UI.WebControls.GridViewCancelEditEventArgs e)
    {
        //Setting the EditIndex property to -1 to cancel the Edit mode in Gridview  
        gvFtps.EditIndex = -1;
        ShowFTPData();
    }

    protected void gvFtps_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Int32 id = Convert.ToInt32((gvFtps.Rows[e.RowIndex].FindControl("lbl_FTPID") as Label).Text);

        sqlhelper.OpenConnection();
        sqlhelper.DeleteRow("TrFTPTargets", "id = " + id);
        sqlhelper.CloseConnection();

        ShowFTPData();
    }
    #endregion

    public List<System.Security.Principal.IPrincipal> getOnlineUsers()
    {
        return global_asax.Sessions;
    }

}
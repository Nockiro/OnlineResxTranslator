using ASP;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

public partial class Admin_Info : PageBase {

    private SQLHelper sqlhelper = new SQLHelper();

    public static DataTable projectList = new DataTable();
    public DataTable userList = new DataTable();

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
            ShowProjectData();
            ShowUserData();
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
            "TrUserLanguages.Language as UserLanguage",

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

        sqlhelper.OpenConnection();
        // update user table
        sqlhelper.UpdateTable("AspNetUsers", "id = '" + id + "'",
         new KeyValuePair<string, string>("UserName", userName),
         new KeyValuePair<string, string>("Email", userMail));
        
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

        projectList = sqlhelper.SelectFromTable("TrProjects", "id", "project", "folder");

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

    public List<System.Security.Principal.IPrincipal> getOnlineUsers()
    {
        return global_asax.Sessions;
    }

}
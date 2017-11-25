using ASP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

public partial class Admin_Info : System.Web.UI.Page {

    private SQLHelper sqlhelper = new SQLHelper();

    public DataTable projectList = new DataTable();

    /// <summary>
    /// Save the ID a new entry would get so we can handle it separately later
    /// </summary>
    public int newIDif = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!User.Identity.IsAuthenticated || !User.IsInRole("admin"))
        {
            Response.Redirect("/Account/Login.aspx?ReturnUrl=/Admin/Info&re=403");
        }

        if (!IsPostBack)
        {
            ShowData();
        }
    }

    protected void ShowData()
    {
        sqlhelper.OpenConnection();

        if (!sqlhelper.connectionOpen)
            return;
        
        projectList = sqlhelper.SelectFromTable("TrProjects", "id", "project", "folder");

        if (projectList.Rows.Count > 0)
            newIDif = (int)projectList.Rows[projectList.Rows.Count - 1]["id"];

        // get ID for a possible new row we are preparing here
        newIDif = (newIDif > projectList.Rows.Count) ? newIDif + 1 : projectList.Rows.Count;

        System.Diagnostics.Debug.WriteLine("Current EditIndex: " + GridView1.EditIndex + ", Rows:" + projectList.Rows.Count);

        int newRowIndex = projectList.Rows.IndexOf(projectList.Rows.Add(new object[] { newIDif, "", "" }));

        // if currently there is no row being edited, set the "new" possible row in edit state
        if (GridView1.EditIndex == -1)
            GridView1.EditIndex = newRowIndex;

        GridView1.DataSource = projectList;
        GridView1.DataBind();

        sqlhelper.CloseConnection();
    }

    protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
    {
        //NewEditIndex property used to determine the index of the row being edited.  
        GridView1.EditIndex = e.NewEditIndex;
        ShowData();
    }
    protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        // Finding the controls from Gridview for the row which is going to update  
        Int32 id = Convert.ToInt32((GridView1.Rows[e.RowIndex].FindControl("lbl_ID") as Label).Text);
        String project_name = (GridView1.Rows[e.RowIndex].FindControl("tb_project") as TextBox).Text;
        String project_folder = (GridView1.Rows[e.RowIndex].FindControl("tb_folder") as TextBox).Text;

        sqlhelper.OpenConnection();

        // if it's a new entry - check here for the id given in the table since we can't be sure if the newID variable is still correct
        if (id == Convert.ToInt32((GridView1.Rows[GridView1.Rows.Count - 1].FindControl("lbl_ID") as Label).Text))
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

        //Setting the EditIndex property to -1 to cancel the Edit mode in Gridview  
        GridView1.EditIndex = -1;

        //Call ShowData method for displaying updated data  
        ShowData();
    }
    protected void GridView1_RowCancelingEdit(object sender, System.Web.UI.WebControls.GridViewCancelEditEventArgs e)
    {
        //Setting the EditIndex property to -1 to cancel the Edit mode in Gridview  
        GridView1.EditIndex = -1;
        ShowData();
    }

    protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Int32 id = Convert.ToInt32((GridView1.Rows[e.RowIndex].FindControl("lbl_ID") as Label).Text);

        sqlhelper.OpenConnection();
        sqlhelper.DeleteRow("TrProjects", "ID = " + id);
        sqlhelper.CloseConnection();

        ShowData();
    }

    public List<System.Security.Principal.IPrincipal> getOnlineUsers()
    {
        return global_asax.Sessions;
    }

}
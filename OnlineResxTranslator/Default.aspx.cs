using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : Page {

    [Flags]
    enum TranslationSummaryCategory {
        Uncompleted = 1,
        Completed
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            updateData(TranslationSummaryCategory.Uncompleted | TranslationSummaryCategory.Completed);
        }
    }



    public void recalculatePoints_Click(Object sender, CommandEventArgs e)
    {
        if (e.CommandName == "recalcPercentage")
        {
            switch ((string)e.CommandArgument)
            {
                case "Complete":
                    refreshData(TranslationSummaryCategory.Completed);
                    break;
                case "Uncomplete":
                    refreshData(TranslationSummaryCategory.Uncompleted);
                    break;
            }

        }

    }

    private void refreshData(TranslationSummaryCategory tsc)
    {
        if (tsc == TranslationSummaryCategory.Completed)
        {
            // count list items before our new percentage values were calculated
            int prevCount = getItemCount(TranslationSummaryCategory.Completed) == -1 ? (int)ViewState["CompleteCount"] : getItemCount(TranslationSummaryCategory.Completed);

            foreach (XMLFile.ProjectShortSummary psc in XMLFile.ComputeSummary(SiteMaster.CurrentlyChosenProject, 100.0, 100.0))
                XMLFile.ComputePercentage(SiteMaster.CurrentlyChosenProject, psc.LangCode, null);

            updateData(TranslationSummaryCategory.Completed);

            // if list of successful changed, it's likely the other list changed, too. (attention: sometimes, this query may behave weird)
            if (prevCount != getItemCount(TranslationSummaryCategory.Completed))
            {
                // refresh other panels data, too
                refreshData(TranslationSummaryCategory.Uncompleted);
                // Update panel manually since it wouldn't do it if we got called from the other panel
                UpdtPnlForUncPbs.Update();
            }

        }
        else if (tsc == TranslationSummaryCategory.Uncompleted)
        {
            // count list items before our new percentage values were calculated
            int prevCount = getItemCount(TranslationSummaryCategory.Uncompleted) == -1 ? (int)ViewState["UncompleteCount"] : getItemCount(TranslationSummaryCategory.Uncompleted);

            foreach (XMLFile.ProjectShortSummary psc in XMLFile.ComputeSummary(SiteMaster.CurrentlyChosenProject, 100.0, 100.0))
                XMLFile.ComputePercentage(SiteMaster.CurrentlyChosenProject, psc.LangCode, null);

            updateData(TranslationSummaryCategory.Uncompleted);

            // if list of successful changed, it's likely the other list changed, too. (attention: sometimes, this query may behave weird)
            if (prevCount != getItemCount(TranslationSummaryCategory.Uncompleted))
            {
                refreshData(TranslationSummaryCategory.Completed);
                // Update panel manually since it wouldn't do it if we got called from the other panel
                UpdtPnlForPbs.Update();
            }

        }
        return;
    }

    private void updateData(TranslationSummaryCategory tsc)
    {
        if (tsc.HasFlag(TranslationSummaryCategory.Completed))
        {
            SuccessRepeater.DataSource = XMLFile.ComputeSummary(SiteMaster.CurrentlyChosenProject, 100.0, 100.0);
            ViewState["CompleteCount"] = getItemCount(TranslationSummaryCategory.Completed);
            SuccessRepeater.DataBind();
        }

        if (tsc.HasFlag(TranslationSummaryCategory.Uncompleted))
        {
            UncompletedRepeater.DataSource = XMLFile.ComputeSummary(SiteMaster.CurrentlyChosenProject);
            ViewState["UncompleteCount"] = getItemCount(TranslationSummaryCategory.Uncompleted);
            UncompletedRepeater.DataBind();
        }
    }

    private int getItemCount(TranslationSummaryCategory tsc)
    {
        if (tsc == TranslationSummaryCategory.Completed)
            return SuccessRepeater.DataSource != null ? ((List<XMLFile.ProjectShortSummary>)SuccessRepeater.DataSource).Count : -1;
        else
            return UncompletedRepeater.DataSource != null ? ((List<XMLFile.ProjectShortSummary>)UncompletedRepeater.DataSource).Count : -1;

    }
}
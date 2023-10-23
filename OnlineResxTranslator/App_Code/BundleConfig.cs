using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.UI;

namespace localhost
{
    public class BundleConfig
    {
        // Weitere Informationen zur Bündelung finden Sie unter https://go.microsoft.com/fwlink/?LinkID=303951
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/WebFormsJs").Include(
                            "~/Scripts/WebForms/WebForms.js",
                            "~/Scripts/WebForms/WebUIValidation.js",
                            "~/Scripts/WebForms/MenuStandards.js",
                            "~/Scripts/WebForms/Focus.js",
                            "~/Scripts/WebForms/GridView.js",
                            "~/Scripts/WebForms/DetailsView.js",
                            "~/Scripts/WebForms/TreeView.js",
                            "~/Scripts/WebForms/WebParts.js"));

            // Die Reihenfolge ist sehr wichtig, damit diese Dateien funktionieren. Sie weisen ausdrückliche Abhängigkeiten auf.
            bundles.Add(new ScriptBundle("~/bundles/MsAjaxJs").Include(
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjax.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxApplicationServices.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxTimer.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxWebForms.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));

            ScriptManager.ScriptResourceMapping.AddDefinition(
                    "bootstrap",
                    new ScriptResourceDefinition
                    {
                        Path = "~/Scripts/bootstrap.bundle.min.js",
                        DebugPath = "~/Scripts/bootstrap.bundle.js",
                    });

            ScriptManager.ScriptResourceMapping.AddDefinition(
                    "respond",
                    new ScriptResourceDefinition
                    {
                        Path = "~/Scripts/respond.min.js",
                        DebugPath = "~/Scripts/respond.js",
                    });
        }
    }
}
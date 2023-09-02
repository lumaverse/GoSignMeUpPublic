using System.Web;
using System.Web.Optimization;
using Gsmu.Api.Data;

namespace Gsmu.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                //"~/Scripts/jquery-ui-{version}.js",
                "~/Scripts/jquery-ui.js",
                "~/Scripts/jquery.cycle.all.js"
            ));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                "~/Content/themes/base/*.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*", 
                "~/Scripts/jquery.unobtrusive*"
            ));

            bundles.Add(new ScriptBundle("~/bundles/gsmu-base").Include(
                "~/Components/extjs5/examples/ux/GMapPanel.js",
                "~/Components/extjs5.1/examples/ux/form/MultiSelect.js",
                "~/Components/skirtlesden.com/CTemplate.js",
                "~/Components/skirtlesden.com/Component.js",
                "~/Scripts.Gsmu/*.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/gsmu-models").Include(
                "~/Scripts.Gsmu/Models/*.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/gsmu-admin").Include(
                "~/Areas/Adm/Scripts/DataStores/*.js",
                "~/Areas/Adm/Scripts/*.js"
            ));

            bundles.Add(new StyleBundle("~/Areas/Adm/Styles/css").Include(
                "~/Areas/Adm/Styles/*.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/gsmu-public").Include(
                "~/Areas/Public/Scripts/*.js"
            ));

            bundles.Add(new StyleBundle("~/Areas/Public/Styles/css").Include(
                "~/Areas/Public/Styles/*.css"
            ));

            bundles.Add(new StyleBundle("~/Styles/common").Include(
                "~/Styles/*.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/gsmu-public-course").Include(
                "~/Areas/Public/Scripts/Course/*.js"
            ));

            bundles.Add(new StyleBundle("~/Areas/Public/Styles/Course/css").Include(
                "~/Areas/Public/Styles/Course/*.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/gsmu-extjs").Include(
                "~/Scripts.Gsmu/Extjs/*.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/public-supervisor").Include(
                "~/Areas/Public/Scripts/Supervisor/*.js",
                "~/Areas/Public/Scripts/Supervisor/Widgets/*.js"
            ));
            bundles.Add(new ScriptBundle("~/bundles/public-instructor").Include(
                 "~/Areas/Public/Scripts/Instructor/*.js",
                 "~/Areas/Public/Scripts/Instructor/Widgets/*.js",
                 "~/Areas/Public/Scripts/Instructor/Widgets/datastores/*.js"
            ));

            BundleTable.EnableOptimizations = true;
        }
    }
}
using System.Web;
using System.Web.Optimization;


namespace TheatreCMS
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            // Needed for Bootstrap dropdowns and popovers
            bundles.Add(new ScriptBundle("~/bundles/popper").Include(
                    "~/Scripts/umd/popper.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/fullcalendar").Include(
                      "~/Scripts/moment.js",
                      "~/Scripts/fullcalendar/fullcalendar.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/fullcalendar.css",
                      "~/Content/all.min.css",
                      "~/Content/summernote/summernote-bs4.css"));

            //Bundling our site-wide Js file.
            bundles.Add(new ScriptBundle("~/bundles/javascript").Include(
                      "~/Scripts/Site.js"));

        }
    }
}

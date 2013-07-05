using System.Web.Optimization;

namespace TFSteno
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/Scripts/jquery-{version}.js",
                //"~/Scripts/bootstrap.js",
                "~/Scripts/angular.js",
                "~/Scripts/ui-bootstrap-tpls-{version}.js",
                "~/app/signup.js"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/site.css",
                "~/Content/bootstrap-responsive.css"));
        }
    }
}

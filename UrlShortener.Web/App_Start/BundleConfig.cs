using System.Web;
using System.Web.Optimization;

namespace UrlShortener.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;

            bundles.Add(new ScriptBundle("~/bundles/jquery",
                "https://ajax.googleapis.com/ajax/libs/jquery/2.1.4/jquery.min.js")
                .Include("~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap",
                "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.4/js/bootstrap.min.js")
                .Include("~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Content/bootstrap.css",
                    "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/angular",
                "https://ajax.googleapis.com/ajax/libs/angularjs/1.4.1/angular.min.js")
                .Include("~/Scripts/angular.js"));

            bundles.Add(new ScriptBundle("~/bundles/littleUrlApp")
                .Include("~/Scripts/littleUrlApp/*.js"));
        }
    }
}

using System.Web.Optimization;

namespace GamexWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //css for index
            bundles.Add(new StyleBundle("~/Content/indexCss")
                .Include(
//                    "~/Content/login.css",
                "~/Content/bootstrap.css",
                "~/Content/login-form-elements.css",
                "~/Content/index-style.css")
                .Include("~/Content/font-awesome/css/all.css", 
                    new CssRewriteUrlTransform()));


            //js for index
            bundles.Add(new ScriptBundle("~/bundles/indexScript").Include(
                "~/Scripts/jquery.js",
                "~/Scripts/bootstrap.bundle.js",
                "~/Scripts/jquery.easing.js",
                "~/Scripts/jquery.backstretch.js",
                "~/Scripts/jquery.validate*",
               "~/Scripts/scripts-login-register.js"));
        }
    }
}

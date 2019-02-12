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
                "~/Content/bootstrap.css",
                "~/Content/login-form-elements.css",
                "~/Content/index-style.css")
                .Include("~/Content/font-awesome/css/all.css", 
                    new CssRewriteUrlTransform()));

            //css for admin template
            bundles.Add(new StyleBundle("~/Content/adminCss").Include(
                "~/Content/bootstrap.css",
                "~/Content/dataTables.bootstrap4.css",
                "~/Content/main-admin-page.css").Include("~/Content/font-awesome/css/all.css", new CssRewriteUrlTransform()));


            //js for index
            bundles.Add(new ScriptBundle("~/bundles/indexScript").Include(
                "~/Scripts/jquery.js",
                "~/Scripts/bootstrap.bundle.js",
                "~/Scripts/jquery.easing.js",
                "~/Scripts/jquery.backstretch.js",
                "~/Scripts/jquery.validate*",
               "~/Scripts/scripts-login-register.js"));

            bundles.Add(new ScriptBundle("~/bundles/adminScript").Include(
                "~/Scripts/jquery.js",
                "~/Scripts/bootstrap.bundle.js",
                "~/Scripts/jquery.easing.js",
                "~/Scripts/jquery.validate*",
                "~/Scripts/sb-admin.js"));
        }
    }
}

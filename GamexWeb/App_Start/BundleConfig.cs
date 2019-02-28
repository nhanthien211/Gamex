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
                "~/Content/index-style.css",
                "~/Content/login-form-elements.css")
                .Include("~/Content/font-awesome/css/all.css", 
                    new CssRewriteUrlTransform()));

            //css for admin template
            bundles.Add(new StyleBundle("~/Content/adminCss")
                .Include("~/Content/font-awesome/css/all.css", 
                    new CssRewriteUrlTransform())
                .Include(
                "~/Content/bootstrap.css",
                "~/Content/dataTables.bootstrap4.css",
                "~/Content/main-admin-page.css"));


            //js for index
            bundles.Add(new ScriptBundle("~/bundles/indexScript").Include(
                "~/Scripts/jquery.js",
                "~/Scripts/bootstrap.bundle.js",
                "~/Scripts/jquery.easing.js",
                "~/Scripts/jquery.backstretch.js",
                "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/adminScript").Include(
                "~/Scripts/jquery.js",
                "~/Scripts/bootstrap.bundle.js",
                "~/Scripts/jquery.easing.js",
                "~/Scripts/jquery.validate*",
                "~/Scripts/sb.admin.js"));

            bundles.Add(new ScriptBundle("~/bundles/datatableCompanyRequest").Include(
                "~/Scripts/jquery.dataTables.js",
                "~/Scripts/dataTables.bootstrap4.js",
                "~/Scripts/sb.admin.datatables.company.request.js"));

            bundles.Add(new ScriptBundle("~/bundles/datatableCompanyList").Include(
                "~/Scripts/jquery.dataTables.js",
                "~/Scripts/dataTables.bootstrap4.js",
                "~/Scripts/sb.admin.datatables.company.list.js"));

            bundles.Add(new ScriptBundle("~/bundles/datatableOrganizerList").Include(
                "~/Scripts/jquery.dataTables.js",
                "~/Scripts/dataTables.bootstrap4.js",
                "~/Scripts/sb.admin.datatables.organizer.list.js"));

            bundles.Add(new ScriptBundle("~/bundles/datatableJoinEvent").Include(
                "~/Scripts/jquery.dataTables.js",
                "~/Scripts/dataTables.bootstrap4.js",
                "~/Scripts/sb.company.datatables.organizer.list.js"));

            //CKEditor script
            bundles.Add(new ScriptBundle("~/bundles/ckeditor").Include(
                "~/Scripts/ckeditor/ckeditor.js"));

            //Validate File Upload
            bundles.Add(new ScriptBundle("~/bundles/validateFile").Include(
                "~/Scripts/exhibition.validate.js"));
            
        }
    }
}

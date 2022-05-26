using System.Web;
using System.Web.Optimization;

namespace VMS.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/jQueryVal").Include(
                    "~/Scripts/jquery.validate.js",
                    "~/Scripts/jquery.validate.unobtrusive.js",
                    "~/Scripts/jquery.unobtrusive-ajax.js"));


            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      //"~/Scripts/bootstrap.js",
                      "~/Content/bower_components/bootstrap/dist/js/bootstrap.min.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/Jquery").Include(
                "~/Content/bower_components/jquery/dist/jquery.js"
                ));
            bundles.Add(new ScriptBundle("~/DataTablesJS").Include(
                    "~/Content/bower_components/datatables.net/js/jquery.dataTables.js",
                    "~/Content/bower_components/datatables.net-bs/js/dataTables.bootstrap.js",
                    "~/Content/bower_components/datatables.net/FixedColumns/js/dataTables.fixedColumns.js",
                    "~/Content/bower_components/datatables.net/FixedHeader/js/dataTables.fixedHeader.js",
                    "~/Content/bower_components/datatables.net/FixedHeader/js/fixedHeader.bootstrap.js",
                    "~/Content/bower_components/datatables.net/FixedHeader/js/fixedHeader.dataTables.js",
                    "~/Content/bower_components/datatables.net/FixedColumns/js/fixedColumns.dataTables.js",
                    "~/Content/bower_components/datatables.net/FixedColumns/js/fixedColumns.bootstrap.js"));
            bundles.Add(new StyleBundle("~/DataTablesCS").Include(
                    "~/Content/bower_components/datatables.net-bs/css/dataTables.bootstrap.css",
                    "~/Content/bower_components/datatables.net/FixedColumns/css/fixedColumns.bootstrap.css",
                    "~/Content/bower_components/datatables.net/FixedHeader/css/fixedHeader.bootstrap.css",
                    "~/Content/bower_components/datatables.net/FixedHeader/css/fixedHeader.dataTables.css",
                    "~/Content/bower_components/datatables.net/FixedColumns/css/fixedColumns.dataTables.css"));
            bundles.Add(new ScriptBundle("~/Js").Include(
                      "~/Content/bower_components/jquery-slimscroll/jquery.slimscroll.min.js",
                      "~/Content/bower_components/fastclick/lib/fastclick.js",
                      "~/Content/toastr/toastr.min.js",
                      "~/Content/dist/js/adminlte.min.js",
                      "~/Content/dist/js/demo.js",
                      "~/Content/bower_components/bootstrap-toggle/js/bootstrap-toggle.js",
                      "~/Scripts/iCheck/jquery.validate.js",
                      "~/Scripts/prototype.js",
                      "~/Scripts/ShimanoObject.js",
                      "~/Scripts/ShiMessage.js"
                      ));
            bundles.Add(new ScriptBundle("~/kendoJS").Include(
                "~/Scripts/kendo/jszip.min.js",
                "~/Scripts/kendo/kendo.all.min.js",
                "~/Scripts/kendo/kendo.mobile.min.js",
                "~/Scripts/kendo/kendo.web.min.js",
                "~/Scripts/kendo/kendo.aspnetmvc.min.js"
                ));
            bundles.Add(new StyleBundle("~/kendoCSS").Include(
                "~/Content/kendo/kendo.common.css",
                "~/Content/kendo/kendo.mobile.all.css",
                "~/Content/kendo/ShimanoKendo.css"
                ));
            bundles.Add(new ScriptBundle("~/Highchart").Include(
                "~/Content/Highchart/highcharts.js",
                "~/Content/Highchart/highcharts-3d.js",
                "~/Content/Highchart/modules/solid-gauge.js",
                "~/Content/Highchart/modules/data.js",
                "~/Content/Highchart/modules/drilldown.js",
                "~/Content/Highchart/modules/exporting.js"
                ));
            bundles.Add(new ScriptBundle("~/Mousetrap").Include(
                    "~/Scripts/Mousetrap.js"));
            bundles.Add(new ScriptBundle("~/Webcam").Include(
                    "~/Scripts/jquery.webcam.js"));
            bundles.Add(new StyleBundle("~/Css").Include(
                      "~/Content/Site.css",
                      "~/Content/bower_components/bootstrap/dist/css/bootstrap.css",
                      "~/Content/bower_components/Ionicons/css/ionicons.min.css",
                      "~/Content/dist/css/AdminLTE.min.css",
                      "~/Content/dist/css/skins/_all-skins.min.css",
                      "~/Content/toastr/toastr.min.css",
                      "~/Content/bower_components/bootstrap-toggle/css/bootstrap-toggle.css"
                      ).Include("~/Content/bower_components/font-awesome/css/font-awesome.min.css"));
            BundleTable.EnableOptimizations = false;

        }
    }
}

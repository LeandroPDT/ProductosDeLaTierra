using System.Web;
using System.Web.Optimization;

namespace Site { 
    public class BundleConfig {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new ScriptBundle("~/bundles/site").Include("~/Scripts/jquery/jquery-1.8.1.min.js",
                "~/Scripts/jQuery/jquery-ui-1.9.0.min.js",
                "~/Scripts/jquery/ui.datepicker-es.js",
                "~/Scripts/jquery.numericspinnernumpad-min.js",
                "~/Scripts/quicksearch/jquery.quicksearch.js",
                "~/Scripts/tablesorter/jquery.tablesorter.min.js",
                "~/Scripts/qtip/jquery.qtip.min.js",
                "~/Scripts/blockUI/jquery.blockUI.js",
                "~/Scripts/plupload/js/plupload.full.js",
                "~/Scripts/plupload/js/i18n/es.js",
                "~/Scripts/WebUi-PopOver/jquery.webui-popover.js",
                "~/Scripts/toastr/toastr.min.js",
                "~/Scripts/ContextMenu/jquery.ui-contextmenu.min.js",
                "~/Scripts/jquery.hoverIntent.min.js", 
                "~/Scripts/MonthPicker/jquery.ui.monthpicker.js",
                "~/Scripts/Calculadora/jquery.calculadora.js",
                "~/Scripts/jquery-timeago-master/jquery.timeago.js", 
                "~/Scripts/jquery-timeago-master/locales/jquery.timeago.es.js",
                "~/Scripts/Calculation/jquery.calculation.min.js",
                "~/Scripts/StickyTableHeaders/jquery.StickyTableHeaders.js",
                "~/Scripts/HighCharts/highcharts.src.js",
                "~/Scripts/jquery-file-upload/js/vendor/jquery.ui.widget.js",
                "~/Scripts/jquery-file-upload/js/jquery.iframe-transport.js",
                "~/Scripts/jquery-file-upload/js/jquery.fileupload.js",
                "~/Scripts/date-es-AR.js",
                "~/Scripts/site.js",
                "~/Scripts/jspdf.min.js",
                "~/Scripts/html2canvas.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/scripts/jQuery/jquery-ui-1.9.0.custom.css",
                      "~/scripts/qtip/jquery.qtip.min.css",
                      "~/scripts/toastr/toastr.min.css",
                      "~/content/webfont/style.css",
                      "~/Scripts/Calculadora/jquery.calculadora.css",
                      "~/Scripts/WebUi-PopOver/jquery.webui-popover.css",
                      "~/Scripts/jquery-file-upload/css/jquery.fileupload.css",
                      "~/Content/site.css"));

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Optimization;

namespace XWG.Laboratory
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/three/js").Include(
                "~/Scripts/three.ie9.js",
                "~/Scripts/three.js"));

            bundles.Add(new ScriptBundle("~/custom/js").Include(
                "~/Scripts/custom/js.common.js"));

            bundles.Add(new ScriptBundle("~/weixin/api").Include(
                "~/Scripts/weixin/WeixinApi.js"));



            #region jQuery(~/jquery/js)

            bundles.Add(new ScriptBundle("~/jquery/js").Include(
                "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/jquery/js/easing").Include(
                "~/Scripts/jquery.easing.{version}.js"));

            bundles.Add(new ScriptBundle("~/jquery/js/scrollto").Include(
                "~/Scripts/jquery.scrollTo.js"));

            bundles.Add(new ScriptBundle("~/jquery/js/scrollify").Include(
                "~/Scripts/jquery.scrollify.custom.js"));

            bundles.Add(new ScriptBundle("~/jquery/js/fileupload").Include(
                       "~/Scripts/fileupload/jquery.ui.widget.js",
                       "~/Scripts/fileupload/jquery.iframe-transport.js",
                       "~/Scripts/fileupload/jquery.fileupload.js"));

            bundles.Add(new StyleBundle("~/jquery/css/fileupload").Include(
                       "~/Content/jquery.fileupload.css"));

            bundles.Add(new ScriptBundle("~/jquery/js/lazyload")
                .Include("~/Scripts/lazyload/jquery.lazyload.min.js"));

            #endregion



            #region HTML5音频播放(~/buzz/js)

            bundles.Add(new ScriptBundle("~/buzz/js").Include(
                "~/Scripts/buzz.js"));

            #endregion



            #region SignalR

            bundles.Add(new ScriptBundle("~/signalr/js").Include(
                "~/Scripts/jquery.signalR-{version}.js"));

            #endregion



            #region Bootstrap

            bundles.Add(new ScriptBundle("~/bootstrap/js").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/bootstrap/css").Include(
                "~/Content/bootstrap.css"));

            #endregion



            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/modernizr/js").Include(
                "~/Scripts/modernizr-*"));



            // 将 EnableOptimizations 设为 false 以进行调试。有关详细信息，
            // 请访问 http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = false;
        }
    }
}

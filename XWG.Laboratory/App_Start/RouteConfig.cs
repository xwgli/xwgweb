using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace XWG.Laboratory
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Queue",
                url: "Queue/{action}",
                defaults: new { controller = "Queue", action = "Index" }
            );

            routes.MapRoute(
                name: "Weibo",
                url: "Weibo/{action}",
                defaults: new { controller = "Weibo", action = "Index" }
            );

            routes.MapRoute(
                name: "Weixin",
                url: "Weixin/{action}",
                defaults: new { controller = "Weixin", action = "Index" }
            );

            routes.MapRoute(
                name: "Room",
                url: "Room/{action}",
                defaults: new { controller = "Room", action = "Index" }
            );

            routes.MapRoute(
                name: "Comment",
                url: "Comment/{action}",
                defaults: new { controller = "Comment", action = "Index" }
            );

            routes.MapRoute(
                name: "HTML5",
                url: "HTML5/{action}",
                defaults: new { controller = "HTML5", action = "Index" }
            );

            routes.MapRoute(
                name: "Tools",
                url: "Tools/{action}",
                defaults: new { controller = "Tools", action = "Index" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{action}",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}

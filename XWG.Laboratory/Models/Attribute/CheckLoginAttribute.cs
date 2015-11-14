using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XWG.Laboratory.Models;

namespace SoulStone5.Web.Models.Attribute
{
    /// <summary>
    /// 检查用户是否登录
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CheckLoginAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 在执行Action方法之前由 ASP.NET MVC 框架调用。
        /// </summary>
        /// <param name="filterContext">过滤器上下文</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //用来解析过滤器的上下文信息
            FilterContextInfo info = new FilterContextInfo(filterContext);

            //授权解析公共类
            VisitorHelper auth = new VisitorHelper(info.Request, info.Response);
            auth.GetVisitorId();

            ////判断跳转
            //if (!auth.IsLoginSuccess())
            //{
            //    //跳转至登录页
            //    filterContext.Result = new ContentResult
            //    {
            //        Content = string.Format("<script type='text/javascript'>top.location='{0}?from=/'</script>",
            //                WebConfig.LoginPageUrl)
            //    };
            //}
        }
    }

    /// <summary>
    /// 解析过滤器上下文信息
    /// </summary>
    public class FilterContextInfo
    {
        /// <summary>
        /// 读取过滤器上下文信息来初始化
        /// </summary>
        /// <param name="filterContext"></param>
        public FilterContextInfo(ActionExecutingContext filterContext)
        {
            RequestUrl = filterContext.HttpContext.Request.Url == null ? string.Empty : filterContext.HttpContext.Request.Url.AbsolutePath;

            ControllerName = filterContext.RouteData.Values["controller"].ToString();

            ActionName = filterContext.RouteData.Values["action"].ToString();

            Session = filterContext.HttpContext.Session;

            Request = filterContext.HttpContext.Request;

            Response = filterContext.HttpContext.Response;
        }

        /// <summary>
        /// 来源的相对路径
        /// </summary>
        public string RequestUrl { get; set; }

        /// <summary>
        /// 控制器名称
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// Action方法名
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// Session信息
        /// </summary>
        public HttpSessionStateBase Session { get; set; }

        /// <summary>
        /// Request信息
        /// </summary>
        public HttpRequestBase Request { get; set; }

        /// <summary>
        /// Response信息
        /// </summary>
        public HttpResponseBase Response { get; set; }
    }
}
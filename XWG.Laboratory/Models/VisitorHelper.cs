using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI.WebControls;
using Microsoft.AspNet.SignalR.Hubs;
using Cookie = Microsoft.AspNet.SignalR.Cookie;

namespace XWG.Laboratory.Models
{
    /// <summary>
    /// 访问者信息
    /// </summary>
    public class VisitorRecord
    {
        /// <summary>
        /// 访问者Id
        /// </summary>
        public long VisitorId { get; set; }

        /// <summary>
        /// 访问者IP
        /// </summary>
        public string VisitorIP { get; set; }

        /// <summary>
        /// 用户代理字符串
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// 访问时间
        /// </summary>
        public DateTime VisitTime { get; set; }
    }

    /// <summary>
    /// 访问者辅助类
    /// </summary>
    public class VisitorHelper
    {
        /// <summary>
        /// Http请求
        /// </summary>
        private HttpRequestBase Request;

        /// <summary>
        /// Http响应
        /// </summary>
        private HttpResponseBase Response;

        /// <summary>
        /// 访问者
        /// </summary>
        private VisitorRecord Visitor;

        /// <summary>
        /// 初始化辅助类
        /// </summary>
        public VisitorHelper(HttpRequestBase request, HttpResponseBase response)
        {
            this.Request = request;
            this.Response = response;

            //获取信息
            Visitor = new VisitorRecord()
            {
                VisitorId = -1,
                UserAgent = Request.UserAgent,
                VisitorIP = Request.UserHostAddress,
                VisitTime = DateTime.Now
            };

            //判断Cookies中是否存在VisitorId
            var visitorId = Request.Cookies["VisitorId"];
            if (visitorId != null)
            {
                long id;
                //检查类型是否转换成功
                if (long.TryParse(visitorId.Value, out id))
                {
                    Visitor.VisitorId = id;
                }
            }
        }

        /// <summary>
        /// 初始化辅助类
        /// </summary>
        public VisitorHelper()
            : this(new HttpRequestWrapper(HttpContext.Current.Request), new HttpResponseWrapper(HttpContext.Current.Response))
        {
        }

        /// <summary>
        /// 初始化辅助类（直接设置信息）
        /// </summary>
        public VisitorHelper(VisitorRecord visitor)
        {
            this.Visitor = visitor;
        }

        /// <summary>
        /// 记录访问者信息
        /// </summary>
        public long RecordVisitor()
        {
            //插入数据库
            Visitor.VisitorId = ChatDataHelper.InsertNewVisitor(Visitor);
            //写入Cookies
            Response.Cookies.Clear();
            Response.Cookies.Add(new HttpCookie("VisitorId", Visitor.VisitorId.ToString()));
            //返回Id
            return Visitor.VisitorId;
        }

        /// <summary>
        /// 获取访问者Id，失败返回负数
        /// </summary>
        /// <returns></returns>
        public long GetVisitorId()
        {
            //检查是否已有VisitorId
            if (Visitor.VisitorId > 0)
            {
                return Visitor.VisitorId;
            }
            //Cookies中没有，则立即插入数据库进行记录
            return RecordVisitor();
        }

        /// <summary>
        /// 获取访问者IP
        /// </summary>
        /// <returns></returns>
        public string GetVisitorIP()
        {
            return this.Visitor.VisitorIP;
        }
    }
}
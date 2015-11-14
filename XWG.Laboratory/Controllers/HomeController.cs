using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XWG.Laboratory.Models;
using SoulStone5.Web.Models.Attribute;

namespace XWG.Laboratory.Controllers
{
    [CheckLogin]
    public class HomeController : Controller
    {
        /// <summary>
        /// 网站首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            SignalRHelper.SendMessageToChatRoom("页面访问", "用户进入聊天室！");
            return View();
        }

        /// <summary>
        /// SignalR 示例1
        /// </summary>
        /// <returns></returns>
        public ActionResult SignalR_Demo1()
        {
            return View();
        }

        #region 滚动测试

        /// <summary>
        /// 滚动测试
        /// </summary>
        /// <returns></returns>
        public ActionResult Scroll()
        {
            return View();
        }

        /// <summary>
        /// 滚动测试嵌入的框架页
        /// </summary>
        /// <returns></returns>
        public ActionResult Scroll_Frame()
        {
            return View();
        }

        #endregion

        /// <summary>
        /// 延时载图测试
        /// </summary>
        /// <returns></returns>
        public ActionResult LazyLoadImageTest()
        {
            return View();
        }

        /// <summary>
        /// 幻灯片测试
        /// </summary>
        /// <returns></returns>
        public ActionResult Slideshow()
        {
            return View();
        }

        /// <summary>
        /// 文件上传测试
        /// </summary>
        /// <returns></returns>
        public ActionResult FileUpload()
        {
            return View();
        }

        /// <summary>
        /// 富文本编辑测试
        /// </summary>
        /// <returns></returns>
        public ActionResult RTF()
        {
            return View();
        }
    }
}

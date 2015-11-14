using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XWG.Laboratory.Models;

namespace XWG.Laboratory.Controllers
{
    public class HTML5Controller : Controller
    {
        /// <summary>
        /// HTML5技术测试
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Three.js测试
        /// </summary>
        /// <returns></returns>
        public ActionResult ThreeJS()
        {
            return View();
        }

        /// <summary>
        /// Three.js示例1
        /// </summary>
        /// <returns></returns>
        public ActionResult ThreeJS_Demo1()
        {
            return View();
        }

        /// <summary>
        /// Buzz音频测试
        /// </summary>
        /// <returns></returns>
        public ActionResult Buzz()
        {
            return View();
        }

        /// <summary>
        /// WebRTC测试
        /// </summary>
        /// <returns></returns>
        public ActionResult WebRTC()
        {
            return View();
        }
    }
}
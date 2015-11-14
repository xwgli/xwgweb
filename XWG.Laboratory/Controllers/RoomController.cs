using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XWG.Laboratory.Controllers
{
    public class RoomController : Controller
    {
        /// <summary>
        /// 小网格聊天室
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 聊天室页面
        /// </summary>
        /// <param name="id">聊天室Id</param>
        /// <returns></returns>
        public ActionResult Enter(long id)
        {
            return View("Room");
        }
    }
}
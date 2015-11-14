using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XWG.Laboratory.Models;

namespace XWG.Laboratory.Controllers
{
    public class CommentController : Controller
    {
        /// <summary>
        /// 页面评论功能测试
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取评论数据
        /// 查询条件：{ desc:"倒序（布尔）" }
        /// </summary>
        /// <param name="current">当前要获取评论的页码（从1开始）</param>
        /// <param name="count">每页的评论条数</param>
        /// <param name="pageId">页面Id（区分评论所属页）</param>
        /// <param name="options">查询选项</param>
        /// <returns></returns>
        public ActionResult Get(int current, int count, long pageId, string options)
        {
            var param = new PagerParam(current, count, options);
            var result = CommentHelper.GetCommentByPageId(param, pageId);

            var data = result.Items as CommentRecord[];
            if (data != null)
            {
                bool isDescent;
                param.TryGetQueryOptions("desc", out isDescent);

                long x = param.SkipCount + 1;
                if (isDescent)
                {
                    x = (result.Total + 1) - x;
                    result.Items = data.Select(r => new
                    {
                        r = r,
                        num = x--,
                    });
                }
                else
                {
                    result.Items = data.Select(r => new
                    {
                        r = r,
                        num = x++,
                    });
                }
            }
            else
            {
                result.Items = new CommentRecord[0];
            }

            return Json(result);
        }

        /// <summary>
        /// 添加新评论
        /// </summary>
        /// <param name="nickname">评论人昵称</param>
        /// <param name="comment">评论内容</param>
        /// <param name="pageId">页面Id（区分评论所属页）</param>
        /// <returns></returns>
        public ActionResult Add(string nickname, string comment, long pageId)
        {
            VisitorHelper visitor = new VisitorHelper();
            var record = new CommentRecord()
            {
                Comment = comment,
                CommentTime = DateTime.Now,
                VisitorId = visitor.GetVisitorId(),
                IP = visitor.GetVisitorIP(),
                Nickname = nickname
            };

            record = CommentHelper.AddNew(record, pageId);
            return Json(record);
        }
    }
}
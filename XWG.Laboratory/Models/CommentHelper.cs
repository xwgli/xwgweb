using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace XWG.Laboratory.Models
{
    /// <summary>
    /// 评论记录
    /// </summary>
    public class CommentRecord
    {
        /// <summary>
        /// 评论Id
        /// </summary>
        public long RecordId { get; set; }

        /// <summary>
        /// 访问者Id
        /// </summary>
        public long VisitorId { get; set; }

        /// <summary>
        /// 评论人IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 评论人昵称
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 评论时间
        /// </summary>
        public DateTime CommentTime { get; set; }
    }

    /// <summary>
    /// 页面评论辅助类
    /// </summary>
    public static class CommentHelper
    {
        /// <summary>
        /// 根据页面Id获取该页面的所有评论（分页）
        /// 查询条件：{ desc:"倒序（布尔）" }
        /// </summary>
        /// <param name="param">分页参数</param>
        /// <param name="pageId">页面Id</param>
        /// <returns></returns>
        public static PagerResult GetCommentByPageId(PagerParam param, long pageId)
        {
            var result = param.GetPagerResult();

            bool isDescent;
            param.TryGetQueryOptions("desc", out isDescent);

            XwgWebDBEntities entities = new XwgWebDBEntities();
            var query = entities.tbl_CommentRecord.Where(r => r.PageId == pageId);

            if (isDescent)
            {
                query = query.OrderByDescending(r => r.CommentTime);
            }
            else
            {
                query = query.OrderBy(r => r.CommentTime);
            }

            result.Total = query.Count();
            param.CheckLastPage(result.Total);

            result.Items = query.Skip(param.SkipCount).Take(param.RowCount)
                .Select(r => new CommentRecord()
                {
                    RecordId = r.RecordId,
                    Comment = r.Comment,
                    CommentTime = r.CommentTime,
                    VisitorId = r.VisitorId,
                    IP = r.tbl_VisitRecord.VisitorIP,
                    Nickname = r.NickName
                })
                .ToArray();

            return result;
        }

        /// <summary>
        /// 添加新的评论
        /// </summary>
        /// <param name="record">评论记录</param>
        /// <returns></returns>
        public static CommentRecord AddNew(CommentRecord record, long pageId)
        {
            var newComment = new tbl_CommentRecord
            {
                Comment = record.Comment,
                CommentTime = record.CommentTime,
                NickName = record.Nickname,
                PageId = pageId,
                VisitorId = record.VisitorId
            };

            XwgWebDBEntities entities = new XwgWebDBEntities();
            entities.tbl_CommentRecord.Add(newComment);
            var count = entities.SaveChanges();

            record.RecordId = newComment.RecordId;
            return count > 0 ? record : null;
        }
    }
}
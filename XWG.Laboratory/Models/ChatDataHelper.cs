using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XWG.Laboratory.Models
{
    /// <summary>
    /// 聊天室数据辅助操作类
    /// </summary>
    public static class ChatDataHelper
    {
        /// <summary>
        /// 记录新访问者的信息
        /// </summary>
        /// <param name="visitor">访问者信息</param>
        /// <returns></returns>
        public static long InsertNewVisitor(VisitorRecord visitor)
        {
            var newVisitor = new tbl_VisitRecord()
            {
                VisitorIP = visitor.VisitorIP,
                UserAgent = visitor.UserAgent,
                VisitTime = visitor.VisitTime
            };
            XwgWebDBEntities db = new XwgWebDBEntities();
            db.tbl_VisitRecord.Add(newVisitor);
            var count = db.SaveChanges();
            return count > 0 ? newVisitor.VisitorId : -1;
        }

        /// <summary>
        /// 保存聊天记录
        /// </summary>
        /// <param name="visitorId">访问者Id</param>
        /// <param name="nickname">昵称</param>
        /// <param name="message">发送的消息</param>
        /// <returns></returns>
        public static long SaveChatMessage(long visitorId, string nickname, string message)
        {
            var newMessage = new tbl_ChatRecord()
            {
                VisitorId = visitorId,
                Message = message,
                NickName = nickname,
                SendTime = DateTime.Now
            };
            XwgWebDBEntities db = new XwgWebDBEntities();
            db.tbl_ChatRecord.Add(newMessage);
            var count = db.SaveChanges();
            return count > 0 ? newMessage.RecordId : -1;
        }
    }
}
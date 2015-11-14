using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XWG.Laboratory.Models;
using Microsoft.AspNet.SignalR;

namespace XWG.Laboratory.Hubs
{
    /// <summary>
    /// 聊天室的消息集线器
    /// </summary>
    public class MessageHub : Hub
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="name">昵称</param>
        /// <param name="message">消息</param>
        public void SendToAll(string name, string message)
        {

            long visitorId = -1;

            //判断Cookies中是否存在VisitorId
            Cookie VisitorId;
            if (this.Context.Request.Cookies.TryGetValue("VisitorId", out VisitorId))
            {
                if (!long.TryParse(VisitorId.Value, out visitorId))
                {
                    visitorId = -1;
                }
            }

            //没有获取到发送者的信息，则调用方法刷新发送者的页面
            if (visitorId < 0)
            {
                Clients.Caller.refresh();
                return;
            }

            //信息齐全，保存聊天记录
            ChatDataHelper.SaveChatMessage(visitorId, name, message);

            //并发送给所有人
            var e = this.Context.Request.Environment;
            object outResult;
            if (e.TryGetValue("server.RemoteIpAddress", out outResult))
            {
                string ip = outResult as string;
                if (string.IsNullOrEmpty(ip))
                {
                    ip = "未获取到";
                }
                Clients.All.addNewMessageToPage(name + "（" + ip + "@" + DateTime.Now.ToString("hh:mm:ss") + "）", message);
            }
        }
    }
}
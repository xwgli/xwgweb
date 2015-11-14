using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XWG.Laboratory.Hubs;
using Microsoft.AspNet.SignalR;

namespace XWG.Laboratory.Models
{
    /// <summary>
    /// 系统消息类型
    /// </summary>
    public enum SystemMessageType
    {
        页面访问,
        调试,
        测试,
        跟踪日志
    }

    /// <summary>
    /// SignalR辅助类
    /// </summary>
    public class SignalRHelper
    {
        /// <summary>
        /// 向小网格聊天室中发送消息
        /// </summary>
        /// <param name="name">昵称</param>
        /// <param name="message">消息内容</param>
        public static void SendMessageToChatRoom(string name, string message)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<MessageHub>();
            context.Clients.All.addNewMessageToPage(name + "@" + DateTime.Now.ToString("hh:mm:ss") + "：", message);
        }

        /// <summary>
        /// 向小网格聊天室中发送系统消息
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="message">消息内容</param>
        public static void SendMessageToChatRoom(SystemMessageType type, string message)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<MessageHub>();
            context.Clients.All.addNewMessageToPage("#系统[" + type + "]@" + DateTime.Now.ToString("hh:mm:ss") + "：", message);
        }
    }
}
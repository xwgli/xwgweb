using Microsoft.AspNet.SignalR;
using Owin;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(XWG.Laboratory.SignalRConfig))]

namespace XWG.Laboratory
{
    public class SignalRConfig
    {
        public void Configuration(IAppBuilder app)
        {
            // 每个hub每个链接存储的消息数量（该设置项会涉及到内存占用）
            GlobalHost.Configuration.DefaultMessageBufferSize = 500;
            app.MapSignalR();
        }
    }
}
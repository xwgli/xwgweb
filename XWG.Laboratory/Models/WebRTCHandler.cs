using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using Microsoft.Web.WebSockets;

namespace XWG.Laboratory.Models
{
    public class WebRTCHandler : WebSocketHandler
    {
        private static WebSocketCollection sessions = new WebSocketCollection();

        public String UserId { get; set; }

        public override void OnOpen()
        {
            this.UserId = Guid.NewGuid().ToString("N");

            var message = new { type = SignalMessageType.Connect, userId = this.UserId };

            sessions.Broadcast(Json.Encode(message));

            sessions.Add(this);
        }

        public override void OnMessage(string msg)
        {
            var obj = Json.Decode(msg);
            var messageType = (SignalMessageType)obj.type;

            switch (messageType)
            {
                case SignalMessageType.Offer:
                case SignalMessageType.Answer:
                case SignalMessageType.IceCandidate:
                    var handler = sessions.Cast<WebRTCHandler>().FirstOrDefault(n => n.UserId == obj.userId);

                    var message = new { type = messageType, userId = this.UserId, description = obj.description };

                    if (handler != null)
                    {
                        handler.Send(Json.Encode(message));
                    }
                    break;
            }
        }
    }

    public enum SignalMessageType
    {
        Connect,
        Disconnect,
        Offer,
        Answer,
        IceCandidate
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using XWG.Laboratory.Models;
using Microsoft.Web.WebSockets;

namespace XWG.Laboratory.api
{
    public class WebRTCController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Connect()
        {
            var session = new WebRTCHandler();
            HttpContext.Current.AcceptWebSocketRequest(session);

            return new HttpResponseMessage(HttpStatusCode.SwitchingProtocols);
        }

        [HttpGet]
        public string Test()
        {
            return "hehe";
        }
    }
}
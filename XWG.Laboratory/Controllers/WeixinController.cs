using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using XWG.Laboratory.Models;
using Newtonsoft.Json;

namespace XWG.Laboratory.Controllers
{
    public class WeixinController : Controller
    {
        /// <summary>
        /// 微信浏览器测试
        /// </summary>
        /// <returns></returns>
        public ActionResult BrowserTest()
        {
            ViewBag.Root = WebHelper.CurrentHostRootUrl;
            return View();
        }

        /// <summary>
        /// 用于验证的Token
        /// </summary>
        private static string WX_Token = ConfigurationManager.AppSettings["WX_Token"];

        /// <summary>
        /// 验证微信签名
        /// </summary>
        /// * 将token、timestamp、nonce三个参数进行字典序排序
        /// * 将三个参数字符串拼接成一个字符串进行sha1加密
        /// * 开发者获得加密后的字符串可与signature对比，标识该请求来源于微信。
        /// <returns></returns>
        private bool CheckWeixinSignature(string signature, string timestamp, string nonce)
        {
            string[] tempArray = { WX_Token, timestamp, nonce };
            Array.Sort(tempArray);

            string result = string.Join("", tempArray);
            result = result.GetSHA1().ToLower();

            return result == signature;
        }

        /// <summary>
        /// 微信接口验证
        /// </summary>
        /// <param name="signature">微信加密签名，signature结合了开发者填写的token参数和请求中的timestamp参数、nonce参数。</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="nonce">随机数</param>
        /// <param name="echostr">随机字符串</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(string signature, string timestamp, string nonce, string echostr)
        {
            if (CheckWeixinSignature(signature, timestamp, nonce))
            {
                SignalRHelper.SendMessageToChatRoom("微信", "接口验证通过！");
                return Content(echostr);
            }
            SignalRHelper.SendMessageToChatRoom("微信", "接口验证失败！");
            return Content("授权错误");
        }

        /// <summary>
        /// 微信接口调用
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(string signature, string timestamp, string nonce)
        {
            //验证调用者身份
            if (CheckWeixinSignature(signature, timestamp, nonce))
            {
                try
                {
                    //读取调用内容XML
                    string requestXml;
                    using (var reader = new StreamReader(Request.InputStream))
                    {
                        requestXml = reader.ReadToEnd();
                    }
                    SignalRHelper.SendMessageToChatRoom("微信", "接口调用请求@" + requestXml);

                    //解析XML
                    var requestDocument = XDocument.Parse(requestXml);
                    var datas = requestDocument.Elements().ToArray();

                    //通用数据模型
                    var commonModel = new
                    {
                        //接收人Id
                        ToUserName = datas.Elements("ToUserName").First().Value,
                        //发送人Id
                        FromUserName = datas.Elements("FromUserName").First().Value,
                        //发送时间
                        CreateTime = datas.Elements("CreateTime").First().Value,
                        //消息类型
                        MsgType = datas.Elements("MsgType").First().Value
                    };

                    var weixin = new WeixinHelper(commonModel.FromUserName);

                    if (commonModel.MsgType == "text" && datas.Elements("Content").First().Value == "重置")
                    {
                        weixin.RemoveData();
                    }

                    var user = weixin.GetFromUser();

                    //根据传入消息类型处理消息并返回
                    string responseMessage;
                    switch (commonModel.MsgType)
                    {
                        //纯文本消息
                        case "text":
                            var textModel = new
                            {
                                //消息Id
                                MsgId = datas.Elements("MsgId").First().Value,
                                //文本消息内容
                                Content = datas.Elements("Content").First().Value
                            };
                            var askNick = weixin.GetData<string>("AskForNickName");
                            if (!string.IsNullOrWhiteSpace(askNick))
                            {
                                if (textModel.Content.Contains("小白"))
                                {
                                    responseMessage = "哇，你就是传说中的小白吧？终于等到你了！我是小黑的替身哦，你对我说的话我会帮你转告给小黑的~";
                                    weixin.SaveNickName(textModel.Content);
                                    weixin.RemoveData("AskForNickName");
                                }
                                else
                                {
                                    responseMessage = "好的，那我就叫你 " + textModel.Content + " 啦~";
                                    weixin.SaveNickName(textModel.Content);
                                    weixin.RemoveData("AskForNickName");
                                }
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(user.NickName))
                                {
                                    responseMessage = "我还不知道你的名字，应该怎么称呼你？";
                                    weixin.SaveData("AskForNickName", responseMessage);
                                }
                                else
                                {
                                    if (textModel.Content.StartsWith("问："))
                                    {
                                        var qa = textModel.Content.Split(new string[] { "问：", "答：" }, StringSplitOptions.RemoveEmptyEntries);
                                        if (qa.Length == 2)
                                        {
                                            weixin.SaveQa(qa[0], qa[1]);
                                            if (user.IsSpecial)
                                            {
                                                responseMessage = "谢谢小白！小黑一定把你告诉我的话铭记于心！ ";
                                            }
                                            else
                                            {
                                                responseMessage = "谢谢你！" + user.NickName + "，我已经记住了！";
                                            }
                                        }
                                        else
                                        {
                                            if (user.IsSpecial)
                                            {
                                                responseMessage = "不好意思小白！小黑太笨了，没有明白你说的是什么呢！/:P-( ";
                                            }
                                            else
                                            {
                                                responseMessage = "对不起，我没有明白你教给我的东西。";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var answer = weixin.GetAnswer(textModel.Content);
                                        if (!string.IsNullOrWhiteSpace(answer))
                                        {
                                            responseMessage = answer;
                                        }
                                        else
                                        {
                                            if (user.IsSpecial)
                                            {
                                                responseMessage = "不好意思，小白，小黑的目前还很笨，都不知道如何回应你这句话，但是你可以教我说，比如发给我：“问：小黑最好的朋友是谁 答：当然是小白啦”，这样小黑就又聪明一点了呢~";
                                            }
                                            else
                                            {
                                                responseMessage = "对不起，我还不知道如何回应你这句话，你可以多教教我，比如：“问：一加一等于几 答：二”";
                                            }
                                        }
                                    }
                                }
                            }
                            weixin.SaveTextMessage(textModel.Content, responseMessage);
                            //responseMessage = "您好，我收到了消息：\r\n" + textModel.Content;
                            break;
                        case "image":
                            goto default;
                        case "voice":
                            goto default;
                        case "video":
                            goto default;
                        //位置消息
                        case "location":
                            var locationModel = new
                            {
                                //纬度
                                Location_X = datas.Elements("Location_X").First().Value,
                                //经度
                                Location_Y = datas.Elements("Location_Y").First().Value,
                                //缩放级别
                                Scale = datas.Elements("Scale").First().Value,
                                //位置信息
                                Label = datas.Elements("Label").First().Value,
                                //消息Id
                                MsgId = datas.Elements("MsgId").First().Value
                            };
                            responseMessage = string.Format("您的位置：\r\n经度：{0}\r\n纬度：{1}\r\n缩放：{2}\r\n信息：{3}"
                                , locationModel.Location_Y, locationModel.Location_X, locationModel.Scale, locationModel.Label);
                            break;
                        case "link":
                            goto default;
                        //事件消息
                        case "event":
                            //获取事件类型
                            var eventType = datas.Elements("Event").First().Value;

                            //对不同的事件进行不同的处理
                            switch (eventType)
                            {
                                //关注事件
                                case "subscribe":

                                    if (string.IsNullOrWhiteSpace(user.NickName))
                                    {
                                        responseMessage = "非常高兴你能关注我，我是哦呀叽，我应该怎么称呼你？";
                                        weixin.SaveData("AskForNickName", responseMessage);
                                    }
                                    else
                                    {
                                        responseMessage = user.NickName + "，感谢你再次关注我。我的功能还不完善，请多多指教！";
                                    }

                                    weixin.SaveTextMessage(user.UserName, responseMessage);

                                    //带参数的二维码（需微信认证）
                                    var qr = datas.Elements("EventKey").FirstOrDefault();
                                    if (qr != null)
                                    {
                                        var qrscene = qr.Value;
                                        if (qrscene.StartsWith("qrscene"))
                                        {
                                            var ticket = datas.Elements("Ticket").First().Value;
                                            responseMessage = string.Format("您未关注并扫描了二维码：\r\nqrscene：{0}\r\nticket：{1}\r\n",
                                                qrscene, ticket);
                                        }
                                    }
                                    break;
                                //取消关注
                                case "unsubscribe":
                                    responseMessage = "非常遗憾您取消了关注小网格工作室！希望能再次获得您的关注，谢谢，再见！";
                                    break;
                                //自动上报用户位置（需微信认证）
                                case "LOCATION":
                                    var Latitude = datas.Elements("Latitude").First().Value;
                                    var Longitude = datas.Elements("Longitude").First().Value;
                                    var Precision = datas.Elements("Scale").First().Value;
                                    responseMessage = string.Format("您自动上报的位置：\r\n经度：{0}\r\n纬度：{1}\r\n精度：{2}",
                                        Longitude, Latitude, Precision);
                                    break;
                                //扫描带参数的二维码（需微信认证）
                                case "SCAN":
                                    var EventKey = datas.Elements("EventKey").First().Value;
                                    var Ticket = datas.Elements("Ticket").First().Value;
                                    responseMessage = string.Format("您已关注并扫描了二维码：\r\nscene_id：{0}\r\nticket：{1}\r\n",
                                        EventKey, Ticket);
                                    break;
                                //未知事件类型
                                default:
                                    responseMessage = "您好，暂不支持[" + eventType + "]类型的事件！";
                                    break;
                            }
                            break;
                        //未知类型消息
                        default:
                            responseMessage = "您好，暂不支持[" + commonModel.MsgType + "]类型的消息！";
                            break;
                    }

                    //拼接返回的XML内容（目前只返回文本消息）
                    var responseDocument = new XDocument(
                        new XElement("xml",
                            new XElement("ToUserName", new XCData(commonModel.FromUserName)),
                            new XElement("FromUserName", new XCData(commonModel.ToUserName)),
                            new XElement("CreateTime", new XCData(StringHelper.GetUnixTimeString())),
                            new XElement("MsgType", new XCData("text")),
                            new XElement("Content", new XCData(responseMessage))
                        )
                    );

                    //将响应XML内容转换为字符串返回
                    var responseXml = responseDocument.ToString(SaveOptions.DisableFormatting);
                    SignalRHelper.SendMessageToChatRoom("微信", "接口调用响应@" + responseXml);
                    WeixinHelper.SaveRecord(requestXml, responseXml);
                    return Content(responseXml);
                }
                catch (Exception ex)
                {
                    SignalRHelper.SendMessageToChatRoom("微信", "接口调用异常@" + ex.Message + "@" + ex.StackTrace);
                }
            }
            SignalRHelper.SendMessageToChatRoom("微信", "接口调用失败！");
            return Content("授权错误");
        }

        #region 获取 Access Token （需微信认证）

        private static string WX_AppID = ConfigurationManager.AppSettings["WX_AppID"];
        private static string WX_AppSecret = ConfigurationManager.AppSettings["WX_AppSecret"];

        private static string _accessToken;
        private static DateTime _accessTokenExpireTime;

        /// <summary>
        /// 获取 Access Token （自动判断是否过期并重新获取）
        /// </summary>
        private static string WX_AccessToken
        {
            get
            {
                if (_accessTokenExpireTime <= DateTime.Now)
                {
                    if (!TryGetAccessToken(out _accessToken, out _accessTokenExpireTime))
                    {
                        return "";
                    }
                }
                return _accessToken;
            }
        }

        /// <summary>
        /// 尝试获取 Access Token
        /// </summary>
        /// <param name="token">获取到的 Access Token</param>
        /// <param name="expire">Access Token 的过期时间</param>
        /// <returns></returns>
        private static bool TryGetAccessToken(out string token, out DateTime expire)
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}",
                    WX_AppID, WX_AppSecret);

            string content = WebHelper.GetHttpResponse(url);

            var result = JsonConvert.DeserializeAnonymousType(content, new { access_token = "", expires_in = 0 });
            string message;
            bool ret;
            if (!string.IsNullOrWhiteSpace(result.access_token))
            {
                message = string.Format("成功。access_token：" + result.access_token + "，expires_in：" + result.expires_in);

                token = result.access_token;
                expire = DateTime.Now.AddSeconds(result.expires_in);

                ret = true;
            }
            else
            {
                var error = JsonConvert.DeserializeAnonymousType(content, new { errcode = "", errmsg = "" });

                message = string.Format("失败。errcode：" + error.errcode + "，errmsg：" + error.errmsg);

                token = string.Empty;
                expire = new DateTime(1, 1, 1);

                ret = false;
            }

            SignalRHelper.SendMessageToChatRoom("微信", "新的access_token@" + url + "@" + message);
            return ret;
        }

        #endregion
    }
}
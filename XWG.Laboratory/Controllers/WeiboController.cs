using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XWG.Laboratory.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace XWG.Laboratory.Controllers
{
    public class WeiboController : Controller
    {
        private static string WB_AppKey = ConfigurationManager.AppSettings["WB_AppKey"];
        private static string WB_AppSecret = ConfigurationManager.AppSettings["WB_AppSecret"];
        private static string WB_AuthCallback = ConfigurationManager.AppSettings["WB_AuthCallback"];

        /// <summary>
        /// 微博测试
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string access_token, string auth_response)
        {
            SignalRHelper.SendMessageToChatRoom("页面访问", "微博测试");

            ViewBag.AuthorizeUrl = string.Format("https://api.weibo.com/oauth2/authorize?response_type=code&client_id={0}&redirect_uri={1}",
                WB_AppKey, WB_AuthCallback);
            ViewBag.NewAuthorizeUrl = string.Format("https://api.weibo.com/oauth2/authorize?response_type=code&forcelogin=true&client_id={0}&redirect_uri={1}",
                WB_AppKey, WB_AuthCallback);

            ViewBag.AccessToken = access_token;

            ViewBag.AuthResponse = auth_response;

            ViewBag.CancelAuthUrl = Url.Action("CancelAuth") + "?access_token=" + access_token;
            ViewBag.GetAuthInfoUrl = Url.Action("GetAuthInfo") + "?access_token=" + access_token;
            ViewBag.PublishUrl = Url.Action("Publish") + "?access_token=" + access_token;
            ViewBag.GetUserInfoUrl = Url.Action("GetUserInfo") + "?access_token=" + access_token;
            ViewBag.GetUserLimitUrl = Url.Action("GetUserLimit") + "?access_token=" + access_token;
            ViewBag.UrlShortenUrl = Url.Action("UrlShorten") + "?access_token=" + access_token;
            ViewBag.UrlExpandUrl = Url.Action("UrlExpand") + "?access_token=" + access_token;
            ViewBag.UrlShareCountUrl = Url.Action("UrlShareCount") + "?access_token=" + access_token;
            ViewBag.GetEmotionsUrl = Url.Action("GetEmotions") + "?access_token=" + access_token;

            return View();
        }

        /// <summary>
        /// 微博授权成功后的跳转页
        /// </summary>
        /// <param name="code">返回值（据此获取access_token）</param>
        /// <returns></returns>
        public ActionResult AuthCallback(string code)
        {
            var url = string.Format("https://api.weibo.com/oauth2/access_token?grant_type=authorization_code&client_id={0}&client_secret={1}&redirect_uri={2}&code={3}"
                , WB_AppKey, WB_AppSecret, WB_AuthCallback, code);

            var response = WebHelper.GetHttpResponse(url, HttpRequestMethod.POST, HttpRequestContentType.UrlEncoded);
            var result = JsonConvert.DeserializeAnonymousType(response, new
            {
                access_token = "ACCESS_TOKEN",
                expires_in = 1234,
                uid = "12341234"
            });

            var message = "access_token：" + result.access_token + ",expires_in：" + result.expires_in + ",uid：" + result.uid;
            SignalRHelper.SendMessageToChatRoom("微博", "授权成功@" + response);

            return RedirectToAction("Index", new { access_token = result.access_token, auth_response = message });
        }

        /// <summary>
        /// 应用被用户取消授权时，微博的回调方法
        /// </summary>
        /// <param name="source">应用appkey</param>
        /// <param name="uid">取消授权的用户</param>
        /// <param name="auth_end">取消授权的时间</param>
        public void CancelAuthCallBack(string source, string uid, string auth_end)
        {
            SignalRHelper.SendMessageToChatRoom("微博", "授权取消@source：" + source + ",uid：" + uid + ",auth_end：" + auth_end.GetDateTimeByUnixTime("yyyy-MM-dd HH:mm:ss"));
        }

        /// <summary>
        /// 主动取消授权
        /// </summary>
        /// <param name="access_token">授权凭证</param>
        /// <returns></returns>
        public ActionResult CancelAuth(string access_token)
        {
            var url = string.Format("https://api.weibo.com/oauth2/revokeoauth2?access_token={0}", access_token);

            var response = WebHelper.GetHttpResponse(url, HttpRequestMethod.POST, HttpRequestContentType.UrlEncoded);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 获取授权信息
        /// </summary>
        /// <param name="access_token">授权凭证</param>
        /// <returns></returns>
        public ActionResult GetAuthInfo(string access_token)
        {
            var url = string.Format("https://api.weibo.com/oauth2/get_token_info?access_token={0}", access_token);

            var response = WebHelper.GetHttpResponse(url, HttpRequestMethod.POST, HttpRequestContentType.UrlEncoded);
            var result = JsonConvert.DeserializeAnonymousType(response, new
            {
                uid = 1073880650,
                appkey = 1352222456,
                scope = "",
                create_at = 1352267591,
                expire_in = 157679471
            });

            var message = "uid：" + result.uid + ",appkey：" + result.appkey + ",scope：" + result.scope + ",create_at：" + result.create_at.ToString().GetDateTimeByUnixTime("yyyy-MM-dd HH:mm:ss") + ",expire_in：" + result.expire_in;

            SignalRHelper.SendMessageToChatRoom("微博", "授权信息@" + response);

            return RedirectToAction("Index", new { access_token = access_token, auth_response = message });
        }

        /// <summary>
        /// 发布微博
        /// </summary>
        /// <param name="content">微博内容</param>
        /// <param name="access_token">授权凭证</param>
        /// <returns></returns>
        public ActionResult Publish(string content, string access_token)
        {
            content = Server.UrlEncode(content);
            var url = string.Format("https://api.weibo.com/2/statuses/update.json?access_token={0}&status={1}", access_token, content);

            var response = WebHelper.GetHttpResponse(url, HttpRequestMethod.POST, HttpRequestContentType.UrlEncoded);

            SignalRHelper.SendMessageToChatRoom("微博", "新微博@" + response);

            return Content(response);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="access_token">授权凭证</param>
        /// <returns></returns>
        public ActionResult GetUserInfo(string access_token)
        {
            //首先通过access_token获取到用户的uid
            var get_uid_url = string.Format("https://api.weibo.com/2/account/get_uid.json?access_token={0}", access_token);

            var uid_response = WebHelper.GetHttpResponse(get_uid_url);
            var uid = JsonConvert.DeserializeAnonymousType(uid_response, new { uid = "" }).uid;

            if (string.IsNullOrWhiteSpace(uid))
            {
                return Content("没有获取到用户数据");
            }

            //然后再根据access_token和uid获取用户的信息
            var url = string.Format("https://api.weibo.com/2/users/show.json?access_token={0}&uid={1}", access_token, uid);

            var response = WebHelper.GetHttpResponse(url);
            var result = JsonConvert.DeserializeAnonymousType(response, new
            {
                //用户UID
                id = 1404376560,
                //用户昵称
                screen_name = "zaku",
                //友好显示名称
                name = "zaku",
                //用户所在地
                location = "北京 朝阳区",
                //用户个人描述
                description = "人生五十年，乃如梦如幻；有生斯有死，壮士复何憾。",
                //用户博客地址
                url = "http://blog.sina.com.cn/zaku",
                //用户头像地址（中图），50×50像素
                profile_image_url = "http://tp1.sinaimg.cn/1404376560/50/0/1",
                //用户的微博统一URL地址
                profile_url = "zaku",
                //用户的个性化域名
                domain = "zaku",
                //性别，m：男、f：女、n：未知
                gender = "m",
                //粉丝数
                followers_count = 1204,
                //关注数
                friends_count = 447,
                //微博数
                statuses_count = 2908,
                //收藏数
                favourites_count = 0,
                //用户创建（注册）时间
                created_at = "Fri Aug 28 00:00:00 +0800 2009",
                //是否是微博认证用户，即加V用户，true：是，false：否
                verified = false,
                //认证类型
                verified_type = 2,
                //用户头像地址（大图），180×180像素
                avatar_large = "http://tp1.sinaimg.cn/1404376560/180/0/1",
                //用户头像地址（高清），高清头像原图
                avatar_hd = "",
                //认证原因
                verified_reason = "",
                //用户的在线状态，0：不在线、1：在线
                online_status = 0,
                //用户的互粉数
                bi_followers_count = 215
            });

            SignalRHelper.SendMessageToChatRoom("微博", "用户信息@" + response);

            return RedirectToAction("Index", new { access_token = access_token, auth_response = JsonConvert.SerializeObject(result) });
        }

        /// <summary>
        /// 获取用户接口限制情况
        /// </summary>
        /// <param name="access_token">授权凭证</param>
        /// <returns></returns>
        public ActionResult GetUserLimit(string access_token)
        {
            var url = string.Format("https://api.weibo.com/2/account/rate_limit_status.json?access_token={0}", access_token);

            var response = WebHelper.GetHttpResponse(url);
            var result = JsonConvert.DeserializeAnonymousType(response, new
            {
                ip_limit = 10000,
                limit_time_unit = "HOURS",
                remaining_ip_hits = 10000,
                remaining_user_hits = 150,
                reset_time = "2011-06-03 18:00:00",
                reset_time_in_seconds = 1415,
                user_limit = 150,
            });

            SignalRHelper.SendMessageToChatRoom("微博", "用户接口限制@" + response);

            return RedirectToAction("Index", new { access_token = access_token, auth_response = JsonConvert.SerializeObject(result) });
        }

        /// <summary>
        /// 转换短链
        /// </summary>
        /// <param name="access_token">授权凭证</param>
        /// <param name="url_long">需要转换的长链接</param>
        /// <returns></returns>
        public ActionResult UrlShorten(string access_token, string url_long)
        {
            url_long = Server.UrlEncode(url_long);
            var url = string.Format("https://api.weibo.com/2/short_url/shorten.json?access_token={0}&url_long={1}", access_token, url_long);

            var response = WebHelper.GetHttpResponse(url);
            var result = JsonConvert.DeserializeAnonymousType(response, new
            {
                urls = new JObject[] { }
            });

            SignalRHelper.SendMessageToChatRoom("微博", "转换短链@" + response);

            return Content(result.urls[0].Value<string>("url_short"));
        }

        /// <summary>
        /// 还原短链
        /// </summary>
        /// <param name="access_token">授权凭证</param>
        /// <param name="url_short">需要还原的短链接</param>
        /// <returns></returns>
        public ActionResult UrlExpand(string access_token, string url_short)
        {
            url_short = Server.UrlEncode(url_short);
            var url = string.Format("https://api.weibo.com/2/short_url/expand.json?access_token={0}&url_short={1}", access_token, url_short);

            var response = WebHelper.GetHttpResponse(url);
            var result = JsonConvert.DeserializeAnonymousType(response, new
            {
                urls = new JObject[] { }
            });

            SignalRHelper.SendMessageToChatRoom("微博", "还原短链@" + response);

            return Content(result.urls[0].Value<string>("url_long"));
        }

        /// <summary>
        /// 获取短链分享数
        /// </summary>
        /// <param name="access_token">授权凭证</param>
        /// <param name="url_short">短链接</param>
        /// <returns></returns>
        public ActionResult UrlShareCount(string access_token, string url_short)
        {
            url_short = Server.UrlEncode(url_short);
            var url = string.Format("https://api.weibo.com/2/short_url/share/counts.json?access_token={0}&url_short={1}", access_token, url_short);

            var response = WebHelper.GetHttpResponse(url);
            var result = JsonConvert.DeserializeAnonymousType(response, new
            {
                urls = new JObject[] { }
            });

            SignalRHelper.SendMessageToChatRoom("微博", "短链分享数@" + response);

            return Content(result.urls[0].Value<string>("share_counts"));
        }

        /// <summary>
        /// 微博表情类
        /// </summary>
        private class WeiboEmotion
        {
            /// <summary>
            /// 种类
            /// </summary>
            public string category { get; set; }

            /// <summary>
            /// 是否为基础表情
            /// </summary>
            public bool common { get; set; }

            /// <summary>
            /// 是否为热门表情
            /// </summary>
            public bool hot { get; set; }

            /// <summary>
            /// 图标地址
            /// </summary>
            public string icon { get; set; }

            /// <summary>
            /// 转换文字
            /// </summary>
            public string phrase { get; set; }

            /// <summary>
            /// 图片Id
            /// </summary>
            public string picid { get; set; }

            /// <summary>
            /// 类型
            /// </summary>
            public string type { get; set; }

            /// <summary>
            /// 地址
            /// </summary>
            public string url { get; set; }

            /// <summary>
            /// 值
            /// </summary>
            public string value { get; set; }
        }

        /// <summary>
        /// 获取微博表情
        /// </summary>
        /// <param name="access_token">授权凭证</param>
        /// <param name="type">表情类别，face：普通表情、ani：魔法表情、cartoon：动漫表情，默认为face。</param>
        /// <returns></returns>
        public ActionResult GetEmotions(string access_token, string type = "face")
        {
            var url = string.Format("https://api.weibo.com/2/emotions.json?access_token={0}&type={1}", access_token, type);

            var response = WebHelper.GetHttpResponse(url);
            var result = JsonConvert.DeserializeObject<WeiboEmotion[]>(response);

            SignalRHelper.SendMessageToChatRoom("微博", "微博表情@" + response.Substring(0, 500));

            return Json(result);
        }
    }
}
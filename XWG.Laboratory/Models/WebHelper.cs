using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XWG.Laboratory.Models
{
    /// <summary>
    /// HTTP请求内容类型
    /// </summary>
    public enum HttpRequestContentType
    {
        Default, UrlEncoded, Json
    }

    /// <summary>
    /// HTTP请求方法类型
    /// </summary>
    public enum HttpRequestMethod
    {
        GET, POST
    }

    /// <summary>
    /// 网站路径相关辅助类
    /// </summary>
    public static class WebHelper
    {
        /// <summary>
        /// 网站的物理根路径
        /// </summary>
        public static string PhysicalRootPath { get { return GetPhysicalPath("/"); } }

        /// <summary>
        /// 网站根路径（会确保至少有一个/）
        /// </summary>
        public static string RootUrl { get { return WebHelper.GetRelativeUrl("/"); } }

        /// <summary>
        /// 当前用请求的网站的域名根路径
        /// </summary>
        public static string CurrentHostRootUrl
        {
            get
            {
                var request = HttpContext.Current.Request;
                var root = request.Url.AbsoluteUri;
                
                var cutIndex = root.IndexOf(request.Path);
                var protocolIndex = root.IndexOf("://") + 3;

                if (cutIndex > protocolIndex)
                {
                    root = root.Substring(0, cutIndex);
                }
                else
                {
                    root = root.TrimEnd('/');
                }

                return root + RootUrl;
            }
        }

        /// <summary>
        /// 根据相对路径返回绝对路径（来的时候最后带/就带，不带就不带。如果来的是空，则最后带/）
        /// </summary>
        /// <param name="url">相对于网站根目录的URL，请以/或~/开头</param>
        /// <returns></returns>
        public static string GetRelativeUrl(string url)
        {
            string path = HttpRuntime.AppDomainAppVirtualPath;
            if (!string.IsNullOrEmpty(path))
            {
                //检查url是否为空
                if (url == null)
                    return path;

                if (url.StartsWith("http://") || url.StartsWith("https://") || url.StartsWith("ftp://"))
                {
                    return url;
                }

                //开头去掉：~和/
                url = url.TrimStart('~').TrimStart('/');

                //确保服务器根目录有：/
                if (!path.EndsWith("/")) path = path + "/";

                return path + url;
            }
            return url;
        }

        /// <summary>
        /// 根据文件在服务器上的物理路径，获取文件在网站中的虚拟路径
        /// </summary>
        /// <param name="path">文件在服务器上的物理路径</param>
        /// <returns></returns>
        public static string GetRelativeUrlByPhysicalPath(string path)
        {
            if (path.StartsWith(PhysicalRootPath))
            {
                return GetRelativeUrl(path.Replace(PhysicalRootPath, "").Replace("\\", "/"));
            }
            else
            {
                return path;
            }
        }

        /// <summary>
        /// 根据虚拟路径获取物理路径（返回结果最后不带\）
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetPhysicalPath(string url)
        {
            var result = HttpContext.Current.Server.MapPath(url);
            if (!result.EndsWith(":\\") && result.EndsWith("\\"))
            {
                result = result.TrimEnd('\\');
            }
            return result;
        }

        /// <summary>
        /// 根据给定地址和请求类型获取内容
        /// </summary>
        /// <param name="url">请求地址（可直接带参数）</param>
        /// <param name="method">请求方法</param>
        /// <param name="type">请求内容类型</param>
        /// <param name="content">请求内容（不需要就不传）</param>
        /// <returns></returns>
        public static string GetHttpResponse(string url, HttpRequestMethod method = HttpRequestMethod.GET, HttpRequestContentType type = HttpRequestContentType.Default, string content = null)
        {
            // 创建 WebRequest 对象，WebRequest 是抽象类，定义了规范
            // HttpWebRequest 是 WebRequest 的派生类，专门用于 HTTP 请求
            var request = (HttpWebRequest)HttpWebRequest.Create(url);

            // 请求的方式通过 Method 属性设置 ，默认为 GET
            // 可以将 Method 属性设置为任何 HTTP 1.1 协议谓词：GET、HEAD、POST、PUT、DELETE、TRACE 或 OPTIONS。
            switch (method)
            {
                case HttpRequestMethod.POST:
                    request.Method = "POST";
                    break;
                case HttpRequestMethod.GET:
                default:
                    request.Method = "GET";
                    break;
            }

            // 设置请求的内容类型
            switch (type)
            {
                case HttpRequestContentType.UrlEncoded:
                    request.ContentType = "application/x-www-form-urlencoded";
                    content = HttpContext.Current.Server.UrlEncode(content);
                    break;
                case HttpRequestContentType.Json:
                    request.ContentType = "application/json";
                    break;
                case HttpRequestContentType.Default:
                default:
                    //request.ContentType = "text/plain";
                    break;
            }

            // 如果有请求内容的话，写入请求流中
            if (!string.IsNullOrWhiteSpace(content))
            {
                // 取得发向服务器的流
                using (var requestStream = request.GetRequestStream())
                {
                    // 使用 POST 方法请求的时候，实际的参数通过请求的 Body 部分以流的形式传送
                    using (var writer = new StreamWriter(requestStream))
                    {
                        writer.Write(content);
                    }

                    // 设置请求参数的长度.
                    request.ContentLength = requestStream.Length;
                }
            }

            #region Cookies暂不需要

            // 还可以在请求中附带 Cookie
            // 但是，必须首先创建 Cookie 容器

            //request.CookieContainer = new CookieContainer();

            //var requestCookie = new System.Net.Cookie("Request", "RequestValue", "/", "localhost");
            //request.CookieContainer.Add(requestCookie);

            #endregion

            HttpWebResponse response;
            try
            {
                // GetResponse 方法才真的发送请求，等待服务器返回
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = ex.Response as HttpWebResponse;
                SignalRHelper.SendMessageToChatRoom("HTTP请求异常", response == null ? ex.Message : response.StatusDescription);
            }

            // 对请求结果的处理
            if (response != null)
            {
                // 请求成功得到以流的形式表示的回应内容
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        string result;

                        //读取返回内容
                        using (var reader = new StreamReader(responseStream))
                        {
                            result = reader.ReadToEnd();
                        }

                        return result;
                    }
                }
            }
            return string.Empty;
        }
    }
}

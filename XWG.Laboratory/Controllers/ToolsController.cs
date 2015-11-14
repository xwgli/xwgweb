using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using XWG.Laboratory.Models;
using Newtonsoft.Json;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace XWG.Laboratory.Controllers
{
    public class ToolsController : Controller
    {
        /// <summary>
        /// 在线小工具
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 当前会话信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetInfo()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("<h3>当前网站地址：" + WebHelper.CurrentHostRootUrl + "</h3>");

            builder.AppendLine("<h3>网站</h3>");
            builder.AppendLine("<div class='well'>");
            builder.AppendLine("服务器名称：" + Server.MachineName);
            builder.AppendLine("<br />网站域名：" + Request.ServerVariables["HTTP_HOST"]);
            builder.AppendLine("<br />网站服务器：" + Request.ServerVariables["SERVER_SOFTWARE"]);
            builder.AppendLine("<br />服务器IP地址：" + Request.ServerVariables["LOCAL_ADDR"]);
            builder.AppendLine("<br />服务器端口：" + Request.ServerVariables["SERVER_PORT"]);
            builder.AppendLine("<br />服务器主机名：" + Request.ServerVariables["SERVER_NAME"]);
            builder.AppendLine("<br />网站物理路径：" + Request.ServerVariables["APPL_PHYSICAL_PATH"]);
            builder.AppendLine("</div>");



            builder.AppendLine("<br /><br /><h3>服务器</h3>");
            builder.AppendLine("<div class='well'>");
            builder.AppendLine(".NET Framework版本：" + Environment.Version);
            builder.AppendLine("<br />操作系统版本：" + Environment.OSVersion);
            builder.AppendLine("<br />CPU架构：" + Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE"));
            builder.AppendLine("<br />CPU个数：" + Environment.ProcessorCount);
            builder.AppendLine("<br />CPU类型：" + Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER"));
            builder.AppendLine("<br />当前用户：" + Environment.UserName);
            builder.AppendLine("<br />系统距上次开机：" + TimeSpan.FromMilliseconds(Environment.TickCount).ToString(@"\共\ dd\天\ hh\小\时\ mm\分\钟\ ss\秒"));
            builder.AppendLine("<br />是否为64位程序：" + (Environment.Is64BitProcess ? "是" : "否"));
            builder.AppendLine("<br />是否为64位处理器：" + (Environment.Is64BitOperatingSystem ? "是" : "否"));
            builder.AppendLine("<br />硬盘分区信息：" + string.Join("，", Environment.GetLogicalDrives()));
            builder.AppendLine("</div>");



            builder.AppendLine("<br /><br /><h3>客户端</h3>");
            builder.AppendLine("<div class='well'>");
            builder.AppendLine("是否为移动设备：" + (Request.Browser.IsMobileDevice ? "是" : "否"));
            if (Request.Browser.IsMobileDevice)
            {
                builder.AppendLine("<br />设备型号：" + Request.Browser.MobileDeviceModel);
                builder.AppendLine("<br />设备厂商：" + Request.Browser.MobileDeviceManufacturer);
            }
            builder.AppendLine("<br />输入类型：" + Request.Browser.InputType);
            builder.AppendLine("<br />客户端平台：" + Request.Browser.Platform);
            builder.AppendLine("<br />ECMAScript版本：" + Request.Browser.EcmaScriptVersion);
            builder.AppendLine("<br />客户端IP地址：" + Request.UserHostAddress);
            builder.AppendLine("<br />客户端端口：" + Request.ServerVariables["REMOTE_PORT"]);
            builder.AppendLine("<br />客户端主机名：" + Request.UserHostName);
            builder.AppendLine("<br />浏览器名称：" + Request.Browser.Browser);
            builder.AppendLine("<br />浏览器版本：" + Request.Browser.Version);
            builder.AppendLine("<br />浏览器代号：" + Request.Browser.Type);
            builder.AppendLine("<br />UserAgent：" + Request.UserAgent);
            builder.AppendLine("<br />Cookies支持：" + (Request.Browser.Cookies ? "是" : "否"));
            builder.AppendLine("<br />Frame支持：" + (Request.Browser.Frames ? "是" : "否"));
            builder.AppendLine("</div>");



            builder.AppendLine("<br /><br /><h3>Cookies：</h3>");
            builder.AppendLine("<div class='well'>");
            builder.AppendLine(JsonConvert.SerializeObject(HttpContext.Request.Cookies));
            builder.AppendLine("</div>");



            builder.AppendLine("<br /><br /><h3>Session：</h3>");
            builder.AppendLine("<div class='well'>");
            builder.AppendLine(JsonConvert.SerializeObject(Session));
            builder.AppendLine("</div>");



            builder.AppendLine("<br /><br /><h3>ServerVariables：</h3>");
            builder.AppendLine("<div class='well'>");
            foreach (var key in Request.ServerVariables.AllKeys)
            {
                builder.AppendLine("<br />" + key + "：" + Request.ServerVariables[key]);
            }
            builder.AppendLine("</div>");



            builder.AppendLine("<br /><br /><h3>EnvironmentVariable：</h3>");
            builder.AppendLine("<div class='well'>");
            foreach (var key in Environment.GetEnvironmentVariables())
            {
                builder.AppendLine("<br />" + "@" + "：" + JsonConvert.SerializeObject(key));
            }
            builder.AppendLine("</div>");



            ViewBag.Content = builder.ToString();



            return View();
        }

        #region 云存储

        /// <summary>
        /// 小网格云存储
        /// </summary>
        /// <returns></returns>
        public ActionResult XWGCloud()
        {
            return View();
        }

        #endregion

        #region 二维码生成/读取

        /// <summary>
        /// 二维码图片下载的物理路径
        /// </summary>
        public static string QRCodeDownloadPath { get { return ConfigHelper.GetPhysicalPath("QRCodeDownloadPath"); } }

        /// <summary>
        /// 二维码图片下载的虚拟路径
        /// </summary>
        public static string QRCodeDownloadVirtualPath { get { return ConfigHelper.GetVirtualPath("QRCodeDownloadPath"); } }

        /// <summary>
        /// 二维码生成/读取
        /// </summary>
        /// <returns></returns>
        public ActionResult QRCode()
        {
            return View();
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="text">要生成的内容</param>
        /// <returns></returns>
        public ActionResult GenerateQRCode(string text)
        {
            BarcodeWriter writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    //禁用ECI（某个二维码规范吧，不知道，好像可能会产生问题，禁用就禁用呗。。。）
                    DisableECI = true,
                    //编码类型
                    CharacterSet = "UTF-8",
                    //图像大小
                    Width = 256,
                    Height = 256,
                    //纠错能力（想在二维码中心加图片，最简单的办法就是提高纠错能力）
                    ErrorCorrection = ErrorCorrectionLevel.M,
                    //图像边距
                    Margin = 0,
                    //不要将内容文本放到生成图像中
                    PureBarcode = true
                }
            };

            //将二维码的生成结果写入图片
            Bitmap bitmap = writer.Write(text);

            ////***方法1：保存成文件提供下载链接***
            //string storePath = QRCodeDownloadPath;
            //if (!Directory.Exists(storePath))
            //{
            //    Directory.CreateDirectory(storePath);
            //}
            //string fileName = string.Format("generate_qrcode_{0}.png", DateTime.Now.ToString("yyyyMMddhhmmssfff"));
            //bitmap.Save(storePath + "/" + fileName, ImageFormat.Png);

            //var downloadUrl = QRCodeDownloadVirtualPath + fileName;
            //return Content(downloadUrl);

            //***方法2：使用 Data URI 直接显示图片（主流浏览器均支持，IE8以前版本不支持）***
            /* 
             *  简介：Data URI
             *  标准：RFC 2397
             *  格式：data:[][;charset=][;base64],
             *  说明：
             *      data:（URI的协议，表明这是一个data URI）
             *      []（MIME类型，表明了要呈现的数据的类型。拿PNG图片举个例子，它的MIME类型是image/png。如果没有指定，MIME类型将会默认为text/plain。）
             *      [;charset=]（在大多数情况下可以无视，对于图片来说它根本没用。）
             *      [;base64]（指明了使用的编码。不一定要用base 64编码。如果内容不是用base 64进行编码，那么这些数据就会使用标准的URL编码[对URL安全的ASCII字符将会保留原样显示，其他会显示成%xx格式的十六进制编码]进行编码。编码后的数据可能会包含一些没用空格）
             *      ,（这后面就放真正的数据内容了）
             */
            TagBuilder img;
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);

                img = new TagBuilder("img");
                img.MergeAttribute("alt", text);
                img.Attributes.Add("src", String.Format("data:image/png;base64,{0}", Convert.ToBase64String(stream.ToArray())));
            }
            return Content(img.ToString());
        }

        /// <summary>
        /// 读取二维码内容
        /// </summary>
        /// <returns></returns>
        public ActionResult DecodeQRCode()
        {
            //获取上传文件
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                if (file != null)
                {
                    //检查文件扩展名
                    Regex regex = new Regex(".jpg$|.png$", RegexOptions.IgnoreCase);
                    bool isFileOk = regex.IsMatch(file.FileName);
                    if (isFileOk)
                    {
                        try
                        {
                            //获取图片内容
                            Bitmap bmp = new Bitmap(file.InputStream);

                            //新建读取器
                            BarcodeReader reader = new BarcodeReader
                            {
                                //自动旋转图像？
                                AutoRotate = false,
                                Options =
                                {
                                    //更努力尝试去解析？
                                    TryHarder = false
                                }
                            };

                            //解码二维码
                            Result result = reader.Decode(bmp);

                            //返回解析到的文本内容
                            return Content(result.Text);
                        }
                        catch (Exception exception)
                        {
                            Trace.WriteLine(exception);
                        }
                    }
                }
            }
            return Content("上传文件有误。");
        }

        #endregion

        #region Url编码/解码

        /// <summary>
        /// Url编码/解码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EncodeUrl()
        {
            return View();
        }

        /// <summary>
        /// 服务器端Url编码/解码
        /// </summary>
        /// <param name="source">要处理的字符串</param>
        /// <param name="encode">是否进行编码</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EncodeUrl(string source, bool encode)
        {
            if (encode)
            {
                return Content(Server.UrlEncode(source));
            }
            else
            {
                return Content(Server.UrlDecode(source));
            }
        }

        #endregion

        #region Unicode编码/解码

        /// <summary>
        /// Unicode编码/解码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EncodeUnicode()
        {
            return View();
        }

        /// <summary>
        /// 服务器端Unicode编码/解码
        /// </summary>
        /// <param name="source">要处理的字符串</param>
        /// <param name="encode">是否进行编码</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EncodeUnicode(string source, bool encode)
        {
            if (encode)
            {
                return Content(source.EncodeUnicode());
            }
            else
            {
                return Content(source.DecodeUnicode());
            }
        }

        #endregion

        #region Html编码/解码

        /// <summary>
        /// Html编码/解码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EncodeHtml()
        {
            return View();
        }

        /// <summary>
        /// 服务器端Html编码/解码
        /// </summary>
        /// <param name="source">要处理的字符串</param>
        /// <param name="encode">是否进行编码</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EncodeHtml(string source, bool encode)
        {
            if (encode)
            {
                return Content(Server.HtmlEncode(source));
            }
            else
            {
                return Content(Server.HtmlDecode(source));
            }
        }

        #endregion

        #region 格式化JSON

        /// <summary>
        /// 格式化JSON
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult FormatJSON()
        {
            return View();
        }

        /// <summary>
        /// 格式化JSON
        /// </summary>
        /// <param name="content">要格式化的JSON内容</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FormatJSON(string content)
        {
            string json;

            try
            {
                var data = JsonConvert.DeserializeObject(content);
                json = JsonHelper.SerializeObjectWithFormat(data);
            }
            catch (Exception exception)
            {
                json = JsonHelper.SerializeObjectWithFormat(exception);
            }

            return Content(json);
        }

        #endregion
    }
}
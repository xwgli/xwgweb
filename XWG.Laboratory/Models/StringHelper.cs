using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace XWG.Laboratory.Models
{
    /// <summary>
    /// 字符串辅助类
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// 计算字符串的哈希值(SHA1)
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns></returns>
        public static string GetSHA1(this string str)
        {
            //以字节方式存储
            byte[] data = Encoding.Default.GetBytes(str);
            //sha1计算器
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            //sha1 = new SHA1Managed();

            MD5 md5 = new MD5CryptoServiceProvider();

            Aes aes = new AesCryptoServiceProvider();
            DES des = new DESCryptoServiceProvider();
            RC2 rc2 = new RC2CryptoServiceProvider();
            
            DSA dsa = new DSACryptoServiceProvider();
            RSA rsa = new RSACryptoServiceProvider();

            //得到哈希值
            byte[] result = sha1.ComputeHash(data);
            //转换成为字符串的显示
            return BitConverter.ToString(result).Replace("-", "");
        }

        /// <summary>
        /// 将 普通日期时间表示 转换为 Unix时间表示（从 [GMT/UTC的?] “1970年1月1日 0时0分0秒”开始所经过的秒数，不考虑闰秒）
        /// 参考：http://baike.baidu.com/link?url=842W6-fL7vRo8sXlF9BygRUnnTPdrB4XbgiPZHHLiD_cPxcCpTKdpGQpBzuGHO_Y
        /// </summary>
        /// <param name="datetime">要转换的日期时间</param>
        /// <returns></returns>
        public static string GetUnixTimeString(DateTime? datetime = null)
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return ((long)((datetime ?? DateTime.Now) - startTime).TotalSeconds).ToString();
        }

        /// <summary>
        /// 将 Unix时间表示 转换为 普通日期时间表示
        /// </summary>
        /// <param name="seconds">Unix时间（秒数）</param>
        /// <param name="format">格式化时间</param>
        /// <returns></returns>
        public static string GetDateTimeByUnixTime(this string seconds, string format)
        {
            return GetDateTimeByUnixTime(seconds).ToString(format);
        }

        /// <summary>
        /// 将 Unix时间表示 转换为 普通日期时间表示
        /// </summary>
        /// <param name="seconds">Unix时间（秒数）</param>
        /// <returns></returns>
        public static DateTime GetDateTimeByUnixTime(this string seconds)
        {
            long totalSeconds;
            if (long.TryParse(seconds, out totalSeconds))
            {
                return GetDateTimeByUnixTime(totalSeconds);
            }
            return new DateTime(1, 1, 1);
        }

        /// <summary>
        /// 将 Unix时间表示 转换为 普通日期时间表示
        /// </summary>
        /// <param name="seconds">Unix时间（秒数）</param>
        /// <returns></returns>
        public static DateTime GetDateTimeByUnixTime(long seconds)
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            var result = startTime.AddSeconds(seconds);
            return result;
        }

        /// <summary>
        /// 汉字转换为Unicode编码
        /// </summary>
        /// <param name="source">要编码的汉字字符串</param>
        /// <returns></returns>
        public static string EncodeUnicode(this string source)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(source);
            string result = "";
            for (int i = 0; i < bytes.Length; i += 2)
            {
                result += "\\u" + bytes[i + 1].ToString("x").PadLeft(2, '0') + bytes[i].ToString("x").PadLeft(2, '0');
            }
            return result;
        }

        /// <summary>
        /// 将Unicode编码转换为汉字字符串
        /// </summary>
        /// <param name="source">Unicode编码字符串</param>
        /// <returns></returns>
        public static string DecodeUnicode(this string source)
        {
            string result = "";
            MatchCollection mc = Regex.Matches(source, @"\\u([\w]{2})([\w]{2})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            byte[] tempBytes = new byte[2];
            foreach (Match m in mc)
            {
                tempBytes[0] = (byte)int.Parse(m.Groups[2].Value, NumberStyles.HexNumber);
                tempBytes[1] = (byte)int.Parse(m.Groups[1].Value, NumberStyles.HexNumber);
                result += Encoding.Unicode.GetString(tempBytes);
            }
            return result;
        }
    }
}
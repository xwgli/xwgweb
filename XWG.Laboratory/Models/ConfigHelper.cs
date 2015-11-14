using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XWG.Laboratory.Models
{
    /// <summary>
    /// 虚拟配置项集合
    /// </summary>
    public class ConfigCollection
    {
        /// <summary>
        /// 获取网站配置
        /// </summary>
        /// <param name="key">配置的KEY</param>
        /// <returns></returns>
        public string this[string key]
        {
            get
            {
                return ConfigHelper.GetSettings(key);
            }
        }
    }

    /// <summary>
    /// 虚拟连接字符串集合
    /// </summary>
    public class ConnStrCollection
    {
        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="name">连接字符串的NAME</param>
        /// <returns></returns>
        public string this[string name]
        {
            get
            {
                return ConfigHelper.GetConnStr(name);
            }
        }
    }

    /// <summary>
    /// 配置文件辅助类
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// 虚拟配置项集合
        /// </summary>
        public static ConfigCollection Settings = new ConfigCollection();

        /// <summary>
        /// 虚拟连接字符串集合
        /// </summary>
        public static ConnStrCollection ConnStrs = new ConnStrCollection();

        /// <summary>
        /// 从配置文件中读取连接字符串
        /// </summary>
        /// <param name="name">连接字符串名</param>
        /// <returns></returns>
        public static string GetConnStr(string name)
        {
            if (ConfigurationManager.ConnectionStrings[name] != null)
            {
                return ConfigurationManager.ConnectionStrings[name].ConnectionString;
            }
            return string.Empty;
        }

        /// <summary>
        /// 从配置文件中读取配置
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        public static string GetSettings(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// 从配置文件中读取虚拟路径配置
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        public static string GetVirtualPath(string key)
        {
            string path = ConfigHelper.Settings[key];
            path = WebHelper.GetRelativeUrl(path);
            return path;
        }

        /// <summary>
        /// 从配置文件中读取物理路径配置
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        public static string GetPhysicalPath(string key)
        {
            string path = ConfigHelper.Settings[key];
            path = WebHelper.GetPhysicalPath(path);
            return path;
        }
    }
}

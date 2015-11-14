using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using Newtonsoft.Json;

namespace XWG.Laboratory.Models
{
    /// <summary>
    /// 分页查询结果类
    /// </summary>
    public class PagerResult
    {
        /// <summary>
        /// 每页的数据条数
        /// </summary>
        public int RowCount;

        /// <summary>
        /// 当前页码（从1开始）
        /// </summary>
        public int CurrentPage;

        private int _total;
        /// <summary>
        /// 数据总条数（如果RowCount大于0，会根据RowCount校正CurrentPage）
        /// </summary>
        public int Total
        {
            get
            {
                return this._total;
            }
            set
            {
                this._total = value;
                this.CheckLastPage();
            }
        }

        /// <summary>
        /// 查询结果数据
        /// </summary>
        public object Items;

        /// <summary>
        /// 初始化分页结果类
        /// </summary>
        /// <param name="current">当前页码（从1开始）</param>
        /// <param name="count">每页的数据条数</param>
        public PagerResult(int current, int count)
        {
            this.CurrentPage = current;
            this.RowCount = count;
        }

        /// <summary>
        /// 通过查询到的数据总条数检查当前获取的页数是否超出范围
        /// 如果超出范围，自动将页数设为有效范围内的最后一页
        /// </summary>
        private void CheckLastPage()
        {
            if (this.RowCount > 0)
            {
                var last = Math.Ceiling(((double)this._total) / this.RowCount);
                if (this.CurrentPage > last)
                {
                    this.CurrentPage = (int)last;
                }
            }
        }
    }


    /// <summary>
    /// 分页查询参数类
    /// </summary>
    public class PagerParam
    {
        private int _current;
        /// <summary>
        /// 当前页码（从1开始）
        /// </summary>
        public int CurrentPage
        {
            get { return _current; }
            set { _current = value < 1 ? 1 : value; }
        }

        /// <summary>
        /// 每页的数据条数
        /// </summary>
        public int RowCount;

        /// <summary>
        /// 计算跳过的条数
        /// </summary>
        public int SkipCount
        {
            get { return (CurrentPage - 1) * RowCount; }
        }

        #region 查询选项相关

        private Dictionary<string, string> _options;

        /// <summary>
        /// 查询的其它选项
        /// </summary>
        public Dictionary<string, string> QueryOptions
        {
            get { return _options ?? (_options = new Dictionary<string, string>()); }
            set { _options = value; }
        }

        /// <summary>
        /// 获取查询的其他选项
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public string this[string key]
        {
            get
            {
                if (QueryOptions.ContainsKey(key))
                {
                    return QueryOptions[key];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (QueryOptions.ContainsKey(key))
                {
                    QueryOptions[key] = value;
                }
                else
                {
                    QueryOptions.Add(key, value);
                }
            }
        }

        /// <summary>
        /// 是否含有查询选项
        /// </summary>
        public bool HasQueryOptions
        {
            get
            {
                if ((_options != null) && (_options.Any()))
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 从"查询的其它选项"中获取保存的数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public bool TryGetQueryOptions(string key, out string data)
        {
            data = null;

            if (HasQueryOptions)
            {
                if (QueryOptions.ContainsKey(key))
                {
                    data = QueryOptions[key];
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 从"查询的其它选项"中获取保存的数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public bool TryGetQueryOptions<T>(string key, out T data)
            where T : struct
        {
            data = default(T);

            if (HasQueryOptions)
            {
                //查询是否已启用
                if (QueryOptions.ContainsKey(key))
                {
                    var val = QueryOptions[key];

                    if (val != null)
                    {
                        Type tp = typeof (T);

                        //泛型Enum判断，做单独的处理
                        if (tp.IsEnum)
                        {
                            return Enum.TryParse(val, out data);
                        }

                        //泛型Nullable判断，取其中的类型
                        if (tp.IsGenericType)
                        {
                            tp = tp.GetGenericArguments()[0];
                        }

                        //反射获取TryParse方法
                        var TryParse = tp.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static,
                            Type.DefaultBinder,
                            new Type[] {typeof (string), tp.MakeByRefType()},
                            new ParameterModifier[] {new ParameterModifier(2)});
                        var parameters = new object[] {val, Activator.CreateInstance(tp)};
                        bool success = (bool) TryParse.Invoke(null, parameters);

                        //成功返回转换后的值，否则返回类型的默认值
                        if (success)
                        {
                            data = (T) parameters[1];
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 将查询选项从字符串转为字典键值对（json格式）
        /// </summary>
        /// <param name="options">查询选项</param>
        /// <returns></returns>
        public static Dictionary<string, string> ConvertToQueryOptions(string options)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(options);
                return result;
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);
                return null;
            }
        }

        #endregion

        public PagerParam()
        {

        }

        /// <summary>
        /// 初始化分页查询参数
        /// </summary>
        /// <param name="currentPage">当前页码（从1开始）</param>
        /// <param name="rowCount">每页的数据条数</param>
        public PagerParam(int currentPage, int rowCount)
        {
            this.CurrentPage = currentPage;
            this.RowCount = rowCount;
        }

        /// <summary>
        /// 初始化分页查询参数
        /// </summary>
        /// <param name="currentPage">当前页码（从1开始）</param>
        /// <param name="rowCount">每页的数据条数</param>
        /// <param name="queryOptions">查询的其它选项</param>
        public PagerParam(int currentPage, int rowCount, string queryOptions)
        {
            this.CurrentPage = currentPage;
            this.RowCount = rowCount;
            this.QueryOptions = PagerParam.ConvertToQueryOptions(queryOptions);
        }

        /// <summary>
        /// 根据当前分页查询参数生成分页查询结果对象
        /// </summary>
        /// <returns></returns>
        public PagerResult GetPagerResult()
        {
            return new PagerResult(this.CurrentPage, this.RowCount);
        }

        /// <summary>
        /// 通过查询到的数据总条数检查当前获取的页数是否超出范围
        /// 如果超出范围，自动将页数设为有效范围内的最后一页
        /// </summary>
        public void CheckLastPage(int total)
        {
            if (this.RowCount > 0)
            {
                var last = Math.Ceiling(((double)total) / this.RowCount);
                if (this.CurrentPage > last)
                {
                    this.CurrentPage = (int)last;
                }
            }
        }
    }
}
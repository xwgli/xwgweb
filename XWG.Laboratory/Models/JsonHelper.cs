using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace XWG.Laboratory.Models
{
    /// <summary>
    /// 纯日期JSON转换器
    /// </summary>
    public class JsonDateConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value != null)
            {
                string str = reader.Value.ToString();
                DateTime result;
                if (DateTime.TryParse(str, out result))
                {
                    return result;
                }
            }
            return new DateTime();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string str = "2012-12-21";
            if (value is DateTime)
            {
                DateTime dateTime = (DateTime)value;
                str = dateTime.ToString("yyyy-MM-dd");
            }
            writer.WriteValue(str);
        }
    }

    /// <summary>
    /// JSON辅助类
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// 序列化传入数据，输出JSON字符串带缩进格式
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static string SerializeObjectWithFormat(object data)
        {
            JsonSerializer serializer = new JsonSerializer();

            StringWriter textWriter = new StringWriter();
            var jsonWriter = new JsonTextWriter(textWriter)
            {
                Formatting = Formatting.Indented,
                Indentation = 4,
                IndentChar = ' '
            };

            serializer.Serialize(jsonWriter, data);

            var json = textWriter.ToString();
            return json;
        }
    }
}
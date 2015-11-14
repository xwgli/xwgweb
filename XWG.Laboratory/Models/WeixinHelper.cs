using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XWG.Laboratory.Models
{
    /// <summary>
    /// 自定义的Session类型
    /// </summary>
    public class XwgSession
    {
        /// <summary>
        /// 存储Session数据
        /// </summary>
        private Dictionary<string, object> Session;

        /// <summary>
        /// Session中键的集合
        /// </summary>
        public Dictionary<string, object>.KeyCollection Keys
        {
            get
            {
                return Session.Keys;
            }
        }

        /// <summary>
        /// 获取Session指定键对应的值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
                if (!Session.ContainsKey(key))
                {
                    return null;
                }
                return Session[key];
            }
            set
            {
                if (!Session.ContainsKey(key))
                {
                    Session.Add(key, value);
                }
                else
                {
                    Session[key] = value;
                }
            }
        }

        /// <summary>
        /// 初始化自定义Session
        /// </summary>
        public XwgSession()
        {
            Session = new Dictionary<string, object>();
        }

        /// <summary>
        /// 从Session中删除指定的键值对
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                Session.Clear();
            }
            else
            {
                Session.Remove(key);
            }
        }
    }

    /// <summary>
    /// 微信辅助类
    /// </summary>
    public class WeixinHelper
    {
        public static Dictionary<string, XwgSession> Sessions = new Dictionary<string, XwgSession>();

        /// <summary>
        /// 会话Session
        /// </summary>
        private XwgSession Session;

        private string UserIdentity;

        /// <summary>
        /// 初始化时设置Session
        /// </summary>
        public WeixinHelper(string username)
        {
            this.UserIdentity = username;
            //判断session字典中有没有sessionid，没有就加一个
            if (!Sessions.ContainsKey(UserIdentity))
            {
                //TODO:多并发会导致资源争抢，怎么解决？？？
                lock (Sessions)
                {
                    if (!Sessions.ContainsKey(UserIdentity))
                    {
                        Sessions.Add(UserIdentity, new XwgSession());
                    }
                }
            }
            //设置当前的session对象
            this.Session = Sessions[UserIdentity];
        }

        /// <summary>
        /// 记录微信的收发信息
        /// </summary>
        /// <param name="recive"></param>
        /// <param name="send"></param>
        public static void SaveRecord(string recive, string send)
        {
            XwgWebDBEntities entities = new XwgWebDBEntities();

            tbl_WeixinRecord record = new tbl_WeixinRecord()
            {
                ReciveData = recive,
                ReciveTime = DateTime.Now,
                SendData = send
            };
            entities.tbl_WeixinRecord.Add(record);
            entities.SaveChangesAsync();
        }

        public void SaveTextMessage(string message, string response)
        {
            var user = GetData<tbl_WeixinUser>("UserInfo");
            XwgWebDBEntities entities = new XwgWebDBEntities();

            tbl_WeixinTextMessage record = new tbl_WeixinTextMessage()
            {
                SendTime = DateTime.Now,
                Message = message,
                SendUserId = user.UserId,
                Response = response
            };
            entities.tbl_WeixinTextMessage.Add(record);
            entities.SaveChangesAsync();
        }

        /// <summary>
        /// 获取传入消息的用户信息
        /// </summary>
        /// <returns></returns>
        public tbl_WeixinUser GetFromUser()
        {
            tbl_WeixinUser user = GetData<tbl_WeixinUser>("UserInfo");
            if (user == null)
            {
                XwgWebDBEntities entities = new XwgWebDBEntities();
                user = entities.tbl_WeixinUser.FirstOrDefault(u => u.UserName == UserIdentity);
                if (user == null)
                {
                    var newUser = new tbl_WeixinUser()
                    {
                        CreateTime = DateTime.Now,
                        IsSpecial = false,
                        NickName = string.Empty,
                        RobotName = "哦呀叽",
                        UserName = UserIdentity
                    };

                    entities.tbl_WeixinUser.Add(newUser);
                    entities.SaveChanges();
                }
                else
                {
                    SaveData("UserInfo", user);
                }
            }
            return user;
        }

        /// <summary>
        /// 保存用户昵称
        /// </summary>
        /// <param name="nickname"></param>
        public void SaveNickName(string nickname)
        {
            XwgWebDBEntities entities = new XwgWebDBEntities();
            var user = entities.tbl_WeixinUser.FirstOrDefault(u => u.UserName == UserIdentity);
            if (user == null)
            {
                var newUser = new tbl_WeixinUser()
                {
                    CreateTime = DateTime.Now,
                    IsSpecial = false,
                    NickName = nickname,
                    RobotName = "哦呀叽",
                    UserName = UserIdentity
                };

                entities.tbl_WeixinUser.Add(newUser);
            }
            else
            {
                user.NickName = nickname;
            }
            if (user.NickName.Contains("小白"))
            {
                user.RobotName = "小黑";
                user.IsSpecial = true;
            }
            entities.SaveChangesAsync();
            SaveData("UserInfo", user);
        }

        public void SaveRobotName(string robotname)
        {
            XwgWebDBEntities entities = new XwgWebDBEntities();
            var user = entities.tbl_WeixinUser.FirstOrDefault(u => u.UserName == UserIdentity);
            if (user == null)
            {
                var newUser = new tbl_WeixinUser()
                {
                    CreateTime = DateTime.Now,
                    IsSpecial = false,
                    NickName = string.Empty,
                    RobotName = robotname,
                    UserName = UserIdentity
                };

                entities.tbl_WeixinUser.Add(newUser);
            }
            else
            {
                user.RobotName = robotname;
            }
            entities.SaveChangesAsync();
            SaveData("UserInfo", user);
        }

        public string GetAnswer(string question)
        {
            XwgWebDBEntities entities = new XwgWebDBEntities();
            var result = entities.tbl_WeixinQa.Where(q => q.Question == question);
            if (result.Any())
            {
                return result.First().Answer;
            }
            result = entities.tbl_WeixinQa.Where(q => q.Question.Contains(question) || question.Contains(q.Question));
            if (result.Any())
            {
                return result.First().Answer;
            }
            return string.Empty;
        }

        public void SaveQa(string question, string answer)
        {
            var user = GetData<tbl_WeixinUser>("UserInfo");
            XwgWebDBEntities entities = new XwgWebDBEntities();

            tbl_WeixinQa qa = new tbl_WeixinQa()
            {
                Question = question,
                Answer = answer,
                UserId = user.UserId
            };
            entities.tbl_WeixinQa.Add(qa);
            entities.SaveChangesAsync();
        }

        /// <summary>
        /// 从session中删除数据
        /// </summary>
        /// <param name="key"></param>
        public void RemoveData(string key = null)
        {
            Session.Remove(key);
        }

        /// <summary>
        /// 保存数据到session
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="data">数据</param> 
        public void SaveData<T>(string key, T data)
        {
            Session[key] = data;
        }

        /// <summary>
        /// 从session中获取保存的数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public T GetData<T>(string key)
            where T : class
        {
            var result = Session[key] as T;
            return result;
        }

        /// <summary>
        /// 从session中获取保存的数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public bool GetStruct<T>(string key, out T data)
            where T : struct
        {
            if (Session[key] is T)
            {
                data = (T)Session[key];
                return true;
            }
            data = new T();
            return false;
        }
    }
}
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace XWG.Laboratory.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_WeixinUser
    {
        public tbl_WeixinUser()
        {
            this.tbl_WeixinTextMessage = new HashSet<tbl_WeixinTextMessage>();
            this.tbl_WeixinQa = new HashSet<tbl_WeixinQa>();
        }
    
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string RobotName { get; set; }
        public bool IsSpecial { get; set; }
        public System.DateTime CreateTime { get; set; }
    
        public virtual ICollection<tbl_WeixinTextMessage> tbl_WeixinTextMessage { get; set; }
        public virtual ICollection<tbl_WeixinQa> tbl_WeixinQa { get; set; }
    }
}

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
    
    public partial class tbl_CommentRecord
    {
        public long RecordId { get; set; }
        public long PageId { get; set; }
        public long VisitorId { get; set; }
        public string NickName { get; set; }
        public string Comment { get; set; }
        public System.DateTime CommentTime { get; set; }
    
        public virtual tbl_CommentPage tbl_CommentPage { get; set; }
        public virtual tbl_VisitRecord tbl_VisitRecord { get; set; }
    }
}

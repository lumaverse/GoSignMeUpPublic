//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gsmu.Api.Data.School.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmailAuditTrail
    {
        public int AuditID { get; set; }
        public System.DateTime AuditDate { get; set; }
        public string EmailSubject { get; set; }
        public string AuditProcess { get; set; }
        public string EmailBody { get; set; }
        public string EmailTo { get; set; }
        public string EmailFrom { get; set; }
        public string EmailCC { get; set; }
        public string EmailBCC { get; set; }
        public string LoggedInUser { get; set; }
        public int Pending { get; set; }
        public string AttachmentName { get; set; }
        public string ErrorInfo { get; set; }
        public Nullable<System.DateTime> RetryDateTime { get; set; }
        public string AttachmentNameMemo { get; set; }
        public Nullable<System.DateTime> ResendDateTime { get; set; }
        public string EmailToUserIDs { get; set; }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gsmu.Api.Data.RoomManagement.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class tempDB
    {
        public int ID { get; set; }
        public string SessionID { get; set; }
        public Nullable<System.DateTime> BookDay { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> FinishTime { get; set; }
        public string Room { get; set; }
        public Nullable<System.DateTime> EntryDate { get; set; }
    }
}

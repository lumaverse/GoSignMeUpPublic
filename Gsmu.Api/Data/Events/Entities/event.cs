//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gsmu.Api.Data.Events.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class @event
    {
        public int eventid { get; set; }
        public Nullable<System.DateTime> Insertdate { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string featuredspeaker { get; set; }
        public string Location { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public string Eventtime { get; set; }
        public string ContactInfo { get; set; }
        public string ContactEmail { get; set; }
        public string WebsiteLink { get; set; }
        public string Display { get; set; }
        public Nullable<int> Priority { get; set; }
        public string Fees { get; set; }
        public string NeedsReview { get; set; }
        public Nullable<int> CID { get; set; }
        public string rrequest { get; set; }
        public string requestinfo { get; set; }
    }
}

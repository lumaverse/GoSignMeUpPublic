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
    
    public partial class RoomWaitingList
    {
        public int RoomWaitingListId { get; set; }
        public int Room_id { get; set; }
        public int member_id { get; set; }
        public string email { get; set; }
        public Nullable<System.DateTime> hour_start { get; set; }
        public Nullable<System.DateTime> Hour_end { get; set; }
        public int NotificationType { get; set; }
        public Nullable<System.DateTime> Start_Day { get; set; }
        public Nullable<System.DateTime> End_Day { get; set; }
        public int Week_end { get; set; }
        public int Period { get; set; }
        public int WeekInterval { get; set; }
        public string WeekdayName { get; set; }
        public int SetupTime { get; set; }
        public int TakedownTime { get; set; }
        public string txtmember_id { get; set; }
        public string title { get; set; }
        public string organization { get; set; }
        public string description { get; set; }
        public string PersonInCharge { get; set; }
        public string PersonInChargePhone { get; set; }
        public int PeopleAttending { get; set; }
        public int RoomSetup { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public Nullable<int> notifyWLuser { get; set; }
    }
}
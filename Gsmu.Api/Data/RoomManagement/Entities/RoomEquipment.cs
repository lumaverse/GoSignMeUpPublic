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
    
    public partial class RoomEquipment
    {
        public int EquipmentId { get; set; }
        public string EquipmentTitle { get; set; }
        public string EquipmentDescription { get; set; }
        public Nullable<int> tCount { get; set; }
        public Nullable<int> RequireApproval { get; set; }
    }
}
﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class RoomManagementEntities : DbContext
    {
        public RoomManagementEntities()
            : base("name=RoomManagementEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<BuildingDetail> BuildingDetails { get; set; }
        public DbSet<config> configs { get; set; }
        public DbSet<email_components> email_components { get; set; }
        public DbSet<floor> floors { get; set; }
        public DbSet<FloorsScrollerConfig> FloorsScrollerConfigs { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<listing_countries> listing_countries { get; set; }
        public DbSet<listing_hours> listing_hours { get; set; }
        public DbSet<member> members { get; set; }
        public DbSet<multi_days_booking> multi_days_booking { get; set; }
        public DbSet<room> rooms { get; set; }
        public DbSet<room_booking> room_booking { get; set; }
        public DbSet<RoomAudience> RoomAudiences { get; set; }
        public DbSet<RoomEqApproval> RoomEqApprovals { get; set; }
        public DbSet<RoomEquipment> RoomEquipments { get; set; }
        public DbSet<RoomSetup> RoomSetups { get; set; }
        public DbSet<tempDB> tempDBs { get; set; }
        public DbSet<RoomWaitingList> RoomWaitingLists { get; set; }
    }
}

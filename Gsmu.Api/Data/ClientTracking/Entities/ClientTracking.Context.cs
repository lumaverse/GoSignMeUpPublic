﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gsmu.Api.Data.ClientTracking.Entities
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ClientTrackingEntities : DbContext
    {
        public ClientTrackingEntities()
            : base("name=ClientTrackingEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<GosignmeupRegistration> GosignmeupRegistrations { get; set; }
        public DbSet<GosignmeupRegStatClient> GosignmeupRegStatClients { get; set; }
        public DbSet<GosignmeupStudentCount> GosignmeupStudentCounts { get; set; }
        public DbSet<Newsletter> Newsletters { get; set; }
        public DbSet<NewsletterTag> NewsletterTags { get; set; }
        public DbSet<PerformanceStat> PerformanceStats { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<AssessmentQuestion> AssessmentQuestions { get; set; }
    }
}

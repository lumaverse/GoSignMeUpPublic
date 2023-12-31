﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gsmu.Api.Data.Survey.Entities
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SurveyEntities : DbContext
    {
        public SurveyEntities()
            : base("name=SurveyEntities")
        {
        }
    	public SurveyEntities(string connectionString) : base(connectionString)
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<AnswerComment> AnswerComments { get; set; }
        public DbSet<AnswerMultiple> AnswerMultiples { get; set; }
        public DbSet<AnswerTextArea> AnswerTextAreas { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<CourseSurvey> CourseSurveys { get; set; }
        public DbSet<dtproperty> dtproperties { get; set; }
        public DbSet<masterinfo> masterinfoes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionAnswerChoice> QuestionAnswerChoices { get; set; }
        public DbSet<ReportRequest> ReportRequests { get; set; }
        public DbSet<ReportSetting> ReportSettings { get; set; }
        public DbSet<SupplementalQuestion> SupplementalQuestions { get; set; }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<User> Users { get; set; }
    }
}

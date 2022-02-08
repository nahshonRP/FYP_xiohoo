using CourseMangement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;


namespace BOL.DBContext
{
    public class AppDBContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>  // IdentityDbContext<ApplicationUser>
    {

        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Course>()
                .HasOne(s => s.CourseCategories)
                .WithMany(w => w.Courses)
                .HasForeignKey(s => s.FkCourseCategoryId);

            modelBuilder.Entity<CourseSurvey>()
               .HasOne(s => s.Course)
               .WithOne(w => w.CourseSurvey)
               .HasForeignKey<CourseSurvey>(s => s.FkCourceId);


            modelBuilder.Entity<CourseSurvey>()
              .HasOne(s => s.Survey)
              .WithOne(w => w.CourseSurvey)
              .HasForeignKey<CourseSurvey>(s => s.FkSurveyId);

            modelBuilder.Entity<SurveyQuestions>()
             .HasOne(s => s.Question)
             .WithMany(w => w.SurveyQuestions)
             .HasForeignKey(s => s.FkQuestionId);

            modelBuilder.Entity<SurveyQuestions>()
            .HasOne(s => s.Survey)
            .WithMany(w => w.SurveyQuestions)
            .HasForeignKey(s => s.FkSurveyId);

            modelBuilder.Entity<UsersSurvey>()
           .HasOne(s => s.User)
           .WithMany(w => w.UsersSurveys)
           .HasForeignKey(s => s.FkUserId);

            modelBuilder.Entity<UsersSurvey>()
           .HasOne(s => s.CourseSurvey)
           .WithMany(w => w.UsersSurveys)
           .HasForeignKey(s => s.FkCourseSurveyId);


            modelBuilder.Entity<UsersSurvey>()
           .HasMany(s => s.UsersSurveyDetails)
           .WithOne(w => w.UsersSurvey)
           .HasForeignKey(s => s.FkUserSruveyID);





            modelBuilder.ApplyConfiguration(new QuestionsEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CourceCategoryEntityConfiguration());
        }

        public DbSet<CourseMangement.Models.Course> Course { get; set; }
        public DbSet<CourseMangement.Models.CourseCategory> CourseCategories { get; set; }
        public DbSet<CourseMangement.Models.CourseAssignee> CourseAssignees { get; set; }
        public DbSet<CourseAttendance> CourseAttendances { get; set; }
        public DbSet<Questions> Questions { get; set; }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<SurveyQuestions> SurveyQuestions { get; set; }
        //public DbSet<SurveyUsers> SurveyUsers { get; set; }
        public DbSet<CourseSurvey> CourseSurveys { get; set; }
        public DbSet<UsersSurvey> UsersSurveys { get; set; }
        public DbSet<UsersSurveyDetails> UsersSurveyDetails { get; set; }

    }


    public static class Dbextension
    {

        //public static void EnsureSeed(this AppDBContext db, IServiceProvider services,string notfounfimage, RoleManager<IdentityRole<int>> RoleManager,DataTable dt=null) {
        //    DataSeeder.Seeddata(db,notfounfimage,RoleManager);
        //}
    }

    public class CourceCategoryEntityConfiguration : IEntityTypeConfiguration<CourseCategory>
    {
        public void Configure(EntityTypeBuilder<CourseCategory> builder)
        {
            builder.HasData(
                new CourseCategory() { Id = 1, Name = "Music" },
                new CourseCategory() { Id = 2, Name = "Art" },
                new CourseCategory() { Id = 3, Name = "Electrition" },
                new CourseCategory() { Id = 4, Name = "SOcial Science" }
            );
        }
    }
    public class QuestionsEntityConfiguration : IEntityTypeConfiguration<Questions>
    {
        public void Configure(EntityTypeBuilder<Questions> builder)
        {
            builder.HasData(
                new Questions() { Id = 1, Name = "The program was relevant and interesting" },
                new Questions() { Id = 2, Name = "I can apply the ideas /skills learnt in my work" },
                new Questions() { Id = 3, Name = "Explanations were clear" },
                new Questions() { Id = 4, Name = "There was good interaction between the trainer and the participants" },
                new Questions() { Id = 5, Name = "The trainer was well prepared" },
                new Questions() { Id = 6, Name = "I have learnt useful ideas" },
                new Questions() { Id = 7, Name = "I am satisfied with the course/workshop/seminar" },
                new Questions() { Id = 8, Name = "I fell Good" },
                new Questions() { Id = 9, Name = "It was worthy" },
                new Questions() { Id = 10, Name = "Subject explanation was great" }
            );
        }
    }
}

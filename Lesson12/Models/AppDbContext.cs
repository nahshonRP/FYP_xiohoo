using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Lesson12.Models
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Lesson> Lesson { get; set; }
        public virtual DbSet<Module> Module { get; set; }
        public virtual DbSet<Topic> Topic { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.HasKey(e => new { e.ModuleId, e.LessonId })
                    .HasName("PK__Lesson__307C3D6A2AC4DBDA");

                entity.Property(e => e.ModuleId)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Attachments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Notes)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.TopicId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.Lesson)
                    .HasForeignKey(d => d.ModuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Lesson__ModuleId__286302EC");

                entity.HasOne(d => d.Topic)
                    .WithMany(p => p.Lesson)
                    .HasForeignKey(d => d.TopicId)
                    .HasConstraintName("FK__Lesson__TopicId__29572725");
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.Property(e => e.ModuleId)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Topic>(entity =>
            {
                entity.Property(e => e.TopicId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ModuleId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.Topic)
                    .HasForeignKey(d => d.ModuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Topic__ModuleId__25869641");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LMS.Models.LMSModels
{
    public partial class Team112LMSContext : DbContext
    {
        public Team112LMSContext()
        {
        }

        public Team112LMSContext(DbContextOptions<Team112LMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrators> Administrators { get; set; }
        public virtual DbSet<AssignmentCategories> AssignmentCategories { get; set; }
        public virtual DbSet<Assignments> Assignments { get; set; }
        public virtual DbSet<Classes> Classes { get; set; }
        public virtual DbSet<Courses> Courses { get; set; }
        public virtual DbSet<Departments> Departments { get; set; }
        public virtual DbSet<Enrollments> Enrollments { get; set; }
        public virtual DbSet<Professors> Professors { get; set; }
        public virtual DbSet<Students> Students { get; set; }
        public virtual DbSet<Submitted> Submitted { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#pragma warning disable CS1030 // #warning directive
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("Server=atr.eng.utah.edu;User Id=u1255186;Password=password;Database=Team112LMS");
#pragma warning restore CS1030 // #warning directive
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrators>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.Property(e => e.UId).HasColumnName("uID");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.FName)
                    .IsRequired()
                    .HasColumnName("fName")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Lname)
                    .IsRequired()
                    .HasColumnName("LName")
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<AssignmentCategories>(entity =>
            {
                entity.HasKey(e => e.CattId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.ClassId)
                    .HasName("classID");

                entity.HasIndex(e => new { e.CattName, e.ClassId })
                    .HasName("cattName")
                    .IsUnique();

                entity.Property(e => e.CattId).HasColumnName("cattID");

                entity.Property(e => e.CattName)
                    .IsRequired()
                    .HasColumnName("cattName")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.ClassId).HasColumnName("classID");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.AssignmentCategories)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AssignmentCategories_ibfk_1");
            });

            modelBuilder.Entity<Assignments>(entity =>
            {
                entity.HasKey(e => e.AssignId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.CattId)
                    .HasName("cattID");

                entity.HasIndex(e => new { e.AssignName, e.CattId })
                    .HasName("assignName")
                    .IsUnique();

                entity.Property(e => e.AssignId).HasColumnName("assignID");

                entity.Property(e => e.AssignName)
                    .IsRequired()
                    .HasColumnName("assignName")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.CattId).HasColumnName("cattID");

                entity.Property(e => e.Contents).HasColumnType("varchar(8192)");

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.HasOne(d => d.Catt)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.CattId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Assignments_ibfk_1");
            });

            modelBuilder.Entity<Classes>(entity =>
            {
                entity.HasKey(e => e.ClassId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Teacher)
                    .HasName("Teacher");

                entity.HasIndex(e => new { e.CourseId, e.Semester, e.Teacher })
                    .HasName("courseID")
                    .IsUnique();

                entity.Property(e => e.ClassId).HasColumnName("classID");

                entity.Property(e => e.CourseId).HasColumnName("courseID");

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Semester)
                    .IsRequired()
                    .HasColumnType("varchar(11)");

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Classes_ibfk_1");

                entity.HasOne(d => d.TeacherNavigation)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.Teacher)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Classes_ibfk_2");
            });

            modelBuilder.Entity<Courses>(entity =>
            {
                entity.HasKey(e => e.CourseId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => new { e.DprtAbv, e.CourseNum })
                    .HasName("dprtAbv")
                    .IsUnique();

                entity.Property(e => e.CourseId).HasColumnName("courseID");

                entity.Property(e => e.CourseName)
                    .IsRequired()
                    .HasColumnName("courseName")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.CourseNum).HasColumnName("courseNum");

                entity.Property(e => e.DprtAbv)
                    .IsRequired()
                    .HasColumnName("dprtAbv")
                    .HasColumnType("varchar(4)");

                entity.HasOne(d => d.DprtAbvNavigation)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.DprtAbv)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Courses_ibfk_1");
            });

            modelBuilder.Entity<Departments>(entity =>
            {
                entity.HasKey(e => e.DprtAbv)
                    .HasName("PRIMARY");

                entity.Property(e => e.DprtAbv)
                    .HasColumnName("dprtAbv")
                    .HasColumnType("varchar(4)");

                entity.Property(e => e.DprtName)
                    .IsRequired()
                    .HasColumnName("dprtName")
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<Enrollments>(entity =>
            {
                entity.HasKey(e => e.EId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.ClassId)
                    .HasName("classID");

                entity.HasIndex(e => new { e.UId, e.ClassId })
                    .HasName("uID")
                    .IsUnique();

                entity.Property(e => e.EId).HasColumnName("eID");

                entity.Property(e => e.ClassId).HasColumnName("classID");

                entity.Property(e => e.Grade).HasColumnType("varchar(2)");

                entity.Property(e => e.UId).HasColumnName("uID");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Enrollments_ibfk_2");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Enrollments_ibfk_1");
            });

            modelBuilder.Entity<Professors>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.DprtAbv)
                    .HasName("dprtAbv");

                entity.Property(e => e.UId).HasColumnName("uID");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.DprtAbv)
                    .IsRequired()
                    .HasColumnName("dprtAbv")
                    .HasColumnType("varchar(4)");

                entity.Property(e => e.FName)
                    .IsRequired()
                    .HasColumnName("fName")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Lname)
                    .IsRequired()
                    .HasColumnName("LName")
                    .HasColumnType("varchar(100)");

                entity.HasOne(d => d.DprtAbvNavigation)
                    .WithMany(p => p.Professors)
                    .HasForeignKey(d => d.DprtAbv)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Professors_ibfk_1");
            });

            modelBuilder.Entity<Students>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.DprtAbv)
                    .HasName("dprtAbv");

                entity.Property(e => e.UId).HasColumnName("uID");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.DprtAbv)
                    .IsRequired()
                    .HasColumnName("dprtAbv")
                    .HasColumnType("varchar(4)");

                entity.Property(e => e.FName)
                    .IsRequired()
                    .HasColumnName("fName")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Lname)
                    .IsRequired()
                    .HasColumnName("LName")
                    .HasColumnType("varchar(100)");

                entity.HasOne(d => d.DprtAbvNavigation)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.DprtAbv)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Students_ibfk_1");
            });

            modelBuilder.Entity<Submitted>(entity =>
            {
                entity.HasKey(e => e.SubId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.AssignId)
                    .HasName("assignID");

                entity.HasIndex(e => new { e.UId, e.AssignId })
                    .HasName("uID")
                    .IsUnique();

                entity.Property(e => e.SubId).HasColumnName("subID");

                entity.Property(e => e.AssignId).HasColumnName("assignID");

                entity.Property(e => e.Sub).HasColumnType("varchar(8192)");

                entity.Property(e => e.SubTime)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UId).HasColumnName("uID");

                entity.HasOne(d => d.Assign)
                    .WithMany(p => p.Submitted)
                    .HasForeignKey(d => d.AssignId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Submitted_ibfk_2");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Submitted)
                    .HasForeignKey(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Submitted_ibfk_1");
            });
        }
    }
}

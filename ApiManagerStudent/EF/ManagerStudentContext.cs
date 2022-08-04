using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ApiManagerStudent.EF
{
    public partial class ManagerStudentContext : DbContext
    {
        public ManagerStudentContext()
        {
        }

        public ManagerStudentContext(DbContextOptions<ManagerStudentContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Class> Classes { get; set; }
        public virtual DbSet<Point> Points { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Subject> Subjects { get; set; }
        public virtual DbSet<Teacher> Teachers { get; set; }
        public virtual DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=ManagerStudentDB");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Class>(entity =>
            {
                entity.ToTable("classes");

                entity.HasIndex(e => e.Alias, "UQ__classes__8C585C04838ED8C0")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Alias)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("alias");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Point>(entity =>
            {
                entity.HasKey(e => new { e.IdStudent, e.IdSubject, e.NumberOfTimes })
                    .HasName("pk_point");

                entity.ToTable("point");

                entity.HasIndex(e => e.Alias, "UQ__point__8C585C0438B0117D")
                    .IsUnique();

                entity.Property(e => e.IdStudent).HasColumnName("idStudent");

                entity.Property(e => e.IdSubject).HasColumnName("idSubject");

                entity.Property(e => e.NumberOfTimes).HasColumnName("numberOfTimes");

                entity.Property(e => e.Alias)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("alias");

                entity.Property(e => e.Points).HasColumnName("points");

                entity.HasOne(d => d.IdStudentNavigation)
                    .WithMany(p => p.Points)
                    .HasForeignKey(d => d.IdStudent)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_point_student");

                entity.HasOne(d => d.IdSubjectNavigation)
                    .WithMany(p => p.Points)
                    .HasForeignKey(d => d.IdSubject)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_point_subject");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");

                entity.HasIndex(e => e.Alias, "UQ__role__8C585C04E5293B35")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Alias)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("alias");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("student");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Alias)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("alias");

                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("date")
                    .HasColumnName("dateOfBirth");

                entity.Property(e => e.IdClass).HasColumnName("idClass");

                entity.Property(e => e.Image)
                    .HasMaxLength(100)
                    .HasColumnName("image");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.HasOne(d => d.IdClassNavigation)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.IdClass)
                    .HasConstraintName("fk_student_classes");
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.ToTable("subject");

                entity.HasIndex(e => e.Alias, "UQ__subject__8C585C04E1D88422")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Alias)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("alias");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.ToTable("teacher");

                entity.HasIndex(e => e.Username, "UQ__teacher__F3DBC5721786E63F")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("date")
                    .HasColumnName("dateOfBirth");

                entity.Property(e => e.IdRole).HasColumnName("idRole");

                entity.Property(e => e.Image)
                    .HasMaxLength(100)
                    .HasColumnName("image");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.Username)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("username");

                entity.HasOne(d => d.IdRoleNavigation)
                    .WithMany(p => p.Teachers)
                    .HasForeignKey(d => d.IdRole)
                    .HasConstraintName("fk_teacher_role");
            });

            modelBuilder.Entity<UserRefreshToken>(entity =>
            {
                entity.ToTable("UserRefreshToken");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("createdDate");

                entity.Property(e => e.ExpirationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("expirationDate");

                entity.Property(e => e.IpAddress)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ipAddress");

                entity.Property(e => e.IsInvalidated).HasColumnName("isInvalidated");

                entity.Property(e => e.RefreshToken)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("refreshToken");

                entity.Property(e => e.TeacherId).HasColumnName("teacherID");

                entity.Property(e => e.Token)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("token");

                entity.HasOne(d => d.Teacher)
                    .WithMany(p => p.UserRefreshTokens)
                    .HasForeignKey(d => d.TeacherId)
                    .HasConstraintName("fk_userRefreshToken_teacher");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

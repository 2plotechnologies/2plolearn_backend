using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LMS.Backend.Models;
namespace LMS.Backend.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }
        public DbSet<User> Users {get; set;}
        public DbSet<Course> Courses { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Score> Scores { get; set; }
        public DbSet<Progress> Progress { get; set; }
        public DbSet<Average> Averages { get; set; }
        public DbSet<UserCourses> UserCourses { get; set; }
        public DbSet<Company> Companies {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity => {entity.HasIndex(c => c.email).IsUnique();});

            // Configuración de relaciones
            modelBuilder.Entity<UserCourses>()
                .HasKey(uc => new { uc.UserId, uc.CourseId });

            modelBuilder.Entity<UserCourses>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserCourses)
                .HasForeignKey(uc => uc.UserId);

            modelBuilder.Entity<UserCourses>()
                .HasOne(uc => uc.Course)
                .WithMany(c => c.UserCourses)
                .HasForeignKey(uc => uc.CourseId);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Units)
                .WithOne(u => u.Course)
                .HasForeignKey(u => u.CourseId);

            modelBuilder.Entity<Unit>()
                .HasMany(u => u.Lessons)
                .WithOne(l => l.Unit)
                .HasForeignKey(l => l.UnitId);

            modelBuilder.Entity<Unit>()
                .HasMany(u => u.Evaluations)
                .WithOne(e => e.Unit)
                .HasForeignKey(e => e.UnitId);

            modelBuilder.Entity<Evaluation>()
                .HasMany(e => e.Questions)
                .WithOne(q => q.Evaluation)
                .HasForeignKey(q => q.EvaluationId);

            modelBuilder.Entity<Question>()
                .HasMany(q => q.Answers)
                .WithOne(a => a.Question)
                .HasForeignKey(a => a.QuestionId);

            modelBuilder.Entity<Score>()
                .HasOne(s => s.User)
                .WithMany(u => u.Scores)
                .HasForeignKey(s => s.UserId);

            modelBuilder.Entity<Score>()
                .HasOne(s => s.Course)
                .WithMany(c => c.Scores)
                .HasForeignKey(s => s.CourseId);

            modelBuilder.Entity<Score>()
                .HasOne(s => s.Evaluation)
                .WithMany(e => e.Scores)
                .HasForeignKey(s => s.EvaluationId);

            modelBuilder.Entity<Progress>()
                .HasOne(p => p.User)
                .WithMany(u => u.Progresses)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<Progress>()
                .HasOne(p => p.Course)
                .WithMany(c => c.Progresses)
                .HasForeignKey(p => p.CourseId);

            modelBuilder.Entity<Progress>()
                .HasOne(p => p.Unit)
                .WithMany(u => u.Progresses)
                .HasForeignKey(p => p.UnitId);

            modelBuilder.Entity<Average>()
                .HasOne(a => a.User)
                .WithMany(u => u.Averages)
                .HasForeignKey(a => a.UserId);

            modelBuilder.Entity<Average>()
                .HasOne(a => a.Course)
                .WithMany(c => c.Averages)
                .HasForeignKey(a => a.CourseId);

            modelBuilder.Entity<Company>()
                .HasMany(co => co.Courses)
                .WithOne(c => c.company)
                .HasForeignKey(c => c.CompanyId);

            modelBuilder.Entity<Company>()
                .HasMany(co => co.Users)
                .WithOne(u => u.company)
                .HasForeignKey(u => u.CompanyId);
        }
    }
}
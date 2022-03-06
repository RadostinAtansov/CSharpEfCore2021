
using CodeFirstExercise.Data.DataSettings;
using CodeFirstExercise.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeFirstExercise
{
    public class StudentSytemContext : DbContext
    {
        public StudentSytemContext()
        {

        }

        public StudentSytemContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }

        public DbSet<HomeWork> HomeWorks { get; set; }

        public DbSet<Resource> Resources { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<StudentCourse> StudentCourses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(DataConnection.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentCourse>(entity => 
            {
                modelBuilder
                .Entity<StudentCourse>()
                .HasKey(sc => new { sc.CourseId, sc.StudentId });

                modelBuilder
                    .Entity<StudentCourse>()
                    .HasOne(s => s.Student)
                    .WithMany(c => c.CoursesEnrollments)
                    .HasForeignKey(fk => fk.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder
                    .Entity<StudentCourse>()
                    .HasOne(c => c.Course)
                    .WithMany(s => s.StudentsEnrolled)
                    .HasForeignKey(c => c.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder
            .Entity<HomeWork>()
            .HasOne(s => s.Student)
            .WithMany(h => h.HomeWorkSubmissions)
            .HasForeignKey(s => s.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Resource>()
                .HasOne(c => c.Course)
                .WithMany(r => r.Resources)
                .HasForeignKey(k => k.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<HomeWork>()
                .HasOne(c => c.Course)
                .WithMany(c => c.HomeWorkSubmissions)
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
                
        }
    }
}

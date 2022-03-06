using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFirstExercise.Data.Models
{
    public class Student
    {
        public int StudentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Range(0, 10)]
        public int PhoneNumber { get; set; }

        public bool RegisteredOn { get; set; }

        public DateTime BirthDay { get; set; }

        public ICollection<StudentCourse> CoursesEnrollments { get; set; } = new HashSet<StudentCourse>();
        public ICollection<HomeWork> HomeWorkSubmissions { get; set; } = new HashSet<HomeWork>();

    }
}

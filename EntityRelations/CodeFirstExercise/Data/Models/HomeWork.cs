using System;

namespace CodeFirstExercise.Data.Models
{
    public class HomeWork
    {
        public int HomeWorkId { get; set; }

        public string Content { get; set; }

        public ContentType ContentType { get; set; }

        public DateTime SubmissionTime { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

    }
}

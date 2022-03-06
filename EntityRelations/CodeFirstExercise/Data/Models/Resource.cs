using System.ComponentModel.DataAnnotations;

namespace CodeFirstExercise.Data.Models
{
    public class Resource
    {
        public int ResourceId { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        public string Url { get; set; }

        public ResourceType ResourceType { get; set; }

        public int CourseId { get; set; }

        public Course Course { get; set; }
    }
}

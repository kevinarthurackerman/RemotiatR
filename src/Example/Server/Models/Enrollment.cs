using ContosoUniversity.Shared.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Server.Models
{
    public class Enrollment : IEntity
    {
        [Column("EnrollmentID")]
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public Grade? Grade { get; set; }

        public Course Course { get; set; }
        public Student Student { get; set; }
    }
}
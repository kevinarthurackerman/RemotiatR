using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DelegateDecompiler;

namespace ContosoUniversity.Server.Models
{
    public class Student : IEntity
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        [Column("FirstName")]
        public string FirstMidName { get; set; }
        [DataType(DataType.Date)]
        public DateTime EnrollmentDate { get; set; }

        [Computed]
        public string FullName => LastName + ", " + FirstMidName;

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
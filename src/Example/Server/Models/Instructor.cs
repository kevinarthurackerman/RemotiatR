using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ContosoUniversity.Shared.Infrastructure;

namespace ContosoUniversity.Server.Models
{
    public class Instructor : IEntity, IPerson
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        [Column("FirstName")]
        public string FirstMidName { get; set; }
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        public ICollection<CourseAssignment> CourseAssignments { get; private set; } = new List<CourseAssignment>();
        public OfficeAssignment OfficeAssignment { get; set; }
    }
}
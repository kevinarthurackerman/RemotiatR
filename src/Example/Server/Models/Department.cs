using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Server.Models
{
    public class Department : IEntity
    {
        [Column("DepartmentID")]
        public int Id { get; set; }
        public string Name { get; set; }
        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Budget { get; set; }
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        public int? InstructorId { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public Instructor Instructor { get; set; }
        public ICollection<Course> Courses { get; set; }
    }
}
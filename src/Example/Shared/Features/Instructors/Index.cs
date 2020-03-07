using ContosoUniversity.Shared.Shared;
using MediatR;
using System;
using System.Collections.Generic;

namespace ContosoUniversity.Shared.Features.Instructors
{
    public class Index
    {
        public class Query : IRequest<Model>
        {
            public int? Id { get; set; }
            public int? CourseId { get; set; }
        }

        public class Model
        {
            public int? InstructorId { get; set; }
            public int? CourseId { get; set; }

            public IList<Instructor> Instructors { get; set; }
            public IList<Course> Courses { get; set; }
            public IList<Enrollment> Enrollments { get; set; }

            public class Instructor
            {
                public int Id { get; set; }
                public string LastName { get; set; }
                public string FirstMidName { get; set; }
                public DateTime HireDate { get; set; }
                public string OfficeAssignmentLocation { get; set; }
                public IEnumerable<CourseAssignment> CourseAssignments { get; set; }
            }

            public class CourseAssignment
            {
                public int CourseId { get; set; }
                public string CourseTitle { get; set; }
            }

            public class Course
            {
                public int Id { get; set; }
                public string Title { get; set; }
                public string DepartmentName { get; set; }
            }

            public class Enrollment
            {
                public Grade? Grade { get; set; }
                public string StudentFullName { get; set; }
            }
        }
    }
}

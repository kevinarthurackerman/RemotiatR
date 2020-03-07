using MediatR;
using System.Collections.Generic;
using System.ComponentModel;

namespace ContosoUniversity.Shared.Features.Courses
{
    public class Index
    {
        public class Query : IRequest<Result>
        {
        }

        public class Result
        {
            public List<Course> Courses { get; set; }

            public class Course
            {
                public int Id { get; set; }
                public string Title { get; set; }
                public int Credits { get; set; }
                [DisplayName("Department")]
                public string DepartmentName { get; set; }
            }
        }
    }
}

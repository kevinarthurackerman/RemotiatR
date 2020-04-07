using MediatR;
using System.Collections.Generic;

namespace ContosoUniversity.Shared.Features.Dashboard
{
    public class Courses
    {
        public class Query : IRequest<Result>
        {
        }

        public class Result
        {
            public IEnumerable<Course> Courses { get; set; }

            public class Course
            {
                public string Title { get; set; }
            }
        }
    }
}

using MediatR;
using System.Collections.Generic;
using System.ComponentModel;

namespace ContosoUniversity.Shared.Features.Dashboard
{
    public class Instructors
    {
        public class Query : IRequest<Result>
        {
        }

        public class Result
        {
            public IEnumerable<Instructor> Instructors { get; set; }

            public class Instructor
            {
                public string LastName { get; set; }
                [DisplayName("First Name")]
                public string FirstMidName { get; set; }
            }
        }
    }
}

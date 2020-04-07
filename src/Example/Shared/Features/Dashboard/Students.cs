using MediatR;
using System.Collections.Generic;
using System.ComponentModel;

namespace ContosoUniversity.Shared.Features.Dashboard
{
    public class Students
    {
        public class Query : IRequest<Result>
        {
        }

        public class Result
        {
            public IEnumerable<Student> Students { get; set; }

            public class Student
            {
                public string LastName { get; set; }
                [DisplayName("First Name")]
                public string FirstMidName { get; set; }
            }
        }
    }
}

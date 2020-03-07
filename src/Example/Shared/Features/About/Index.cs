using MediatR;
using System;
using System.Collections.Generic;

namespace ContosoUniversity.Shared.Features.About
{
    public class Index
    {
        public class Query : IRequest<Result>
        {
        }

        public class Result
        {
            public IEnumerable<EnrollmentDateGroup> EnrollmentDateGroups { get; set; }

            public class EnrollmentDateGroup
            {
                public DateTime? EnrollmentDate { get; set; }

                public int StudentCount { get; set; }
            }
        }
    }
}

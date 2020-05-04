using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ContosoUniversity.Shared.Features.About
{
    public class Index
    {
        public class Query : IRequest<Result>
        {
        }

        public class Result
        {
            public EnrollmentDateGroup[] EnrollmentDateGroups { get; set; }

            public class EnrollmentDateGroup
            {
                [DisplayName("Enrollment Date")]
                public DateTime? EnrollmentDate { get; set; }

                [DisplayName("Students")]
                public int StudentCount { get; set; }
            }
        }
    }
}

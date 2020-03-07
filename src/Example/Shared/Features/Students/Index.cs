using ContosoUniversity.Shared.Infrastructure;
using MediatR;
using System;
using System.ComponentModel;

namespace ContosoUniversity.Shared.Features.Students
{
    public class Index
    {
        public class Query : IRequest<Result>
        {
            public string SortOrder { get; set; }
            public string SearchString { get; set; }
            public int? Page { get; set; }
        }

        public class Result
        {
            public PaginatedList<Model> Results { get; set; }
        }

        public class Model
        {
            public int Id { get; set; }
            [DisplayName("First Name")]
            public string FirstMidName { get; set; }
            public string LastName { get; set; }
            public DateTime EnrollmentDate { get; set; }
            public int EnrollmentsCount { get; set; }
        }
    }
}

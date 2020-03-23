using MediatR;
using System;
using System.Collections.Generic;

namespace ContosoUniversity.Shared.Features.Dashboard
{
    public class Departments
    {
        public class Query : IRequest<Result>
        {
        }

        public class Result
        {
            public IEnumerable<Department> Departments { get; set; }

            public class Department
            {
                public string Name { get; set; }
            }
        }
    }
}

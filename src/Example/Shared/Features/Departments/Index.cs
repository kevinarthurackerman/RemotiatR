using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ContosoUniversity.Shared.Features.Departments
{
    public class Index
    {
        public class Query : IRequest<Result>
        {
        }

        public class Result
        {
            public Model[] Departments { get; set; }

            public class Model
            {
                public string Name { get; set; }
                public decimal Budget { get; set; }
                public DateTime StartDate { get; set; }
                public int Id { get; set; }
                [DisplayName("Administrator")]
                public string AdministratorFullName { get; set; }
            }
        }
    }
}

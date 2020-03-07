using MediatR;
using System;
using System.Collections.Generic;

namespace ContosoUniversity.Shared.Features.Departments
{
    public class Index
    {
        public class Query : IRequest<List<Model>>
        {
        }

        public class Model
        {
            public string Name { get; set; }
            public decimal Budget { get; set; }
            public DateTime StartDate { get; set; }
            public int Id { get; set; }
            public string AdministratorFullName { get; set; }
        }
    }
}

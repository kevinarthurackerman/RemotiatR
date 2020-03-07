using MediatR;
using System;

namespace ContosoUniversity.Shared.Features.Departments
{
    public class Details
    {
        public class Query : IRequest<Model>
        {
            public int Id { get; set; }
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

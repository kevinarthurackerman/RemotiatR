using System;

namespace RemotiatR.Example.Api.Services.Data
{
    public class TodosEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastEdited { get; set; }
    }
}

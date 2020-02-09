using System;

namespace RemotiatR.Example.Web.Features.Todo
{
    public partial class TodosPage
    {
        public class TodosViewModel
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public DateTime Created { get; set; }
            public DateTime LastEdited { get; set; }
            public bool IsEditing { get; set; }

            public PreviousStateViewModel PreviousState { get; set; }

            public class PreviousStateViewModel
            {
                public string Title { get; set; }
            }
        }
    }
}

using AutoMapper;

namespace RemotiatR.Example.Web.Features.Todo
{
    public partial class TodosPage
    {
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<TodosViewModel, TodosViewModel.PreviousStateViewModel>().ReverseMap();
            }
        }
    }
}

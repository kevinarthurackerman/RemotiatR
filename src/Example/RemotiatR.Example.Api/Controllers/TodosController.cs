using MediatR;
using Microsoft.AspNetCore.Mvc;
using RemotiatR.Example.Api.Features.Todo;
using System.Threading.Tasks;

namespace RemotiatR.Example.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly IMediator _mediatr;

        public TodosController(IMediator mediatr) => _mediatr = mediatr;

        [HttpPost]
        [Route("[action]")]
        public Task<Get_Todos.Response> Get() => _mediatr.Send(new Get_Todos.Request());

        [HttpPost]
        [Route("[action]")]
        public Task<Create_Todos.Response> Create(Create_Todos.Request request) => _mediatr.Send(request);

        [HttpPost]
        [Route("[action]")]
        public Task<Edit_Todos.Response> Edit(Edit_Todos.Request request) => _mediatr.Send(request);

        [HttpPost]
        [Route("[action]")]
        public Task<Delete_Todos.Response> Delete(Delete_Todos.Request request) => _mediatr.Send(request);
    }
}

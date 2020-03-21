
# RemotiatR

### MediatR For Your Remote Clients
RemotiatR makes it easy to run you [MediatR](https://github.com/jbogard/MediatR) requests and commands from a remote client on your server without all the ceremony. Just send or publish the same way you always would, and RemotiatR handles everything between.

> RemotiatR is not made in direct affiliation or endorsement of the authors or maintainers of any other libraries

### Install Packages
To add RemotiatR to your project add the client and server packages to your respective client and server projects

- <https://www.nuget.org/packages/RemotiatR.Client/>
- <https://www.nuget.org/packages/RemotiatR.Server/>

```
dotnet add package RemotiatR.Client
dotnet add package RemotiatR.Server
```

#### Extensions

Adding support for [HTTP](https://github.com/kevinarthurackerman/RemotiatR/tree/master/src/MessageTransport/RemotiatR.MessageTransport.Http)

Adding support for [JSON](https://github.com/kevinarthurackerman/RemotiatR/tree/master/src/Serializer/RemotiatR.Serializer.Json)

Adding support for [FluentValidation](https://github.com/kevinarthurackerman/RemotiatR/tree/master/src/FluentValidation)

### Getting Started
Check out the example at [src/Example/](https://github.com/kevinarthurackerman/RemotiatR/tree/master/src/Example)

In your shared project
```csharp
using MediatR;

public class Ping
{
    public class Request : IRequest<Response>
    {
    }

    public class Response
    {
        public DateTime ServerTime { get; set; }
    }
}
```

On your server
```csharp
using RemotiatR.Server;
using RemotiatR.MessageTransport.Http.Server;
using RemotiatR.Serializer.Json.Server;

public class Startup
{
    ...
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddRemotiatr(x => 
        {
            // register host uri and assemblies to search for notifications and requests
            x.AddHost(new Uri("https://localhost:44337"), typeof(Ping).Assembly, typeof(Startup).Assembly);
			
            // register http message transport
            x.AddHttpMessageTransport();
            
            // register json serializer
            x.AddJsonSerializer();
        }
    }
    ...
    public void Configure(IApplicationBuilder app)
    {
        ...
        // uses the registered transport
        app.UseHttpRemotiatr();
    }
}

public class PingHandler : IRequestHandler<Ping.Request, Ping.Response>
{
    public Task<Response> Handle(Request request, CancellationToken cancellationToken) =>
        Task.FromResult(new Response { ServerTime = DateTime.UtcNow });
}
```

On your client
```csharp
using RemotiatR.Client;
using RemotiatR.MessageTransport.Http.Client;
using RemotiatR.Serializer.Json.Client;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddRemotiatr(x =>
        {
            // register host uri and assemblies to search for notifications and requests
            x.AddHost(new Uri("https://localhost:44337"), typeof(Ping).Assembly, typeof(Startup).Assembly);
			
            // register http message transport
            x.AddHttpMessageTransport();
            
            // register json serializer
            x.AddJsonSerializer();
        });
    }
    ...
}

public class ServerTimeService
{
    private readonly IRemotiatr _remotiatr;

    public ServerTimeService(IRemotiatr remotiatr) =>
        _remotiatr = remotiatr;

    public async Task<DateTime> GetServerTime() =>
        (await _remotiatr.Send(new Ping.Request())).ServerTime;
}
```

### Contributing
Want to contribute? Great!

### License
[MIT](https://github.com/kevinarthurackerman/RemotiatR/blob/master/LICENSE)

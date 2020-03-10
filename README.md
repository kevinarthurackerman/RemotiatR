# RemotiatR

### MediatR For Your Remote Clients
RemotiatR makes it easy to run you MediatR requests and commands from a remote client on your server without all the ceremony. Just send or publish the same way you always would, and RemotiatR handles everything between.

### Install Packages
Base nuget packages
- <https://www.nuget.org/packages/RemotiatR.Client/>
- <https://www.nuget.org/packages/RemotiatR.Server/>

Add support for FluentValidation (see <https://github.com/FluentValidation/FluentValidation>)
- <https://www.nuget.org/packages/RemotiatR.Client.FluentValidation/>
- <https://www.nuget.org/packages/RemotiatR.Server.FluentValidation/>

### Getting Started
In your shared project
```csharp
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
public class Startup
{
    ...
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddRemotiatr(x => x.AddAssemblies(typeof(PingHandler),typeof(Startup)));
    }
    ...
    public void Configure(IApplicationBuilder app)
    {
        ...
        app.UseRemotiatr(x =>
        {
            x.AddAssemblies();
        });
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
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddRemotiatr(x =>
        {
            x.AddAssemblies(typeof(Ping));
            x.SetEndpointUri(new Uri("https://localhost:{{port number}}/remotiatr"));
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
MIT

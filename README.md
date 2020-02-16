# RemotiatR

### MediatR For Your Remote Clients
RemotiatR makes it easy to run you MediatR requests and commands from a remote client on your server without all the ceremony. Just send or publish the same way you always would, and RemotiatR handles everything between.

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
        services.AddMediatR(typeof(PingHandler));
        services.AddRemotiatr();
    }
    ...
    public void Configure(IApplicationBuilder app)
    {
        ...
        app.UseRemotiatr(x =>
        {
            x.AddAssemblies(typeof(SharedMarker));
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
            x.SetBaseUri(new Uri("https://localhost:{{port number}}"));
        });
    }
    ...
}
```

### Contributing
Want to contribute? Great!

### License
MIT

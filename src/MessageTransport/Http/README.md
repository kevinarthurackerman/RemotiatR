# RemotiatR.MessageTransport.Http

### Add support for sending notifications and request via HTTP

> RemotiatR is not made in direct affiliation or endorsement of the authors or maintainers of any other libraries

### Install Packages
To add support for HTTP add the client and server packages to your respective client and server projects
- <https://www.nuget.org/packages/RemotiatR.MessageTransport.Http.Client/>
- <https://www.nuget.org/packages/RemotiatR.MessageTransport.Http.Server/>

```
dotnet add package RemotiatR.MessageTransport.Http.Client
dotnet add package RemotiatR.MessageTransport.Http.Server
```

### Getting Started
Check out the example at [src/Example/](https://github.com/kevinarthurackerman/RemotiatR/tree/master/src/Example)

Configure RemotiatR HTTP services

On your server
```csharp
using RemotiatR.MessageTransport.Http.Server;

public class Startup
{
    ...
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddRemotiatr(x => 
        {
            ...
            // register http message transport
            x.AddHttpMessageTransport();
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
```

On your client
```csharp
using RemotiatR.MessageTransport.Http.Client;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddRemotiatr(x =>
        {
            ...
            // register http message transport
            x.AddHttpMessageTransport();
        });
    }
    ...
}
```

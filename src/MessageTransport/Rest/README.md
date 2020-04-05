# RemotiatR.MessageTransport.Rest

### Add support for sending notifications and request via HTTP using a RESTful API

> RemotiatR is not made in direct affiliation or endorsement of the authors or maintainers of any other libraries

### Install Packages
To add support for REST add the client and server packages to your respective client and server projects
- <https://www.nuget.org/packages/RemotiatR.MessageTransport.Rest.Client/>
- <https://www.nuget.org/packages/RemotiatR.MessageTransport.Rest.Server/>

```
dotnet add package RemotiatR.MessageTransport.Rest.Client
dotnet add package RemotiatR.MessageTransport.Rest.Server
```

### Getting Started
Check out the example at [src/Example/](https://github.com/kevinarthurackerman/RemotiatR/tree/master/src/Example)

Configure RemotiatR REST services

On your server
```csharp
using RemotiatR.MessageTransport.Rest.Server;

public class Startup
{
    ...
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddRemotiatr(x => 
        {
            ...
            // register rest message transport
            x.AddRestMessageTransport();
        }
    }
    ...
    public void Configure(IApplicationBuilder app)
    {
        ...
        // uses the registered transport
        app.UseRestRemotiatr();
    }
}
```

On your client
```csharp
using RemotiatR.MessageTransport.Rest.Client;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddRemotiatr(x =>
        {
            ...
            // register rest message transport
            x.AddRestMessageTransport();
        });
    }
    ...
}
```

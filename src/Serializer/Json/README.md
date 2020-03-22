# RemotiatR.Serializer.Json.Http

### Add support for sending notifications and request via JSON

> RemotiatR is not made in direct affiliation or endorsement of the authors or maintainers of any other libraries

### Install Packages
To add support for JSON add the client and server packages to your respective client and server projects
- <https://www.nuget.org/packages/RemotiatR.Serializer.Json.Client/>
- <https://www.nuget.org/packages/RemotiatR.Serializer.Json.Server/>

```
dotnet add package RemotiatR.Serializer.Json.Client
dotnet add package RemotiatR.Serializer.Json.Server
```

### Getting Started
Check out the example at [src/Example/](https://github.com/kevinarthurackerman/RemotiatR/tree/master/src/Example)

Configure RemotiatR JSON services

On your server
```csharp
using RemotiatR.Serializer.Json.Server;

public class Startup
{
    ...
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddRemotiatr(x => 
        {
            ...
            // register json serializer
            x.AddJsonSerializer();
        }
    }
}
```

On your client
```csharp
using RemotiatR.Serializer.Json.Client;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddRemotiatr(x =>
        {
            ...
            // register json serializer
            x.AddJsonSerializer();
        });
    }
    ...
}
```

> Remove this header and update this template when creating a new extension project

___

# RemotiatR.{{ProjectCategory}}.{{ProjectName}}

### Add support for {{ProjectName}} {{ProjectCategory}}

> RemotiatR is not made in direct affiliation or endorsement of the authors or maintainers of any other libraries

### Install Packages
To add support for JSON add the client and server packages to your respective client and server projects
- <https://www.nuget.org/packages/RemotiatR.{{ProjectCategory}}.{{ProjectName}}.Client/>
- <https://www.nuget.org/packages/RemotiatR.{{ProjectCategory}}.{{ProjectName}}.Server/>

```
dotnet add package RemotiatR.{{ProjectCategory}}.{{ProjectName}}.Client
dotnet add package RemotiatR.{{ProjectCategory}}.{{ProjectName}}.Server
```

### Getting Started
Check out the example at [src/Example/](https://github.com/kevinarthurackerman/RemotiatR/tree/master/src/Example)

Configure {{ProjectName}}{{ProjectCategory}}

On your server
```csharp
using RemotiatR.{{ProjectCategory}}.{{ProjectName}}.Server;

public class Startup
{
    ...
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddRemotiatr(x => 
        {
            ...
            // register {{ProjectName}}{{ProjectCategory}}
            x.Add{{ProjectName}}{{ProjectCategory}}();
        }
    }
}
```

On your client
```csharp
using RemotiatR.{{ProjectCategory}}.{{ProjectName}}.Client;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddRemotiatr(x =>
        {
            ...
            // register {{ProjectName}}{{ProjectCategory}}
            x.Add{{ProjectName}}{{ProjectCategory}}();
        });
    }
    ...
}
```

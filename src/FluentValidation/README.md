# RemotiatR.FluentValidation

### Add support for FluentValidation
Configure rules with an easy fluent syntax with [FluentValidation](https://github.com/FluentValidation/FluentValidation/) and apply those rules on the client, server, or both.

### Install Packages
Add support for FluentValidation
- <https://www.nuget.org/packages/RemotiatR.FluentValidation.Client/>
- <https://www.nuget.org/packages/RemotiatR.FluentValidation.Server/>

### Getting Started
Check out the example at [src/Example/](https://github.com/kevinarthurackerman/RemotiatR/tree/master/src/Example)

Configure core RemotiatR services

In your shared project
```csharp
public class UpdatePerson
{
    public class Request : IRequest<Response>
    {
        public Guid Id { get; set; }
        public String FirstName { get; set; }
        public String EmailAddress { get; set; }
    }

    public class Response
    {
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
        services.AddRemotiatr(x => 
        {
            ...
            x.Services.AddFluentValidation(typeof(UpdatePersonHandler),typeof(Startup));
        }
    }
}

public class UpdatePersonHandler : IRequestHandlerUpdatePerson.Request, UpdatePerson.Response>
{
    public Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        ...
    }
}
```

On your client
```csharp
@inject IRemotiatr _remotiatr
...
@code {
    ...
    private async Task OnPostAsync(EditContext editContext)
    {
        await _remotiatr.WithValidationContext(editContext).Send(Data);
        if(editContext.IsValid()) _navigationManager.NavigateTo("/People");
    }
}
```

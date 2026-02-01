---
layout: default
title: Commands & Queries
---

# Requests (Commands & Queries)

In the Mediator pattern, a **Request** represents a single intent—something you want the application to *do* or *retrieve*. It acts as a contract between the caller and the handler.

## The Request Contract

Concordia distinguishes between two types of requests based on the need for a return value.

### 1. Queries (Values Returning)
Use `IRequest<TResponse>` when you expect a result. This aligns typically with the **Query** side of CQRS.

```csharp
public class GetUserByIdQuery : IRequest<UserDto>
{
    public Guid UserId { get; }
    public GetUserByIdQuery(Guid userId) => UserId = userId;
}
```

### 2. Commands (Void / Fire-and-Forget)
Use `IRequest` when the operation is an action where the result is simply "completion" or side-effects. This aligns with the **Command** side of CQRS.

```csharp
public class DeleteUserCommand : IRequest
{
    public Guid UserId { get; }
    public DeleteUserCommand(Guid userId) => UserId = userId;
}
```

---

## Implementing Handlers

A handler is a dedicated class responsible for processing a specific request. This enforces the **Single Responsibility Principle**.

### Basic Handler Structure

```csharp
public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserRepository _repository;

    public GetUserByIdHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        // 1. Validate (optional, better in pipelines)
        // 2. Execute Logic
        var entity = await _repository.GetAsync(request.UserId, cancellationToken);
        
        // 3. Map & Return
        return entity.ToDto();
    }
}
```

### Best Practices

> [!IMPORTANT]
> **CancellationToken Propagation**
> Always respect the `CancellationToken` passed to the `Handle` method. Pass it down to your database calls or HTTP requests. This allows the server to gracefully stop work if the client disconnects.

> [!TIP]
> **Keep Handlers Thin**
> Handlers should primarily orchestrate logic, not contain complex business rules themselves. Delegate domain logic to your Domain Entities or Domain Services.

---

## Dispatching Requests

Use the `ISender` interface (or `IMediator` which aggregates it) to dispatch requests.

```csharp
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender) => _sender = sender;

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _sender.Send(new GetUserByIdQuery(id));
        return Ok(result);
    }
}
```

## Error Handling

Since handlers are just C# classes, exceptions bubble up naturally. You can catch them globally in your host (e.g., ASP.NET Core Middleware) or intercept them using [Pipeline Behaviors](pipelines.html).

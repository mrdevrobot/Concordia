---
layout: default
title: Pipelines
---

# Pipeline Behaviors

Pipelines are the implementation of the **Decorator Pattern** or **Chain of Responsibility**. They allow you to wrap the execution of a handler with custom logic, executed *before* and *after* the handler itself.

Often referred to as "Middleware for your Mediator", they are perfect for cross-cutting concerns.

## How it Works

When you call `Sender.Send(request)`, Concordia builds a pipeline:

1. **Behavior 1 (Pre)**
2. **Behavior 2 (Pre)**
3. ...
4. **Actual Handler**
5. ...
6. **Behavior 2 (Post)**
7. **Behavior 1 (Post)**

## Implementing a Behavior

Implement `IPipelineBehavior<TRequest, TResponse>`.

### Example 1: Request Validation

One of the most common use cases is validating the request before it reaches the handler.

```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var failures = _validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }

        // Proceed to next behavior or handler
        return await next();
    }
}
```

### Example 2: Performance Logging

Measure how long each request takes.

```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("Processing Request: {Name}", requestName);
        
        var timer = Stopwatch.StartNew();
        var response = await next(); // Allow request to process
        timer.Stop();

        if (timer.ElapsedMilliseconds > 500)
        {
             _logger.LogWarning("Long Running Request: {Name} ({Elapsed}ms)", requestName, timer.ElapsedMilliseconds);
        }

        return response;
    }
}
```

## Registration Order

The order in which behaviors are registered matters. In the Source Generator approach, behaviors can be detected automatically, but ensuring strict ordering might require manual registration or interface constraints if exact onion-layering is critical.

In reflection-based setup (`AddMediator`), order is strictly determined by registration order.

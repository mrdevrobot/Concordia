---
layout: default
title: Notifications
---

# Notifications (Events)

Notifications represent the **Publish/Subscribe** pattern. Unlike Requests, which are 1-to-1, Notifications are **1-to-many**. They allow multiple parts of your system to react to a single event without being coupled to the source.

## Defining a Notification

A notification is a simple DTO (Data Transfer Object) implementing `INotification`. It usually represents something that *has happened*.

```csharp
public class UserRegisteredNotification : INotification
{
    public Guid UserId { get; }
    public DateTime RegisteredAt { get; }

    public UserRegisteredNotification(Guid userId)
    {
        UserId = userId;
        RegisteredAt = DateTime.UtcNow;
    }
}
```

## Creating Handlers (Subscribers)

Create as many handlers as you need. Each handler will be executed when the notification is published.

```csharp
// Handler 1: Send a Welcome Email
public class SendWelcomeEmailHandler : INotificationHandler<UserRegisteredNotification>
{
    public async Task Handle(UserRegisteredNotification notification, CancellationToken cancellationToken)
    {
        await _emailService.SendWelcomeAsync(notification.UserId);
    }
}

// Handler 2: Audit Logging
public class AuditLogHandler : INotificationHandler<UserRegisteredNotification>
{
    public Task Handle(UserRegisteredNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("New user registered: {UserId}", notification.UserId);
        return Task.CompletedTask;
    }
}
```

---

## Publishing Strategies

Concordia gives you control over *how* these handlers are executed via the `INotificationPublisher` interface.

### Default Behavior (Foreach Await)
By default, Concordia executes handlers sequentially, awaiting each one. If one fails, the exception propagates immediately, potentially stopping subsequent handlers.

### Custom Publishers: Parallel Execution
You can override the default strategy. For example, to run all handlers in parallel ("Fire-and-Forget"):

```csharp
public class ParallelNoWaitPublisher : INotificationPublisher
{
    public Task Publish(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification, CancellationToken cancellationToken)
    {
        var tasks = handlerExecutors
            .Select(handler => handler.HandlerCallback(notification, cancellationToken))
            .ToArray();

        return Task.WhenAll(tasks);
    }
}
```

To register a custom publisher, configure it during startup:

```csharp
// In Program.cs
builder.Services.AddConcordiaCoreServices<ParallelNoWaitPublisher>();
```

---
layout: default
title: Source Generators
---

# Source Generators: The Engine of Concordia

Concordia distinguishes itself from many other .NET Mediator implementations by relying heavily on **C# Source Generators**. This technology allows us to inspect your code *during compilation* and generate the necessary wiring code automatically.

## How it Works

1. **Analysis**: The `Concordia.Generator` analyzer runs continuously in the background (within Visual Studio/Rider) or during the build process (`dotnet build`).
2. **Discovery**: It scans your project for classes implementing `IRequestHandler<>`, `INotificationHandler<>`, `IPipelineBehavior<>`, etc.
3. **Synthesis**: It generates a C# file containing a specific extension method that registers all found types with the Dependency Injection container.

This generated code is then compiled along with your project.

## Performance vs. Reflection

Legacy libraries (like older versions of MediatR) typically use `Assembly.GetExecutingAssembly().GetTypes()` at startup to find handlers.

| Metric | Reflection (Legacy) | Source Generators (Concordia) |
| :--- | :--- | :--- |
| **Startup Cost** | O(N) where N is assembly size | **Zero** (O(1)) |
| **Memory Usage** | High (loading all types metadata) | Low (only necessary types) |
| **Safety** | Runtime Errors (Missing Dependencies) | **Compile-Time** Safety |
| **Trimming** | Hard / Requires directives | **Native Support** |

## Advanced Configuration

You can customize the behavior of the generator using MSBuild properties in your `.csproj` file.

### Custom Method Name

By default, the generator creates a method named `AddConcordiaHandlers`. You can change this to avoid collisions or match your naming conventions.

```xml
<PropertyGroup>
    <ConcordiaGeneratedMethodName>AddMyModuleHandlers</ConcordiaGeneratedMethodName>
</PropertyGroup>
```

### Inspecting Generated Code

To see exactly what Concordia is generating for you:
1. Open **Dependencies** in Solution Explorer.
2. Go to **Analyzers** -> **Concordia.Generator**.
3. Expand **Concordia.Generator.SourceGenerator**.
4. Double-click the generated file (e.g., `ConcordiaGeneratedRegistrations.g.cs`).

You will see standard, readable C# code:

```csharp
// Example of generated code (simplified)
public static IServiceCollection AddMyModuleHandlers(this IServiceCollection services)
{
    services.TryAddTransient<IRequestHandler<GetFooQuery, Foo>, GetFooHandler>();
    // ...
    return services;
}
```

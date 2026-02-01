---
layout: default
title: Installation
---

# Installation & Setup

Concordia is modular by design. Depending on your project's needs—specifically whether you prefer performance (Source Generators) or legacy compatibility (Reflection)—you will install different combinations of packages.

## Package Ecosystem

| Package | Description |
| :--- | :--- |
| **`Concordia.Core`** | **Required**. Contains the core interfaces (`IMediator`, `IRequest`, `INotification`) and the default `Mediator` implementation. No dependencies on external logic. |
| **`Concordia.Generator`** | **Recommended**. The C# Source Generator that analyzes your code during compilation to generate handler registrations. |
| **`Concordia.MediatR`** | **Legacy / Migration**. A compatibility shim that provides `AddMediator` using runtime reflection, mimicking MediatR's behavior. |

---

## Strategy A: The "Modern" Approach (Source Generators)
*Recommended for all new projects.*

This approach leverages the Roslyn compiler to inject registration code directly into your assembly. It guarantees zero startup overhead.

### 1. Install Packages
Add the Core library and the Generator to your project using the .NET CLI:

```bash
dotnet add package Concordia.Core --version 2.3.0
dotnet add package Concordia.Generator --version 2.3.0
```

### 2. Verify csproj Configuration
Ensure that the `Concordia.Generator` is properly referenced (usually handled automatically by NuGet, but good to verify):

```xml
<ItemGroup>
    <PackageReference Include="Concordia" Version="2.3.0" />
    <PackageReference Include="Concordia.Generator" Version="2.3.0" PrivateAssets="all" />
</ItemGroup>
```

### 3. Register Services
The generator creates an extension method based on your project's content. By default, it follows naming conventions, but you can look for it in your startup code.

```csharp
using Concordia;
using Concordia.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Core Services (IMediator, ISender, IPublisher)
builder.Services.AddConcordiaCoreServices();

// 2. Add Generated Handlers
// The name 'AddMyProjectHandlers' is an example. 
// You can customize this via MSBuild properties if needed.
builder.Services.AddConcordiaHandlers(); 
```

> [!TIP]
> **Customizing the Generated Method Name:**
> You can control the name of the generated extension method by adding a property to your `.csproj` file:
> ```xml
> <PropertyGroup>
>    <ConcordiaGeneratedMethodName>AddMyCustomHandlers</ConcordiaGeneratedMethodName>
> </PropertyGroup>
> ```

---

## Strategy B: The "Compatibility" Approach (Reflection)
*Best for teams migrating large monolithic applications from MediatR.*

If you cannot immediately switch to compile-time generation (e.g., due to complex dynamic loading requirements), you can use the reflection-based approach.

### 1. Install Packages
Add the Core library and the MediatR compatibility layer:

```bash
dotnet add package Concordia.Core --version 2.3.0
dotnet add package Concordia.MediatR --version 2.3.0
```

### 2. Register Services
Use the standard `AddMediator` method. This will scan the provided assemblies for handlers.

```csharp
using Concordia.MediatR;
using System.Reflection;

builder.Services.AddMediator(cfg =>
{
    // Scan the current assembly for Handlers, Behaviors, etc.
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    
    // Optional configuration
    cfg.Lifetime = ServiceLifetime.Scoped;
});
```

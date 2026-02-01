---
layout: default
title: MediatR Compatibility
---

# MediatR Compatibility Guide

We understand that migrating heavily used libraries in large codebases can be daunting. Concordia includes a compatibility layer specifically designed to make the transition from **MediatR** seamless.

## The Compatibility Layer (`Concordia.MediatR`)

This package provides an implementation of the `AddMediator` extension method that mimics the API signature of MediatR v12+. It allows you to keep your existing registration logic (using `Assembly.Scan`) while switching the underlying engine to Concordia.

> [!WARNING]
> **Performance Note**: Using this compatibility layer means you are **opting out** of the Source Generator performance benefits. It uses Runtime Reflection, just like MediatR. However, it is an excellent first step for migration before refactoring to Source Generators.

## Migration Steps

### 1. Update Dependencies
Remove `MediatR` and install `Concordia`.

```diff
- <PackageReference Include="MediatR" Version="12.0.1" />
+ <PackageReference Include="Concordia.Core" Version="2.2.0" />
+ <PackageReference Include="Concordia.MediatR" Version="2.2.0" />
```

### 2. Rename Namespaces
Use "Find and Replace" to update namespaces. The interface names are identical, so mostly you just need to change the `using` statements.

```csharp
// Before
using MediatR;

// After
using Concordia;
```

### 3. Update Startup Registration
If you were using `AddMediatR`, you can switch to `AddMediator`.

```csharp
// Program.cs

// Before
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// After (requires 'using Concordia.MediatR;')
builder.Services.AddMediator(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
```

## Supported Features
The compatibility layer supports the vast majority of MediatR features:
- Request/Response Handlers
- Notification Handlers
- Pipeline Behaviors (`AddOpenBehavior`)
- Stream Behaviors

## What's Next?
Once your application is running on Concordia (via reflection), you can migrate module-by-module to the **Source Generator** approach to unlock instant startup performance.

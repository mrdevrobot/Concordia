using Xunit;

namespace Concordia.Generator.Tests;

public class BasicGenerationTests
{
    [Fact]
    public void ShouldGenerate_AddConcordiaHandlers_When_AttributeIsPresent()
    {
        var source = @"
using Concordia;
using Concordia.Attributes;

[assembly: DiscoverConcordiaHandlers]

namespace MyTestApp.Handlers
{
    public class MyRequest : IRequest<string> { }

    public class MyHandler : IRequestHandler<MyRequest, string>
    {
        public System.Threading.Tasks.Task<string> Handle(MyRequest request, System.Threading.CancellationToken cancellationToken)
        {
            return System.Threading.Tasks.Task.FromResult(""ok"");
        }
    }
}";

        var (diagnostics, generatedSource) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        Assert.NotEmpty(generatedSource);
        Assert.Contains("public static IServiceCollection AddConcordiaHandlers(this IServiceCollection services)", generatedSource);
        Assert.Contains("services.AddTransient<global::Concordia.IRequestHandler<global::MyTestApp.Handlers.MyRequest, string>, global::MyTestApp.Handlers.MyHandler>();", generatedSource);
    }

    [Fact]
    public void ShouldNotGenerate_When_AttributeIsMissing()
    {
        var source = @"
using Concordia;

namespace MyTestApp.Handlers
{
    public class MyRequest : IRequest<string> { }

    public class MyHandler : IRequestHandler<MyRequest, string>
    {
        public System.Threading.Tasks.Task<string> Handle(MyRequest request, System.Threading.CancellationToken cancellationToken)
        {
            return System.Threading.Tasks.Task.FromResult(""ok"");
        }
    }
}";

        var (diagnostics, generatedSource) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        Assert.Empty(generatedSource);
    }
}

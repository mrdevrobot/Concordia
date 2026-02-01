using Xunit;
using System.Collections.Generic;

namespace Concordia.Generator.Tests;

public class ConcordiaGeneratedMethodNameTests
{
    [Fact]
    public void ShouldGenerate_CustomMethodName_When_PropertyIsSet()
    {
        var source = @"
using Concordia;
using Concordia.Attributes;

[assembly: DiscoverConcordiaHandlers]

namespace MyTestApp.Handlers
{
    public class MyRequest : IRequest { }

    public class MyHandler : IRequestHandler<MyRequest>
    {
        public System.Threading.Tasks.Task Handle(MyRequest request, System.Threading.CancellationToken cancellationToken)
        {
            return System.Threading.Tasks.Task.CompletedTask;
        }
    }
}";

        var globalOptions = new Dictionary<string, string>
        {
            { "build_property.concordiageneratedmethodname", "AddMyCustomHandlers" }
        };

        var (diagnostics, generatedSource) = GeneratorTestHelper.RunGenerator(source, globalOptions);

        Assert.Empty(diagnostics);
        Assert.NotEmpty(generatedSource);
        
        // Use Assert.Contains with a substring to avoid whitespace issues, but make it specific enough
        Assert.Contains("public static IServiceCollection AddMyCustomHandlers(this IServiceCollection services)", generatedSource);
        Assert.DoesNotContain("AddConcordiaHandlers", generatedSource);
    }
}

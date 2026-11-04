using System.Reflection;

using NetArchTest.Rules;

using Xunit;

namespace VetSystem.ArchitectureTests;

public class LayerDependencyTests
{
    private static readonly Assembly DomainAssembly = VetSystem.Domain.AssemblyReference.Assembly;
    private static readonly Assembly ApplicationAssembly = VetSystem.Application.AssemblyReference.Assembly;
    private static readonly Assembly InfrastructureAssembly = VetSystem.Infrastructure.AssemblyReference.Assembly;

    private static readonly string[] Modules = ["Registry", "Scheduling"];

    [Fact]
    public void Domain_should_not_depend_on_other_layers_or_EF_Core()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "VetSystem.Application",
                "VetSystem.Infrastructure",
                "VetSystem.Api",
                "Microsoft.EntityFrameworkCore")
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Application_should_not_depend_on_Infrastructure_or_Api()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny("VetSystem.Infrastructure", "VetSystem.Api")
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Infrastructure_should_not_depend_on_Api()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot()
            .HaveDependency​On("VetSystem.Api")
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Modules_should_not_depend_on_each_other()
    {
        var targets = new[]
        {
            (Assembly: DomainAssembly, Root: "VetSystem.Domain"),
            (Assembly: ApplicationAssembly, Root: "VetSystem.Application"),
        };

        foreach (var (assembly, root) in targets)
            foreach (var module in Modules)
                foreach (var other in Modules.Where(m => m != module))
                {
                    var result = Types.InAssembly(assembly)
                        .That().ResideInNamespaceStartingWith($"{root}.{module}")
                        .ShouldNot().HaveDependencyOn($"{root}.{other}")
                        .GetResult();

                    Assert.True(result.IsSuccessful, $"{root}.{module} -> {root}.{other}: {Describe(result)}");
                }
    }

    private static string Describe(TestResult result) =>
        string.Join(", ", result.FailingTypeNames ?? []);
}
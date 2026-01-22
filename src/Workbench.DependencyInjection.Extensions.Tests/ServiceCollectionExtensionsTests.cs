using Microsoft.Extensions.DependencyInjection;
using Workbench.DependencyInjection.Extensions;
using Xunit;

namespace Workbench.DependencyInjection.Extensions.Tests;

public class ServiceCollectionExtensionsTests
{
    public interface ITestService
    {
        string Execute();
    }

    public class TestService : ITestService
    {
        public string Execute() => "Original";
    }

    public class TestDecorator : ITestService
    {
        private readonly ITestService _inner;

        public TestDecorator(ITestService inner)
        {
            _inner = inner;
        }

        public string Execute() => $"Decorated({_inner.Execute()})";
    }

    public class SecondDecorator : ITestService
    {
        private readonly ITestService _inner;

        public SecondDecorator(ITestService inner)
        {
            _inner = inner;
        }

        public string Execute() => $"Second[{_inner.Execute()}]";
    }

    [Fact(DisplayName = "Decorate - Deve decorar serviço registrado como Transient")]
    public void Decorate_DeveDecorarServicoTransient()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<ITestService, TestService>();

        // Act
        services.Decorate<ITestService, TestDecorator>();
        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<ITestService>();

        // Assert
        Assert.IsType<TestDecorator>(service);
        Assert.Equal("Decorated(Original)", service.Execute());
    }

    [Fact(DisplayName = "Decorate - Deve decorar serviço registrado como Scoped")]
    public void Decorate_DeveDecorarServicoScoped()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<ITestService, TestService>();

        // Act
        services.Decorate<ITestService, TestDecorator>();
        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITestService>();

        // Assert
        Assert.IsType<TestDecorator>(service);
        Assert.Equal("Decorated(Original)", service.Execute());
    }

    [Fact(DisplayName = "Decorate - Deve decorar serviço registrado como Singleton")]
    public void Decorate_DeveDecorarServicoSingleton()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ITestService, TestService>();

        // Act
        services.Decorate<ITestService, TestDecorator>();
        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<ITestService>();

        // Assert
        Assert.IsType<TestDecorator>(service);
        Assert.Equal("Decorated(Original)", service.Execute());
    }

    [Fact(DisplayName = "Decorate - Deve empilhar múltiplos decorators")]
    public void Decorate_DeveEmpilharMultiplosDecorators()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<ITestService, TestService>();

        // Act
        services.Decorate<ITestService, TestDecorator>();
        services.Decorate<ITestService, SecondDecorator>();
        
        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<ITestService>();

        // Assert
        Assert.IsType<SecondDecorator>(service);
        Assert.Equal("Second[Decorated(Original)]", service.Execute());
    }

    [Fact(DisplayName = "Decorate - Deve lançar exceção quando serviço não está registrado")]
    public void Decorate_DeveLancarExcecao_QuandoServicoNaoRegistrado()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            services.Decorate<ITestService, TestDecorator>());

        Assert.Contains("is not registered", exception.Message);
    }

    [Fact(DisplayName = "Decorate - Deve decorar serviço com implementação factory")]
    public void Decorate_DeveDecorarServicoComImplementacaoFactory()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<ITestService>(sp => new TestService());

        // Act
        services.Decorate<ITestService, TestDecorator>();
        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<ITestService>();

        // Assert
        Assert.IsType<TestDecorator>(service);
        Assert.Equal("Decorated(Original)", service.Execute());
    }

    [Fact(DisplayName = "Decorate - Deve decorar serviço com instância específica")]
    public void Decorate_DeveDecorarServicoComInstanciaEspecifica()
    {
        // Arrange
        var services = new ServiceCollection();
        var instance = new TestService();
        services.AddSingleton<ITestService>(instance);

        // Act
        services.Decorate<ITestService, TestDecorator>();
        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<ITestService>();

        // Assert
        Assert.IsType<TestDecorator>(service);
        Assert.Equal("Decorated(Original)", service.Execute());
    }

    [Fact(DisplayName = "Decorate - Deve respeitar o lifetime do serviço original")]
    public void Decorate_DeveRespeitarLifetimeDoServicoOriginal()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ITestService, TestService>();

        // Act
        services.Decorate<ITestService, TestDecorator>();
        var provider = services.BuildServiceProvider();
        
        var service1 = provider.GetRequiredService<ITestService>();
        var service2 = provider.GetRequiredService<ITestService>();

        // Assert - Singleton deve retornar a mesma instância
        Assert.Same(service1, service2);
    }

    [Fact(DisplayName = "Decorate - Deve retornar IServiceCollection para encadeamento")]
    public void Decorate_DeveRetornarIServiceCollectionParaEncadeamento()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<ITestService, TestService>();

        // Act & Assert
        var result = services.Decorate<ITestService, TestDecorator>();
        
        Assert.Same(services, result);
    }

    [Fact(DisplayName = "Decorate - Múltiplas instâncias devem ser independentes com Transient")]
    public void Decorate_MultiplasInstanciasDevemSerIndependentes_ComTransient()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<ITestService, TestService>();
        services.Decorate<ITestService, TestDecorator>();

        var provider = services.BuildServiceProvider();

        // Act
        var service1 = provider.GetRequiredService<ITestService>();
        var service2 = provider.GetRequiredService<ITestService>();

        // Assert - Transient deve retornar instâncias diferentes
        Assert.NotSame(service1, service2);
        Assert.IsType<TestDecorator>(service1);
        Assert.IsType<TestDecorator>(service2);
    }
}

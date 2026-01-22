using System.Diagnostics;
using DesignPatternSamples.Application.DTO;
using DesignPatternSamples.Application.Services;
using Microsoft.Extensions.Logging;

namespace DesignPatternSamples.Application.Decorators;

public class DetranVerificadorDebitosDecoratorLogger : IDetranVerificadorDebitosService
{
    private readonly IDetranVerificadorDebitosService _inner;
    private readonly ILogger<DetranVerificadorDebitosDecoratorLogger> _logger;

    public DetranVerificadorDebitosDecoratorLogger(
        IDetranVerificadorDebitosService inner,
        ILogger<DetranVerificadorDebitosDecoratorLogger> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<IEnumerable<DebitoVeiculo>> ConsultarDebitos(Veiculo veiculo)
    {
        var watch = Stopwatch.StartNew();
        _logger.LogInformation("Iniciando a execução do método ConsultarDebitos({Veiculo})", veiculo);
        var result = await _inner.ConsultarDebitos(veiculo);
        watch.Stop();
        _logger.LogInformation("Encerrando a execução do método ConsultarDebitos({Veiculo}) {ElapsedTime}ms", veiculo, watch.ElapsedMilliseconds);
        return result;
    }
}

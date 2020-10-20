using DesignPatternSamples.Application.DTO;
using DesignPatternSamples.Application.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DesignPatternSamples.Application.Decorators
{
    public class DetranVerificadorDebitosDecoratorLogger : IDetranVerificadorDebitosService
    {
        private readonly IDetranVerificadorDebitosService _Inner;
        private readonly ILogger<DetranVerificadorDebitosDecoratorLogger> _Logger;

        public DetranVerificadorDebitosDecoratorLogger(
            IDetranVerificadorDebitosService inner,
            ILogger<DetranVerificadorDebitosDecoratorLogger> logger)
        {
            _Inner = inner;
            _Logger = logger;
        }

        public async Task<IEnumerable<DebitoVeiculo>> ConsultarDebitos(Veiculo veiculo)
        {
            Stopwatch watch = Stopwatch.StartNew();
            _Logger.LogInformation($"Iniciando a execução do método ConsultarDebitos({veiculo})");
            var result = await _Inner.ConsultarDebitos(veiculo);
            watch.Stop(); 
            _Logger.LogInformation($"Encerrando a execução do método ConsultarDebitos({veiculo}) {watch.ElapsedMilliseconds}ms");
            return result;
        }
    }
}
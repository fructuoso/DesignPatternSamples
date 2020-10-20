using DesignPatternSamples.Application.DTO;
using DesignPatternSamples.Application.Repository;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesignPatternSamples.Infra.Repository.Detran
{
    public class DetranSPVerificadorDebitosRepository : IDetranVerificadorDebitosRepository
    {
        private readonly ILogger _Logger;

        public DetranSPVerificadorDebitosRepository(ILogger<DetranSPVerificadorDebitosRepository> logger)
        {
            _Logger = logger;
        }

        public Task<IEnumerable<DebitoVeiculo>> ConsultarDebitos(Veiculo veiculo)
        {
            _Logger.LogDebug($"Consultando débitos do veículo placa {veiculo.Placa} para o estado de SP.");
            return Task.FromResult<IEnumerable<DebitoVeiculo>>(new List<DebitoVeiculo>() { new DebitoVeiculo() });
        }
    }
}

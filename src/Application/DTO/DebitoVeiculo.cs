using System;

namespace DesignPatternSamples.Application.DTO
{
    [Serializable]
    public class DebitoVeiculo
    {
        public DateTime DataOcorrencia { get; set; }
        public string Descricao { get; set; }
        public double Valor { get; set; }
    }
}
using System;

namespace DesignPatternSamples.Application.DTO
{
    [Serializable]
    public class PontosCarteira
    {
        public DateTime DataOcorrencia { get; set; }
        public string Descricao { get; set; }
        public double Quantidade { get; set; }
    }
}
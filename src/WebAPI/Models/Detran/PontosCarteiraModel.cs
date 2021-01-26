using System;

namespace DesignPatternSamples.WebAPI.Models.Detran
{
    public class PontosCarteiraModel
    {
        public DateTime DataOcorrencia { get; set; }
        public string Descricao { get; set; }
        public double Quantidade { get; set; }
    }
}
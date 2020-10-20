using System;

namespace DesignPatternSamples.WebAPI.Models.Detran
{
    public class DebitoVeiculoModel
    {
        public DateTime DataOcorrencia { get; set; }
        public string Descricao { get; set; }
        public double Valor { get; set; }
    }
}
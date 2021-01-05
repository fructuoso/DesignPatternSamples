using System;

namespace DesignPatternSamples.WebAPI.Models.Detran
{
    public class PontosCarteiraModel
    {
        public DateTime DataOcorrencia { get; set; }
        public string CodigoInfracao { get; set; }
        public bool SituacaoAtiva { get; set; }
        public int Quantidade { get; set; }
    }
}
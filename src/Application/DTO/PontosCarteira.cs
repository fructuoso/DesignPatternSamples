using System;

namespace DesignPatternSamples.Application.DTO
{
    [Serializable]
    public class PontosCarteira
    {
        public DateTime DataOcorrencia { get; set; }
        public string CodigoInfracao { get; set; }
        public bool SituacaoAtiva { get; set; }
        public int Quantidade { get; set; }
    }
}
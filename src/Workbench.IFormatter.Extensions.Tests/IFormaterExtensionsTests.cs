using System;
using Xunit;

namespace Workbench.IFormatter.Extensions.Tests
{
    public class IFormaterExtensionsTests
    {
        [Serializable]
        private struct PessoaFisica
        {
            public string Nome { get; set; }
            public string NomeMae { get; set; }
            public string CPF { get; set; }
        }

        [Fact(DisplayName = "Dado um Objeto que será serializado e deserializado utilizando o serializador 'Default' devemos ter um novo Objeto resultante igual ao original")]
        public void SerializeDeserializeDefaultSerializer()
        {
            PessoaFisica pessoaOriginal = new PessoaFisica() { Nome = "Victor Fructuoso", CPF = "111.111.111-11", NomeMae = "Ana Paula" };

            PessoaFisica resultado = pessoaOriginal
                .Serialize()
                .Deserialize<PessoaFisica>();

            Assert.Equal(pessoaOriginal, resultado);
        }
    }
}

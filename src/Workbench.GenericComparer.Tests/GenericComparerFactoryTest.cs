using System.Collections.Generic;
using System.Linq;
using Workbench.Comparer;
using Xunit;

namespace Workbench.GenericComparer.Tests
{
    public class GenericComparerFactoryTest
    {
        private struct PessoaFisica
        {
            public string Nome { get; set; }
            public string NomeMae { get; set; }
            public string CPF { get; set; }
        }

        [Fact(DisplayName = "Data uma coleção com 3 objetos sendo 2 iguais então o DISTINCT com uma comparação simples deve retornar uma lista com 2 objetos.")]
        public void ListagemComItensRepetidosComparecaoSimples()
        {
            IEnumerable<PessoaFisica> pessoas = new List<PessoaFisica>()
            {
                new PessoaFisica() { Nome = "Victor Fructuoso", NomeMae = "Ana", CPF = "111.111.111-11" },
                new PessoaFisica() { Nome = "Victor Fructuoso", NomeMae = "Paula", CPF = "111.111.111-11" },
                new PessoaFisica() { Nome = "Victor Fructuoso", NomeMae = "Ana", CPF = "222.222.222-22" }
            };

            IEnumerable<PessoaFisica> pessoasDiferentes = pessoas.Distinct(GenericComparerFactory<PessoaFisica>.Create(p => p.CPF));

            Assert.NotNull(pessoasDiferentes);
            Assert.True(pessoasDiferentes.Any());
            Assert.Equal(2, pessoasDiferentes.Count());
            Assert.DoesNotContain(pessoasDiferentes, p => p.NomeMae == "Paula");
        }

        [Fact(DisplayName = "Data uma coleção com 3 objetos sendo 2 iguais então o DISTINCT com uma comparação composta deve retornar uma lista com 2 objetos.")]
        public void ListagemComItensRepetidosComparecaoComposta()
        {
            IEnumerable<PessoaFisica> pessoas = new List<PessoaFisica>()
            {
                new PessoaFisica() { Nome = "Victor Fructuoso", NomeMae = "Ana", CPF = "111.111.111-11" },
                new PessoaFisica() { Nome = "Victor Fructuoso", NomeMae = "Paula", CPF = "111.111.111-11" },
                new PessoaFisica() { Nome = "Victor Fructuoso", NomeMae = "Ana", CPF = "222.222.222-22" }
            };

            IEnumerable<PessoaFisica> pessoasDiferentes = pessoas.Distinct(GenericComparerFactory<PessoaFisica>.Create(p => new { p.Nome, p.NomeMae }));

            Assert.NotNull(pessoasDiferentes);
            Assert.True(pessoasDiferentes.Any());
            Assert.Equal(2, pessoasDiferentes.Count());
            Assert.DoesNotContain(pessoasDiferentes, p => p.CPF == "222.222.222-22");
        }
    }
}
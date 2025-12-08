using Workbench.Comparer;
using Xunit;

namespace Workbench.GenericComparer.Tests;

public class GenericComparerFactoryTest
{
    private struct PessoaFisica
    {
        public required string Nome { get; init; }
        public required string NomeMae { get; init; }
        public required string CPF { get; init; }
    }

    [Fact(DisplayName = "Data uma coleção com 3 objetos sendo 2 iguais então o DISTINCT com uma comparação simples deve retornar uma lista com 2 objetos.")]
    public void ListagemComItensRepetidosComparecaoSimples()
    {
        IEnumerable<PessoaFisica> pessoas = [
            new() { Nome = "Victor Fructuoso", NomeMae = "Ana", CPF = "111.111.111-11" },
            new() { Nome = "Victor Fructuoso", NomeMae = "Paula", CPF = "111.111.111-11" },
            new() { Nome = "Victor Fructuoso", NomeMae = "Ana", CPF = "222.222.222-22" }
        ];

        pessoas.DistinctBy(p => p.CPF);
        var pessoasDiferentes = pessoas.Distinct(GenericComparerFactory<PessoaFisica>.Create(p => p.CPF));

        Assert.NotNull(pessoasDiferentes);
        Assert.True(pessoasDiferentes.Any());
        Assert.Equal(2, pessoasDiferentes.Count());
    }

    [Fact(DisplayName = "Data uma coleção com 3 objetos sendo 2 iguais então o DISTINCT com uma comparação múltipla deve retornar uma lista com 2 objetos.")]
    public void ListagemComItensRepetidosComparecaoMultipla()
    {
        IEnumerable<PessoaFisica> pessoas = [
            new() { Nome = "Victor Fructuoso", NomeMae = "Ana", CPF = "111.111.111-11" },
            new() { Nome = "Victor Fructuoso", NomeMae = "Ana", CPF = "222.222.222-22" },
            new() { Nome = "Victor Fructuoso", NomeMae = "Paula", CPF = "333.333.333-33" }
        ];

        var pessoasDiferentes = pessoas.Distinct(GenericComparerFactory<PessoaFisica>.Create(p => new { p.Nome, p.NomeMae }));

        Assert.NotNull(pessoasDiferentes);
        Assert.True(pessoasDiferentes.Any());
        Assert.Equal(2, pessoasDiferentes.Count());
        Assert.DoesNotContain(pessoasDiferentes, p => p.CPF == "222.222.222-22");
    }
}

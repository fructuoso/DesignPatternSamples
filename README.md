# DesignPatternSamples
Aplicação de exemplo de aplicação de Design Patterns na prática em um projeto WebAPI .NET Core 3.1
## Testes de Cobertura

Passo a passo sobre como executar os testes unitários (e calcular o code coverage) localmente antes de realizar o commit.

### Pré-Requisitos

Para gerar o relatório é necessário instalar o **dotnet-reportgenerator-globaltool**

```script
dotnet tool install --global dotnet-reportgenerator-globaltool --version 4.6.1
````

### Execução

Executar o **.bat** para realizar a execução dos testes automatizados com a extração do relatório de cobertura na sequência.

```bat
$ test-coverage.bat
```

## Padrões na Prática

### Strategy

#### Problema:

Utilizar o método Distinct do System.Linq, este espera uma IEqualityComparer.

Nós não queremos criar uma única implementação engessada que nos permita comparar os objetos de uma única forma.

##### Solução:

1. Criar uma classe que implemente a interface IEqualityComparer;
2. Esta classe deve receber o 'como' os objetos deverão ser comparados através de um parametro;

<u>Desta forma a classe que criamos sabe comparar objetos, porém ela não sabe os critérios que serão utilizados, os critérios serão injetados através de uma função anônima.</u>

* [Implementação](src/Workbench.Comparer/GenericComparerFactory.cs)
* [Consumo](src/Workbench.GenericComparer.Tests/GenericComparerFactoryTest.cs#L27)

Podemos tornar o consumo ainda mais interessante criando uma *Sugar Sintax* através de métodos de extensão.

* [Implementação](src/Workbench.Linq.Extensions/DistinctExtensions.cs)
* [Consumo](src/Workbench.Linq.Extensions.Tests/DistinctExtensionsTests.cs#L26)
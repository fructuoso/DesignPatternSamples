# DesignPatternSamples

> **📋 Nota:** Este repositório foi criado originalmente para uma apresentação sobre Design Patterns, desenvolvido inicialmente em .NET Core 3.1 e posteriormente **migrado para .NET 8** com as melhores práticas e recursos modernos do C# 12.

Aplicação de exemplo de aplicação de Design Patterns na prática em um projeto WebAPI .NET 8 utilizando as melhores práticas e recursos modernos do C# 12. Projeto utilizado na palestra "Aplicando design patterns na prática com C#" ([Link Apresentação](Apresenta%C3%A7%C3%A3o/Aplicando%20design%20patterns%20na%20pr%C3%A1tica%20com%20C%23.pdf))

## Testes de Cobertura

Passo a passo sobre como executar os testes unitários (e calcular o code coverage) localmente antes de realizar o commit.

<u>Obs.: O Visual Studio possui esta funcionalidade nativamente nas versões Enterprise e Professional.</u>

### Pré-Requisitos

- **.NET 8 SDK** ou superior
- **dotnet-reportgenerator-globaltool** para gerar relatórios de cobertura

```bash
# Instalar .NET 8 SDK (se não estiver instalado)
# Verificar versões instaladas
dotnet --list-sdks

# Instalar ferramenta de relatórios globalmente
dotnet tool install --global dotnet-reportgenerator-globaltool
```

### Execução

Executar os comandos para realizar a execução dos testes automatizados:

```bash
# Executar todos os testes
dotnet test

# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Scripts automatizados
# Windows
test-coverage.bat

# Unix/Linux/macOS  
./test-coverage.sh
```

## Padrões na Prática

### Strategy

#### Problema:

Nosso objetivo é Utilizar o método Distinct do System.Linq, este por sua vez espera como entrada uma IEqualityComparer. Isso por si só já representa uma implementação de Strategy, entretanto nós não queremos criar uma única implementação engessada que nos permita comparar um determinado objeto de uma única forma.

##### Solução:

1. Criar uma classe que implemente a interface [IEqualityComparer](https://docs.microsoft.com/pt-br/dotnet/api/system.collections.generic.iequalitycomparer-1?view=net-8.0);
2. Esta classe deve receber o 'como' os objetos deverão ser comparados através de um parâmetro, que neste caso é uma função anônima;

Desta forma a classe que criamos sabe comparar objetos, porém ela não sabe os critérios que serão utilizados, os critérios serão injetados através de uma função anônima.

**Características modernas do .NET 8:**
- Uso de **file-scoped namespaces**
- **Collection expressions** (`[]` em vez de `new List<>()`)
- **Required properties** nos modelos

[Implementação](src/Workbench.Comparer/GenericComparerFactory.cs)\
[Consumo](src/Workbench.GenericComparer.Tests/GenericComparerFactoryTest.cs#L27)

Podemos tornar o consumo ainda mais interessante criando uma *Sugar Syntax* através de métodos de extensão.

[Implementação](src/Workbench.Linq.Extensions/DistinctExtensions.cs)\
[Consumo](src/Workbench.Linq.Extensions.Tests/DistinctExtensionsTests.cs#L26)

### 💡 Evolução Natural: DistinctBy Nativo

Um exemplo interessante de como os padrões evoluem: nosso método de extensão personalizado `DistinctBy` foi incorporado nativamente no **.NET 6** e posteriores. 

Como mencionado na apresentação: *"Alguns padrões surgiram para solucionar limitações de linguagens de programação com menos recursos no que diz respeito à abstração [...] Linguagens mais recentes trazem alguns destes recursos nativamente"*.

Veja a comparação:

**Nossa implementação personalizada (.NET Core 3.1):**
```csharp
IEnumerable<PessoaFisica> pessoasDiferentes = pessoas.Distinct(p => new { p.Nome, p.NomeMae });
```

**Método nativo do .NET 8:**
```csharp
IEnumerable<PessoaFisica> pessoasDiferentes = pessoas.DistinctBy(p => new { p.Nome, p.NomeMae });
```

[Teste demonstrando ambas as abordagens](src/Workbench.Linq.Extensions.Tests/DistinctExtensionsTests.cs#L52)

Desta forma através do padrão [Strategy](#strategy) estamos aderentes ao princípio **Aberto-Fechado** e **Inversão de Controle**.

### Factory

#### Problema: 

Nós queremos criar um serviço de consulta de débitos do veículo que seja capaz de acessar o sistema do DETRAN e obter estas informações, porém o DETRAN possui uma aplicação completamente diferente de acordo com o estado.

Nós não queremos modificar o nosso serviço sempre que um novo estado for implementado.

#### Solução:

1. Criar uma interface que determine uma assinatura única para o serviço;
2. Realizar uma implementação para cada um dos estados;
3. Criar uma classe Factory, onde sua responsabilidade será determinar qual classe concreta deverá ser instanciada;

[Implementação](src/Infra.Repository.Detran/DetranVerificadorDebitosFactory.cs)\
[Consumo](src/Application/Implementations/DetranVerificadorDebitosServices.cs#L20)\
[Teste](src/Infra.Repository.Detran.Tests/DetranVerificadorDebitosFactoryTests.cs#L22)

Desta forma através do padrão [Factory](#factory) estamos aderentes ao princípio **Aberto-Fechado**.

Neste exemplo o nosso [Factory](#factory) ainda está diretamente relacionado ao princípio **Substituição de Liskov**.

### Singleton

#### Problema:

Visto que o nosso Factory tem como responsabilidade apenas identificar a classe concreta que teve ser inicializada a partir de um Setup pré-estabelecido no [Program.cs](src/WebAPI/Program.cs) da aplicação, não faz sentido que ele seja instanciado a cada solicitação.

#### Solução:

Como estamos fazendo uso da Injeção de Dependência nativa do .NET 8, o processo se torna mais simples:

1. Modificar o registro no Program.cs para que o serviço seja registrado como Singleton.

[Implementação](src/WebAPI/Program.cs)

Com isso nós temos uma única instância sendo inicializada e configurada no [Program.cs](src/WebAPI/Program.cs) da aplicação usando a arquitetura moderna de **Minimal APIs**.

### Template Method

#### Problema:

Visto que em algum momento iremos implementar 27 serviços diferentes para acessar os DETRAN que temos espalhados pelo Brasil.

Entendemos que, mesmo que, os sites sejam diferentes, os passos necessários para extrair a informação costumam ser semelhantes:

* Consultar Site
* Consolidar Resultado

Como a nossa interface [IDetranVerificadorDebitosRepository](src/Application/Repository/IDetranVerificadorDebitosRepository.cs) possui apenas o método ConsultarDebitos, nosso código corre risco de não ficar padronizado e ainda perdermos o princípio da **Responsabilidade Única**.

#### Solução:

1. Criar uma classe abstrata com métodos mais específicos para realizar o trabalho desejado;
2. A classe abstrata 'deve' implementar o método exposto pela Interface;
3. Ao invés das classes implementarem a Interface, elas herdarão o comportamento da classe abstrata, implementando apenas os métodos mais específicos.

Com isso torna-se mais fácil:
* Dividir o trabalho;
* Testar o código.

[Implementação](src/Infra.Repository.Detran/DetranVerificadorDebitosRepositoryCrawlerBase.cs)\
[Consumo](src/Infra.Repository.Detran/DetranPEVerificadorDebitosRepository.cs)

O neste exemplo o nosso [Template Method](#template-method) ainda seguindo o princípio **Segregação da Interface**, onde os métodos específicos foram adicionados na nossa classe abstrata [DetranVerificadorDebitosRepositoryCrawlerBase](src/Repository.Detran/../Infra.Repository.Detran/DetranVerificadorDebitosRepositoryCrawlerBase.cs), desta forma conseguimos atingir também o princípio de **Substituição de Liskov**.

### Decorator

#### Problema: 

Com o serviço [DetranVerificadorDebitosServices](src/Application/Implementations/DetranVerificadorDebitosServices.cs) identificamos que precisamos adicionar funcionalidades técnicas a ele (como por exemplo **Log** e **Cache**), porém essas funcionalidades não devem gerar acoplamento no nosso código.

Então como fazer isso sem quebrar os princípios **Responsabilidade Única** e **Aberto-Fechado**?

#### Solução:

Neste cenário estamos usando uma abordagem que nos permite transferir a complexidade de registrar um Decorator no ServiceCollection para um método de extensão.

Desta forma precisamos:

1. Criar uma nova classe concreta que deverá implementar a Interface que será 'decorada';
2. Implementar nesta nova classe a funcionalidade que gostaríamos de acrescentar ao método em questão;
3. Adicionar Decorator no Injetor de Dependências fazendo referência à interface que será decorada.

Obs.: É possível incluir mais de um Decorator, porém é preciso ter ciência de que a ordem em que eles são associados faz diferença no resultado final.

[Método de Extensão](src/Workbench.DependencyInjection.Extensions/ServiceCollectionExtensions.cs#L10)\
[Implementação](src/Application/Decorators/DetranVerificadorDebitosDecoratorLogger.cs)\
[Registro](src/WebAPI/Program.cs)

O Decorator funciona como uma 'Boneca Russa' dessa forma podemos 'empilhar' diversos Decorators em uma mesma Interface.

Temos o exemplo de um segundo Decorator adicionando o recurso de Cache ao nosso Service.

[Implementação](src/Application/Decorators/DetranVerificadorDebitosDecoratorCache.cs)\
[Registro](src/WebAPI/Program.cs)

<u>Obs.: Seguir o princípio Segregação de Interfaces pode tornar o seu Decorator mais simples de ser implementado, visto que você terá menos métodos para submeter ao padrão.</u>

### 📋 Principais Atualizações

#### **Arquitetura Moderna**
- **Minimal APIs** - Substituição do modelo `Startup.cs` por `Program.cs` com top-level statements
- **Simplified hosting model** - Configuração mais direta e enxuta

#### **Recursos do C# 12**
- **File-scoped namespaces** - Redução da indentação e código mais limpo
- **Collection expressions** - `[]` em vez de `new List<>()`
- **Required properties** - Maior segurança na inicialização de objetos
- **Primary constructors** - Sintaxe mais concisa para construtores

#### **Nullable Reference Types**
- Habilitado em todos os projetos para maior segurança de tipos
- Tratamento adequado de valores null em todo o codebase

#### **Serialização Moderna**
- **System.Text.Json** substituindo `Newtonsoft.Json`
- Remoção do `BinaryFormatter` obsoleto
- Configurações otimizadas para performance

#### **Logging Estruturado**
- **Serilog** integrado com .NET 8
- **Structured logging** com interpolação segura
- Melhor rastreabilidade e debugging

#### **Testes Atualizados**
- **xUnit 2.9.2** - Versão mais recente
- **Coverlet 6.0.2** - Análise de cobertura moderna
- Sintaxe moderna nos testes

### 🔄 Compatibilidade

O projeto mantém **100% de compatibilidade funcional** com a versão anterior, preservando:
- Todos os Design Patterns implementados
- APIs e contratos existentes  
- Comportamento dos serviços
- Estrutura de testes

### 🎯 Performance

As atualizações para .NET 8 trouxeram melhorias significativas em:
- **Throughput** das APIs
- **Memory allocation** reduzida
- **Startup time** otimizado
- **JSON serialization** mais rápida

---

> **🤖 Nota sobre Refatoração:** Todo o código deste repositório foi **100% refatorado pelo GitHub Copilot**, demonstrando o potencial das ferramentas de IA na modernização de código legado. A migração do .NET Core 3.1 para .NET 8 e a aplicação das melhores práticas do C# 12 foram realizadas com assistência da IA, mantendo a integridade funcional e os padrões de design originais.
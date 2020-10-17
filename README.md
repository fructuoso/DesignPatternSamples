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
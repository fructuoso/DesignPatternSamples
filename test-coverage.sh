#!/bin/bash

# =============================================================================
# Script de Análise de Cobertura de Código - DesignPatternSamples
# =============================================================================
# 
# Este script executa os testes unitários e gera um relatório de cobertura
# de código usando dotnet test e reportgenerator.
# 
# Funcionalidades:
# - Remove resultados anteriores
# - Executa testes com coleta de cobertura
# - Gera relatório HTML interativo
# - Abre o relatório automaticamente (se possível)
#
# Compatível com: Linux, macOS, WSL
# =============================================================================

set -e  # Sair em caso de erro

echo "🧪 Executando testes com análise de cobertura..."
echo "📁 Projeto: DesignPatternSamples (.NET 8)"
echo ""

# Remove diretório de resultados anteriores
if [ -d "CoverageResults" ]; then
    echo "🗑️  Removendo resultados anteriores..."
    rm -rf CoverageResults
    echo "✅ Limpeza concluída"
fi

echo ""
echo "🔍 Executando testes..."
echo "⏱️  Aguarde, isso pode levar alguns momentos..."
echo ""

# Executa os testes com cobertura usando coverlet
dotnet test ./src/DesignPatternSamples.sln \
    --collect:"XPlat Code Coverage" \
    --results-directory:./CoverageResults \
    --logger:"console;verbosity=minimal" \
    --configuration:Release \
    -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.ExcludeByFile="**/Program.cs"

# Verifica se os testes foram executados com sucesso
if [ $? -eq 0 ]; then
    echo "✅ Testes executados com sucesso!"
    
    # Verifica se o reportgenerator está instalado
    if ! command -v reportgenerator &> /dev/null; then
        echo "⚠️  reportgenerator não encontrado. Instalando..."
        dotnet tool install --global dotnet-reportgenerator-globaltool
    fi
    
    echo "📊 Gerando relatório HTML..."
    
    # Encontra todos os arquivos de cobertura gerados
    COVERAGE_FILES=$(find ./CoverageResults -name "coverage.cobertura.xml" | tr '\n' ';')
    
    if [ -z "$COVERAGE_FILES" ]; then
        echo "⚠️  Arquivos de cobertura não encontrados. Procurando outros formatos..."
        COVERAGE_FILES=$(find ./CoverageResults -name "*.cobertura.xml" | tr '\n' ';')
    fi
    
    if [ -n "$COVERAGE_FILES" ]; then
        # Remove o último ponto e vírgula
        COVERAGE_FILES=${COVERAGE_FILES%;}
        
        # Conta quantos arquivos foram encontrados
        FILE_COUNT=$(echo "$COVERAGE_FILES" | tr ';' '\n' | wc -l)
        echo "📁 Encontrados $FILE_COUNT arquivo(s) de cobertura"
        
        # Gera o relatório HTML agregando todos os arquivos
        echo "📊 Gerando relatório HTML detalhado..."
        reportgenerator \
            -reports:"$COVERAGE_FILES" \
            -targetdir:"CoverageResults/Report" \
            -reporttypes:Html\;HTMLSummary\;Badges \
            -title:"DesignPatternSamples - Code Coverage Report"
    else
        echo "❌ Arquivos de cobertura não encontrados em CoverageResults/"
        echo "💡 Verifique se os testes possuem cobertura configurada"
        exit 1
    fi
    
    if [ $? -eq 0 ]; then
        echo ""
        echo "✅ Relatório gerado com sucesso!"
        echo ""
        echo "📄 Arquivos disponíveis:"
        echo "   📊 Relatório principal: CoverageResults/Report/index.html" 
        echo "   📋 Resumo executivo:    CoverageResults/Report/summary.html"
        echo "   🏆 Badges:             CoverageResults/Report/badge_*.svg"
        echo ""
        
        # Tenta abrir o relatório automaticamente
        if command -v xdg-open &> /dev/null; then
            echo "🌐 Abrindo relatório no navegador padrão..."
            xdg-open "./CoverageResults/Report/index.html" &
        elif command -v open &> /dev/null; then
            echo "🌐 Abrindo relatório no navegador padrão..."
            open "./CoverageResults/Report/index.html" &
        elif command -v wsl-open &> /dev/null; then
            echo "🌐 Abrindo relatório no navegador (WSL)..."
            wsl-open "./CoverageResults/Report/index.html" &
        else
            echo "ℹ️  Para visualizar o relatório, execute:"
            echo "   xdg-open ./CoverageResults/Report/index.html"
            echo "   ou abra manualmente no navegador"
        fi
    else
        echo ""
        echo "❌ Erro ao gerar relatório"
        echo "💡 Verifique se o reportgenerator está funcionando corretamente"
        exit 1
    fi
else
    echo ""
    echo "❌ Falha na execução dos testes"
    echo "💡 Verifique os erros acima e corrija antes de prosseguir"
    exit 1
fi

echo ""
echo "🎉 Análise de cobertura concluída com sucesso!"
echo "⚡ Projeto: DesignPatternSamples (.NET 8)"
echo "📈 Use os relatórios para melhorar a qualidade do código"
echo ""
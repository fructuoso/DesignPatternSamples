# CI Setup Instructions

## 🎯 Visão Geral

Este projeto usa **apenas CI (Continuous Integration)** - não há CD (Continuous Delivery/Deployment) pois não entregamos para nenhum ambiente externo.

O workflow de CI executa **tudo junto em cada PR**:
- 🎨 Code formatting
- 🏗️ Build 
- 🔍 Static analysis
- 🔒 Security audit
- 🧪 Tests
- 📊 Coverage

## 🛠️ Configuração Inicial

### 1. Configurar Branch Protection
Para garantir qualidade do código:

1. Vá para **Settings** → **Branches**
2. Adicione uma regra para `main`:
   - ✅ Require status checks before merging
   - ✅ Require branches to be up to date before merging
   - Selecione o check obrigatório:
     - `ci` (CI - Quality & Coverage)

## 🚀 Workflow Único

### **CI - Quality & Coverage** (`.github/workflows/ci.yml`)
- **Trigger**: PR e push para main/develop (apenas mudanças em `src/**`)
- **Executa TUDO em sequência**:
  1. 🎨 **Code formatting** - `dotnet format --verify-no-changes`
  2. 🏗️ **Build** - `dotnet build` em Release
  3. 🔍 **Static analysis** - `dotnet build` em Debug para warnings
  4. 🔒 **Security audit** - Check de vulnerabilidades e deprecated packages
  5. 🧪 **Tests** - `dotnet test` com coleta de coverage
  6. 📊 **Coverage report** - Geração e verificação de threshold (80%)
  7. 💬 **PR comment** - Comentário automático com resultados de coverage

## 📋 Como Usar

### Para Development
```bash
# 1. Criar feature branch
git checkout -b feature/minha-funcionalidade

# 2. Fazer mudanças no código
# ... desenvolver ...

# 3. Commit e push
git add .
git commit -m "feat: minha funcionalidade"
git push origin feature/minha-funcionalidade

# 4. Abrir PR
# O workflow CI rodará automaticamente testando TUDO
```

### Para Verificação Local
```bash
# Executar o mesmo pipeline localmente
./qa-pipeline.sh

# Ou executar partes específicas
cd src/
dotnet format --verify-no-changes  # formatting
dotnet build                       # build
dotnet test --collect:"XPlat Code Coverage"  # tests + coverage
```

## 📊 Monitoring

### GitHub Actions
- **Status**: Visível no PR como check único "ci"
- **Logs**: Detalhados para cada etapa
- **Artifacts**: Coverage report e test results sempre disponíveis

### Coverage Reports
- **PR Comments**: Automáticos em cada PR
- **Artifacts**: HTML reports baixáveis
- **Threshold**: 80% (configurável)

### Badges
Adicione no README.md:
```markdown
![CI](https://github.com/{username}/{repo}/workflows/CI%20-%20Quality%20&%20Coverage/badge.svg)
```

## 🔧 Customização

### Alterar Coverage Threshold
Edite `.github/workflows/ci.yml`:
```yaml
env:
  COVERAGE_THRESHOLD: '80'  # Altere para o valor desejado
```

### Alterar Paths de Trigger
```yaml
on:
  pull_request:
    paths:
      - 'src/**'          # Apenas mudanças no código
      - 'global.json'     # Mudanças na configuração .NET
```

### Personalizar Checks
Edite `.github/workflows/ci.yml` - cada step está bem documentado e pode ser modificado independentemente.

## 🚨 Troubleshooting

### CI Falhou - Checklist
1. **Formatting**: Execute `dotnet format` na pasta `src/`
2. **Build**: Execute `dotnet build` na pasta `src/`
3. **Tests**: Execute `dotnet test` na pasta `src/`
4. **Coverage**: Verifique se está acima de 80%
5. **References**: Valide ProjectReferences no monorepo

### Coverage Baixo
1. Adicione mais testes unitários
2. Remova código não testável
3. Ajuste o threshold temporariamente se necessário

### Security Issues
1. Execute `dotnet list package --vulnerable`
2. Update pacotes vulneráveis
3. Execute `dotnet list package --deprecated` 
4. Considere substituir pacotes deprecated

## 🎯 Filosofia

**Simples e Eficaz:**
- ✅ Um único workflow que faz tudo
- ✅ Falha rápido - para na primeira falha
- ✅ Feedback imediato no PR
- ✅ Zero configuração complexa
- ✅ Sem CD - apenas CI de qualidade

**Focado em Qualidade:**
- 🎨 Código sempre formatado
- 🏗️ Build sempre funcionando  
- 🧪 Testes sempre passando
- 📊 Coverage sempre adequado
- 🔒 Segurança sempre verificada
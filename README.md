# MicroondasApp

Projeto ASP.NET 8 para simulação de funcionamento de um microondas, com programas pré-definidos, cadastro de programas customizados, controle de aquecimento, sistema de autenticação JWT, pausa, continuação, cancelamento e documentação automática via Swagger.

## Visão Geral

O MicroondasApp foi desenvolvido para simular as principais funções de um microondas moderno, incluindo:

- **Sistema de autenticação JWT** com registro e login de usuários
- **Programas de aquecimento** pré-definidos (Pipoca, Leite, Carnes de boi, Frango, Feijão) e customizados
- **Controle completo de aquecimento** com tempo (1-120s), potência (1-10) e caractere personalizado
- **Funcionalidades avançadas**: pausa, continuação, cancelamento e início rápido
- **Persistência em JSON** para programas customizados e usuários
- **Sistema robusto de logging** com tratamento global de exceções
- **API documentada** automaticamente via Swagger

## Estrutura do Projeto

### Controllers
- **Controllers/MicroondasController.cs**: Endpoints principais para gerenciamento de programas e operações do microondas
- **Controllers/AuthController.cs**: Endpoints de autenticação (registro, login e status)

### Application Layer
- **Application/MicroondasService.cs**: Lógica central de aquecimento e validações de negócio
- **Application/MicroondasStateService.cs**: Gerenciamento de estado do microondas (pausado, em aquecimento, etc.)
- **Application/UsuarioService.cs**: Gerenciamento de usuários com hash de senhas SHA256
- **Application/ErrorLogService.cs**: Serviço de logging para operações e login

### Domain Layer
- **Domain/Aquecimento.cs**: Entidade com regras de negócio para tempo e potência
- **Domain/RegrasDeNegocioException.cs**: Exception específica para violações de regras de negócio

### Infrastructure
- **Middleware/ExceptionMiddleware.cs**: Middleware para tratamento global de exceções
- **Program.cs**: Configuração da aplicação, serviços, JWT e pipeline de middleware

### Persistência
- **programas_customizados.json**: Programas criados pelos usuários
- **usuarios.json**: Base de usuários com senhas criptografadas
- **Logs**: Arquivos de log para operações e tentativas de login

## Recursos Técnicos

- **.NET 8 e C# 12**: Versões mais recentes para performance e recursos modernos
- **Autenticação JWT**: Tokens seguros com expiração de 2 horas
- **Middleware de Exception**: Tratamento global com respostas padronizadas e logging detalhado
- **Persistência JSON**: Solução simples e eficiente para o escopo do projeto
- **Swagger/OpenAPI**: Documentação interativa automática
- **Criptografia SHA256**: Senhas armazenadas com hash seguro
- **Validação rigorosa**: Regras de negócio centralizadas com exception específica

## Endpoints da API

### Autenticação (`/auth`)
> **Nota**: Todos os endpoints do microondas requerem autenticação JWT

- `POST /auth/register` — Registra novo usuário (nome, username, senha)
- `POST /auth/login` — Autentica usuário e retorna token JWT
- `GET /auth/status` — Verifica status de autenticação

### Gerenciamento de Programas (`/microondas`)

#### Consulta de Programas
- `GET /microondas` — Lista todos os programas (pré-definidos e customizados)
  - Retorna programas com IDs sequenciais
  - Programas customizados marcados com `Customizado: true`

#### Programas Customizados
- `POST /microondas/cadastrar` — Cadastra novo programa customizado
  ```json
  {
    "nome": "string",
    "alimento": "string", 
    "tempoSegundos": 1-120,
    "potencia": 1-10,
    "stringPersonalizada": "char único",
    "instrucoes": "string (opcional)"
  }
  ```

- `PUT /microondas/editar/{id}` — Edita programa customizado existente
  - `id`: ID do programa customizado (baseado na posição na lista)
  - Body: Mesmo formato do cadastro

- `DELETE /microondas/remover/{id}` — Remove programa customizado
  - `id`: ID do programa customizado

### Operações de Aquecimento (`/microondas`)

#### Iniciar Aquecimento
- `POST /microondas/iniciar` — Inicia aquecimento (múltiplos modos)
  
  **Modo 1: Por programa (ID)**
  ```json
  {
    "id": 1
  }
  ```
  
  **Modo 2: Manual (tempo + potência)**
  ```json
  {
    "tempoSegundos": 60,
    "potencia": 8
  }
  ```
  
  **Modo 3: Só tempo (potência padrão 10)**
  ```json
  {
    "tempoSegundos": 30
  }
  ```
  
  **Modo 4: Início rápido (30s, potência 10)**
  ```json
  {}
  ```

#### Controle de Estado
- `POST /microondas/pausar` — Pausa aquecimento atual
  - Se já pausado, **cancela** o aquecimento
  - Retorna status da operação

- `POST /microondas/continuar` — Retoma aquecimento pausado
  - Mantém tempo restante e configurações originais
  - Só funciona se houver aquecimento pausado

## Programas Pré-definidos

| Programa | Alimento | Tempo | Potência | Caractere | Instruções Especiais |
|----------|----------|-------|----------|-----------|---------------------|
| **Pipoca** | Pipoca de microondas | 3 min | 7 | `*` | Observar intervalos entre estouros |
| **Leite** | Leite | 5 min | 5 | `#` | Cuidado com choque térmico |
| **Carnes de boi** | Carne em pedaços | 14 min | 4 | `~` | Virar na metade do processo |
| **Frango** | Frango (qualquer corte) | 8 min | 7 | `@` | Virar na metade do processo |
| **Feijão** | Feijão congelado | 8 min | 9 | `+` | Manter destampado |

## Regras de Negócio

### Validações de Entrada
- **Tempo**: Entre 1 e 120 segundos
- **Potência**: Entre 1 e 10
- **Caractere personalizado**: Único, não pode ser "." (reservado para início rápido)
- **Autenticação**: Obrigatória para todas as operações do microondas

### Sistema de IDs
- **Programas pré-definidos**: IDs 1-5 (Pipoca, Leite, Carnes de boi, Frango, Feijão)
- **Programas customizados**: IDs sequenciais após os pré-definidos (6+)
- **Exclusão**: Apenas programas customizados podem ser editados/removidos

### Estado do Microondas
- **Controle de estado**: Sistema previne operações conflitantes
- **Pausa/Continuação**: Preserva configurações originais (tempo, potência, caractere)
- **Cancelamento**: Pausa dupla cancela o aquecimento completamente

### Validações Especiais
- **Potência sem tempo**: Retorna erro explicativo
- **Caractere duplicado**: Verifica contra todos os programas existentes
- **Programas inexistentes**: Validação de ID antes da execução

## Tratamento de Erros e Logging

- **Respostas padronizadas**: JSON com sucesso, mensagem, tipo e caminho
- **Status codes apropriados**: 400 para regras de negócio, 401 para autenticação, 500 para erros inesperados
- **Logging segregado**: Arquivos separados para operações do microondas e tentativas de login
- **Informações detalhadas**: StackTrace e InnerException para debugging

## Configuração e Execução

### Pré-requisitos
- .NET 8 SDK instalado
- Visual Studio 2022 ou VS Code (opcional)

### Configuração JWT
Configure as chaves JWT no `appsettings.json`:
```json
{
  "Jwt": {
    "Key": "sua-chave-secreta-aqui-minimo-32-caracteres",
    "Issuer": "MicroondasApp",
    "Audience": "MicroondasApp-Users"
  }
}
```

### Executando o Projeto
```bash
# Clone o repositório
git clone <url-do-repositorio>
cd MicroondasApp

# Execute o projeto
dotnet run

# Ou usando Visual Studio
# Pressione F5 ou Ctrl+F5
```

### Acessando a API
- **Swagger UI**: [http://localhost:5263](http://localhost:5263)
- **API Base**: [http://localhost:5263/api](http://localhost:5263/api)

## Fluxo de Uso Recomendado

### Primeira Utilização
1. **Registrar usuário**: `POST /auth/register` com nome, username e senha
2. **Fazer login**: `POST /auth/login` → receber token JWT
3. **Configurar headers**: Incluir `Authorization: Bearer <token>` em todas as requisições

### Operações Básicas
4. **Explorar programas**: `GET /microondas` para ver programas disponíveis
5. **Usar programa existente**: `POST /microondas/iniciar` com ID do programa
6. **Controlar aquecimento**: Use `/pausar` e `/continuar` conforme necessário

### Personalização
7. **Criar programa customizado**: `POST /microondas/cadastrar` com suas configurações
8. **Gerenciar programas**: Use `/editar/{id}` ou `/remover/{id}` para programas customizados
9. **Testar programa**: Use o novo programa através do ID retornado

## Arquivos Gerados

Durante a execução, a aplicação criará automaticamente:
- `usuarios.json` — Base de usuários registrados
- `programas_customizados.json` — Programas criados pelos usuários
- `operations.log` — Log de operações do microondas
- `login.log` — Log de tentativas de autenticação

## Observações de Desenvolvimento

- Projeto configurado para ambiente de desenvolvimento por padrão
- Swagger habilitado automaticamente
- Assembly info desabilitado para evitar conflitos
- Logs detalhados para facilitar debugging e auditoria
- Arquitetura em camadas para facilitar manutenção e testes

## Exemplos de Uso

### 1. Fluxo Completo de Autenticação
```bash
# Registrar usuário
curl -X POST "http://localhost:5263/auth/register" \
  -H "Content-Type: application/json" \
  -d '{"nome":"João","username":"joao123","senha":"minhasenha"}'

# Fazer login e obter token
curl -X POST "http://localhost:5263/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"joao123","senha":"minhasenha"}'

# Usar token nas próximas requisições
TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

### 2. Gerenciamento de Programas
```bash
# Listar todos os programas
curl -X GET "http://localhost:5263/microondas" \
  -H "Authorization: Bearer $TOKEN"

# Cadastrar programa customizado
curl -X POST "http://localhost:5263/microondas/cadastrar" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Café",
    "alimento": "Café requentado", 
    "tempoSegundos": 90,
    "potencia": 6,
    "stringPersonalizada": "☕",
    "instrucoes": "Mexer após aquecimento"
  }'
```

### 3. Operações de Aquecimento
```bash
# Usar programa pré-definido (Pipoca = ID 1)
curl -X POST "http://localhost:5263/microondas/iniciar" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"id": 1}'

# Aquecimento manual
curl -X POST "http://localhost:5263/microondas/iniciar" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"tempoSegundos": 45, "potencia": 7}'

# Pausar aquecimento
curl -X POST "http://localhost:5263/microondas/pausar" \
  -H "Authorization: Bearer $TOKEN"

# Continuar aquecimento
curl -X POST "http://localhost:5263/microondas/continuar" \
  -H "Authorization: Bearer $TOKEN"
```

---

Este projeto demonstra boas práticas de desenvolvimento de APIs REST com .NET 8, incluindo autenticação JWT, tratamento de erros, logging, validação de regras de negócio e documentação automática.

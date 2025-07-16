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

- `POST /auth/register` — Registra novo usuário (nome, username, senha)
- `POST /auth/login` — Autentica usuário e retorna token JWT
- `GET /auth/status` — Verifica status de autenticação

### Programas (`/microondas`)

- `GET /microondas` — Lista todos os programas disponíveis
- `POST /microondas/cadastrar` — Cadastra programa customizado
- `PUT /microondas/editar/{id}` — Edita programa customizado existente

### Operações de Aquecimento

- `POST /microondas/iniciar/{nome}` — Inicia aquecimento por programa pré-definido
- `POST /microondas/inicio-rapido` — Início rápido (30s, potência 10)
- `POST /microondas/pausar` — Pausa aquecimento (ou cancela se já pausado)
- `POST /microondas/continuar` — Retoma aquecimento pausado

## Programas Pré-definidos

| Programa | Alimento | Tempo | Potência | Caractere | Instruções Especiais |
|----------|----------|-------|----------|-----------|---------------------|
| **Pipoca** | Pipoca de microondas | 3 min | 7 | `*` | Observar intervalos entre estouros |
| **Leite** | Leite | 5 min | 5 | `#` | Cuidado com choque térmico |
| **Carnes de boi** | Carne em pedaços | 14 min | 4 | `~` | Virar na metade do processo |
| **Frango** | Frango (qualquer corte) | 8 min | 7 | `@` | Virar na metade do processo |
| **Feijão** | Feijão congelado | 8 min | 9 | `+` | Manter destampado |

## Regras de Negócio

- **Tempo**: Entre 1 e 120 segundos
- **Potência**: Entre 1 e 10
- **Caractere personalizado**: Único, não pode ser "." (reservado para início rápido)
- **Autenticação**: Obrigatória para todas as operações
- **Estado do microondas**: Controlado para permitir pausa/continuação

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

## Fluxo de Uso

1. **Registrar usuário**: `POST /auth/register`
2. **Fazer login**: `POST /auth/login` → receber token JWT
3. **Incluir token**: Adicionar `Authorization: Bearer <token>` nos headers
4. **Usar microondas**: Listar programas, cadastrar novos, iniciar aquecimento, etc.

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

---

Este projeto demonstra boas práticas de desenvolvimento de APIs REST com .NET 8, incluindo autenticação JWT, tratamento de erros, logging, validação de regras de negócio e documentação automática.

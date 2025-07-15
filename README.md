# MicroondasApp

Projeto ASP.NET 8 para simulação de funcionamento de um microondas, com programas pré-definidos, cadastro de programas customizados, controle de aquecimento, pausa, continuação, cancelamento e documentação automática via Swagger.

## Visão Geral

O MicroondasApp foi desenvolvido para simular as principais funções de um microondas moderno, incluindo:

- Programas de aquecimento pré-definidos e customizados
- Controle de tempo, potência e caractere de aquecimento
- Persistência de programas customizados em arquivo JSON
- Tratamento global de exceções e logging
- API documentada automaticamente via Swagger

## Estrutura do Projeto

- **Controllers/MicroondasController.cs**: Controller principal da API, responsável pelos endpoints de programas, aquecimento, pausa, continuação e cadastro de programas customizados.
- **Application/MicroondasService.cs**: Serviço central que executa a lógica de aquecimento, validações e manipulação dos programas.
- **Domain/Aquecimento.cs**: Classe que representa o aquecimento, com regras de negócio para tempo e potência.
- **Domain/RegrasDeNegocioException.cs**: Exception específica para regras de negócio do micro-ondas.
- **programas_customizados.json**: Arquivo de persistência dos programas customizados.
- **Program.cs**: Configuração do pipeline, middleware de tratamento de exceções, Swagger e inicialização da aplicação.

## Escolhas Técnicas

- **.NET 8 e C# 12**: Utilização das versões mais recentes para melhor performance e recursos modernos.
- **Middleware de Exception**: Tratamento global de erros, resposta padronizada e logging detalhado em arquivo para facilitar manutenção e auditoria.
- **Persistência em JSON**: Simples, eficiente e suficiente para o escopo do projeto, sem necessidade de banco de dados.
- **Swagger**: Documentação automática e interativa, facilitando testes e integração.
- **Validação de regras de negócio**: Centralizada na classe de serviço e exception específica, garantindo consistência.

## Endpoints da API

### Programas

- `GET /microondas` — Lista todos os programas (pré-definidos e customizados, customizados marcados com `Customizado: true`).
- `POST /microondas/cadastrar` — Cadastra um programa customizado. Campos obrigatórios: nome, alimento, tempo, potência, caractere. Instruções são opcionais. Caractere não pode repetir nem ser ".".

### Aquecimento

- `POST /microondas/iniciar/{nome}` — Inicia aquecimento por nome de programa pré-definido.
- `POST /microondas/inicio-rapido` — Inicia aquecimento rápido (30s, potência 10, caractere ".").
- `POST /microondas/pausar` — Pausa o aquecimento. Se já estiver pausado, cancela.
- `POST /microondas/continuar` — Continua aquecimento pausado, mantendo tempo e potência restantes.

## Tratamento de Erros

- Respostas de erro são padronizadas em JSON.
- Erros de regras de negócio retornam status 400 e mensagem específica.
- Erros inesperados retornam status 500 e são logados em `exceptions.log` com detalhes completos.

## Como Rodar

1. Instale o .NET 8 SDK.
2. Execute o projeto pelo Visual Studio ou via terminal: ```sh dotnet run ```
3. Acesse [http://localhost:5263](http://localhost:5263) para visualizar o Swagger e testar os endpoints.

## Observações

- Programas customizados são persistidos em `programas_customizados.json` no diretório do projeto.
- O projeto está configurado para ambiente de desenvolvimento por padrão.
- O Swagger é exibido automaticamente ao iniciar o projeto.

---

Este projeto foi estruturado para ser didático, extensível e fácil de testar, com foco em boas práticas de API REST, tratamento de erros e documentação.
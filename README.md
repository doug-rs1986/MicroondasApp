# MicroondasApp

Projeto ASP.NET 8 para simula��o de funcionamento de um microondas, com programas pr�-definidos, cadastro de programas customizados, controle de aquecimento, pausa, continua��o, cancelamento e documenta��o autom�tica via Swagger.

## Vis�o Geral

O MicroondasApp foi desenvolvido para simular as principais fun��es de um microondas moderno, incluindo:

- Programas de aquecimento pr�-definidos e customizados
- Controle de tempo, pot�ncia e caractere de aquecimento
- Persist�ncia de programas customizados em arquivo JSON
- Tratamento global de exce��es e logging
- API documentada automaticamente via Swagger

## Estrutura do Projeto

- **Controllers/MicroondasController.cs**: Controller principal da API, respons�vel pelos endpoints de programas, aquecimento, pausa, continua��o e cadastro de programas customizados.
- **Application/MicroondasService.cs**: Servi�o central que executa a l�gica de aquecimento, valida��es e manipula��o dos programas.
- **Domain/Aquecimento.cs**: Classe que representa o aquecimento, com regras de neg�cio para tempo e pot�ncia.
- **Domain/RegrasDeNegocioException.cs**: Exception espec�fica para regras de neg�cio do micro-ondas.
- **programas_customizados.json**: Arquivo de persist�ncia dos programas customizados.
- **Program.cs**: Configura��o do pipeline, middleware de tratamento de exce��es, Swagger e inicializa��o da aplica��o.

## Escolhas T�cnicas

- **.NET 8 e C# 12**: Utiliza��o das vers�es mais recentes para melhor performance e recursos modernos.
- **Middleware de Exception**: Tratamento global de erros, resposta padronizada e logging detalhado em arquivo para facilitar manuten��o e auditoria.
- **Persist�ncia em JSON**: Simples, eficiente e suficiente para o escopo do projeto, sem necessidade de banco de dados.
- **Swagger**: Documenta��o autom�tica e interativa, facilitando testes e integra��o.
- **Valida��o de regras de neg�cio**: Centralizada na classe de servi�o e exception espec�fica, garantindo consist�ncia.

## Endpoints da API

### Programas

- `GET /microondas` � Lista todos os programas (pr�-definidos e customizados, customizados marcados com `Customizado: true`).
- `POST /microondas/cadastrar` � Cadastra um programa customizado. Campos obrigat�rios: nome, alimento, tempo, pot�ncia, caractere. Instru��es s�o opcionais. Caractere n�o pode repetir nem ser ".".

### Aquecimento

- `POST /microondas/iniciar/{nome}` � Inicia aquecimento por nome de programa pr�-definido.
- `POST /microondas/inicio-rapido` � Inicia aquecimento r�pido (30s, pot�ncia 10, caractere ".").
- `POST /microondas/pausar` � Pausa o aquecimento. Se j� estiver pausado, cancela.
- `POST /microondas/continuar` � Continua aquecimento pausado, mantendo tempo e pot�ncia restantes.

## Tratamento de Erros

- Respostas de erro s�o padronizadas em JSON.
- Erros de regras de neg�cio retornam status 400 e mensagem espec�fica.
- Erros inesperados retornam status 500 e s�o logados em `exceptions.log` com detalhes completos.

## Como Rodar

1. Instale o .NET 8 SDK.
2. Execute o projeto pelo Visual Studio ou via terminal: ```sh dotnet run ```
3. Acesse [http://localhost:5263](http://localhost:5263) para visualizar o Swagger e testar os endpoints.

## Observa��es

- Programas customizados s�o persistidos em `programas_customizados.json` no diret�rio do projeto.
- O projeto est� configurado para ambiente de desenvolvimento por padr�o.
- O Swagger � exibido automaticamente ao iniciar o projeto.

---

Este projeto foi estruturado para ser did�tico, extens�vel e f�cil de testar, com foco em boas pr�ticas de API REST, tratamento de erros e documenta��o.
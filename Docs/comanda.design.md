# Design do Sistema - Comanda

## Introdução

**Propósito**

Este documento descreve a arquitetura, os padrões de design e as decisões técnicas adotadas no projeto **Comanda**. Ele visa fornecer uma visão clara do design do sistema e suas motivações.

**Escopo**

O **Comanda** é um sistema monolítico construído em **.NET 8** para gerenciar pedidos online em lanchonetes. Ele utiliza padrões modernos como **Repository Pattern**, **CQS** (Command Query Separation) e **Mediator**, além de seguir princípios como **SRP** (Single Responsibility Principle), **DIP** (Dependency Inversion Principle) e **DI** (Dependency Injection).

## Arquitetura Geral

O **Comanda** é uma aplicação monolítica. A escolha por um design monolítico foi motivada pela simplicidade inicial.

## Padrões e Tecnologias

### Injeção de Dependência (DI)

A Injeção de Dependência (DI) é usada no projeto para desacoplar as classes e suas dependências, o que facilita testes, manutenção e flexibilidade. Ao utilizar DI, conseguimos substituir facilmente implementações concretas, como repositórios, por mocks ou outras implementações, o que melhora a testabilidade e a modularidade do código.


### Repository Pattern
No projeto, utilizamos o Repository Pattern não apenas para isolar a dependência do banco de dados (EF Core), mas também para facilitar os testes unitários. A principal motivação é garantir que o repositório possa ser tratado como uma unidade isolada, sem depender diretamente do banco de dados durante os testes. Esse padrão permite que possamos simular ou "mockar" o repositório em nossos testes, sem que a implementação real do acesso ao banco de dados seja executada, o que torna os testes mais rápidos e independentes de infraestrutura externa.

Embora o EF Core já forneça uma implementação de repositório por meio do DbContext, ao abstrairmos essa funcionalidade com o padrão Repository, conseguimos definir operações específicas que nosso domínio precisa, além de permitir que o repositório seja facilmente substituído por uma implementação falsa ou mockada durante os testes, como quando utilizamos uma implementação em memória para testar casos específicos sem afetar os dados reais.

### CQS + Mediator

O sistema segue **CQS**, separando comandos e consultas em **Payloads**, organizados por contexto. Para cada Payload Request, há um **handler** que processa a solicitação e retorna uma resposta.

```text
Payloads/
├── Requests/
│   ├── IdentityPayloads/
│   │   └── AuthenticationRequest.cs
├── Responses/
│   ├── IdentityPayloads/
│   │   └── AuthenticationResponse.cs
```

### Dependency Inversion Principle (DIP)

Ao depender de abstrações em vez de implementações concretas, é possível substituir serviços reais por mocks ou stubs durante os testes.

**Exemplo de DIP com Serviço de E-mail**  
O sistema usa **Brevo** para envio de e-mails em produção, mas durante os testes, uma implementação simulada é usada para evitar gastos.

### Uso de Extension Methods para Organização e Configuração

O projeto utiliza **Extension Methods** para organizar e estruturar a configuração de serviços e repositórios de maneira modular e coesa. Em vez de registrar todos os serviços em um único arquivo como o `Program.cs`, foram criados métodos de extensão específicos para cada grupo de serviços ou componentes. Isso facilita a manutenção e a expansão do projeto, promovendo uma estrutura organizada e de fácil entendimento.

#### Exemplo de `ApplicationServicesExtension`

Em `Source/Extensions/ApplicationServicesExtension.cs`, são registrados os serviços específicos da aplicação, como `IAddressService`, `IAuthenticationService` e `ICheckoutManager`. Essa abordagem centraliza o registro de serviços, permitindo que a modificação ou adição de novos serviços seja feita em um único local, sem a necessidade de alterações em múltiplos arquivos.

#### Exemplo de `DataPersistenceExtension`

Já em `Source/Extensions/DataPersistenceExtension.cs`, são configurados os repositórios e a persistência de dados utilizando o Entity Framework Core (EF Core). O objetivo dessa classe de extensão é registrar os repositórios específicos do contexto de dados, como `IAddressRepository`, `ICartRepository` e `ICustomerRepository`, além de configurar o `DbContext`.
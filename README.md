# Comanda API

**Versão:** `v1.0.0-alpha1`

Comanda é uma Web API desenvolvida para facilitar a gestão de pedidos online em lanchonetes, como aquelas que vendem hambúrgueres, pizzas, entre outros. A API fornece uma interface simples e intuitiva para gerenciar pedidos, personalizar produtos, gerenciar perfis de clientes, e muito mais.

## Funcionalidades

### 1. **Gerenciamento de Pedidos**
- **Realização de Pedidos:** Clientes podem realizar pedidos online, customizando seus lanches conforme suas preferências. A personalização permite adicionar ou remover ingredientes, o que influencia o preço final.

- **Carrinho do Cliente:** Endpoints para gerenciar o carrinho do cliente, representando um estado temporário do pedido antes da confirmação.

- **Gestão de Pedidos (Admin):** Administradores podem visualizar todos os pedidos, alterar o status de um pedido, cancelar pedidos (com suporte a reembolso via Stripe), e acessar detalhes completos dos pedidos.

### 2. **Customização de Produtos**
- **Personalização:** Clientes podem customizar seus pedidos, como adicionar queijo extra ou remover uma carne, com preços ajustados conforme os adicionais escolhidos.

- **Gerenciamento de Produtos e Categorias:** Endpoints para criar, atualizar, e excluir produtos e categorias. Cada produto pode ser associado a uma categoria específica e incluir imagens e ingredientes.

### 3. **Gerenciamento de Adicionais**
- **Adicionais por Categoria:** Cada categoria de produto, como "Lanches", pode ter seus próprios adicionais permitidos, como queijo, carne extra, etc.

- **Gestão de Adicionais:** Endpoints para gerenciar os adicionais dos produtos.

### 4. **Perfis de Clientes**
- **Endereços:** Clientes podem registrar, atualizar e deletar endereços. Isso facilita o processo de pedido, permitindo que o cliente selecione um endereço pré-cadastrado, evitando a necessidade de reescrevê-lo a cada pedido. Um cliente pode ter múltiplos endereços.

- **Histórico e Detalhes de Pedidos:** Endpoints para obter todos os pedidos atuais do cliente que ainda não foram concluídos, acessar detalhes específicos de um pedido e visualizar o histórico completo de pedidos.

### 5. **Gerenciamento de Pedidos (Admin)**
- **Visualização e Gestão de Pedidos:** Administradores têm acesso a todos os pedidos, com a capacidade de ver detalhes, alterar o status, e cancelar pedidos.

- **Notificações em Tempo Real:** Notificações são enviadas para a cozinha em tempo real quando um novo pedido é feito, garantindo que a equipe de cozinha esteja imediatamente ciente dos novos pedidos e das personalizações feitas pelos clientes.

- **Cancelamento de Pedidos:** Tanto clientes quanto administradores podem solicitar o cancelamento de um pedido. Se for o administrador, o status do pedido será "Cancelado Pelo Sistema"; se for o cliente, o status será "Cancelado pelo Cliente". O reembolso é processado via Stripe.

### 6. **Gerenciamento de Ingredientes**
- **Informações Básicas:** Endpoints para gerenciar os ingredientes dos produtos, permitindo a associação de ingredientes a produtos específicos, sem gerenciar o estoque.

### 7. **Configurações da API/Sistema**
- **Configurações Personalizadas:** Endpoints para definir configurações como aceitação automática de pedidos, limite máximo de pedidos que podem ser aceitos automaticamente, taxa de entrega e estimativa do tempo de entrega.

### 8. **Pagamento e Checkout Online**
- **Pagamentos:** Atualmente, a API suporta pagamentos com cartão de crédito através do Stripe, com planos para implementar pagamentos via PIX no futuro.

- **Reembolso:** Suporte a reembolsos automatizados via Stripe em caso de cancelamento de pedidos.

### 9. **Autenticação e Autorização**
- **Autenticação:** Implementada usando JWT (JSON Web Tokens), permitindo que os clientes façam login e mantenham sessões seguras.

- **Autorização:** Endpoints protegidos que exigem autenticação para garantir que apenas usuários autorizados possam acessar certas funcionalidades, como gerenciar pedidos ou acessar informações de perfil.


## Tecnologias Utilizadas

- **Linguagem:** C#
- **Framework:** ASP.NET Core
- **Banco de Dados:** SQL Server (usando Entity Framework Core)
- **Integrações de Pagamento:** Stripe

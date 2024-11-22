# Comanda API

**Versão Atual:** `v1.0.0-beta-1`  
**Status:** Beta (possível presença de bugs; melhorias em andamento 🚧)

Bem-vindo ao **Comanda API**, uma solução poderosa e flexível para gerenciar pedidos online, perfeita para lanchonetes, restaurantes e estabelecimentos que desejam oferecer uma experiência moderna e eficiente aos seus clientes. Comanda é uma API desenvolvida para simplificar e automatizar desde o registro de clientes até o checkout online com pagamentos integrados.

> 🚀 Esta versão representa um marco significativo, trazendo uma API muito mais funcional e próxima do uso em produção.


## ✨ **Destaques da v1.0.0-beta-1**

### 🚀 **Novidades**

- **Recomendações Personalizadas**: Implementação de um sistema baseado em inteligência artificial para sugerir produtos com base no histórico de pedidos.
- **Melhorias no Fluxo de Checkout**: Redirecionamento para a aplicação frontend após o pagamento ou cancelamento no **Stripe**.

- **Gerenciamento de Perfis**:
    - **Obter e Atualizar Informações**: Endpoints para visualizar e editar dados do perfil do cliente.
    - **Recuperação de Senha**: Implementada uma funcionalidade de redefinição de senha segura para casos de esquecimento.
    - **Validação de Registro**: Agora, o sistema impede a criação de duplicatas, verificando se já existe um usuário registrado com o mesmo e-mail.


### 🛠️ **Melhorias**

- **Correções Críticas**: Resolvidos problemas severos que tornavam a API inutilizável em muitos casos.
    - **#5**: Endereços registrados corretamente, mas não associados ao cliente.
    - **#6**: Carrinho não era limpo após a conclusão do checkout e pagamento.
    - **#7**: O total do pedido exibia "0" após o checkout bem-sucedido.
    - **#9**: `NullReferenceException` ao recuperar pedidos atuais.
    - **#10**: Detalhes inconsistentes de pedidos retornados em `/api/profile/orders/{orderId}`.
    - **#11**: Dados inconsistentes no histórico de pedidos, com total exibindo "0".
    - **#12**: Sistema de recomendações de pedidos afirmava incorretamente que nenhum pedido havia sido feito.
    - **#13**: Vulnerabilidades em endpoints de incremento e decremento de itens no carrinho.
    - **#14**: Testes instáveis com comportamento não determinístico corrigidos.

- **IDs no Carrinho**: Os IDs agora refletem corretamente os itens do carrinho, eliminando confusões com IDs de produtos.

- **Cobertura de Testes**: A base de código agora conta com 242 testes no total, abrangendo testes unitários, de integração e end-to-end, garantindo estabilidade, confiabilidade e prevenindo regressões futuras.

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

### **7. Configurações Personalizáveis**
- **Administração Dinâmica**:
    - Aceitação automática de pedidos.
    - Limites para aceitação automática.
    - Taxas de entrega e estimativas de tempo configuráveis.

### 8. **Pagamento e Checkout Online**
- **Pagamentos:** Atualmente, a API suporta pagamentos com cartão de crédito através do Stripe, com planos para implementar pagamentos via PIX no futuro.

- **Reembolso:** Suporte a reembolsos automatizados via Stripe em caso de cancelamento de pedidos.

### 9. **Autenticação e Autorização**
- **Autenticação:** Implementada usando JWT (JSON Web Tokens), permitindo que os clientes façam login e mantenham sessões seguras.

- **Autorização:** Endpoints protegidos que exigem autenticação para garantir que apenas usuários autorizados possam acessar certas funcionalidades, como gerenciar pedidos ou acessar informações de perfil.

### **10 . Recomendações Personalizadas**
- **Sistema Inteligente**: Sugestões de produtos baseadas em inteligência artificial, considerando o histórico de pedidos do cliente.


## Tecnologias Utilizadas

- **Linguagem:** C#
- **Framework:** ASP.NET Core
- **Banco de Dados:** SQL Server (usando Entity Framework Core)
- **Integrações de Pagamento:** Stripe


## 🐞 **Feedback e Relatório de Bugs**

Encontrou um problema? Nos ajude a melhorar!

- Abra uma issue no repositório do GitHub.
- Descreva o problema e os passos para reproduzi-lo.
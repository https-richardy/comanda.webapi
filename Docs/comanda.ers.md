## Especificação de Requisitos de Software (ERS) - Comanda

### 1. Introdução

**Propósito**: O sistema Comanda tem como objetivo oferecer uma plataforma para gerenciar pedidos online em lanchonetes, permitindo personalização de produtos, gerenciamento de carrinho, autenticação segura via JWT, integração de pagamentos online, e notificações em tempo real para administradores.

**Escopo**: O projeto abrange a construção de uma Web API para gerenciar pedidos de lanches, focando em funcionalidades como cadastro e gerenciamento de produtos, categorias, ingredientes e adicionais, personalização de lanches, e gerenciamento de pedidos e configurações de sistema. O sistema também inclui suporte para pagamentos online e notificações para garantir um fluxo de pedidos eficiente.

---

### 2. Descrição Geral do Sistema

**Visão geral do sistema**: O Comanda é um sistema de pedidos online para lanchonetes, oferecendo tanto uma interface administrativa para gerenciamento de produtos e pedidos quanto uma interface para clientes personalizarem e realizarem pedidos de forma simples. O sistema oferece autenticação, controle de permissões (clientes x administradores), e gestão de pedidos com integração de pagamentos por cartão de crédito.

**Premissas e Dependências**:

- Integração com Stripe para processamento de pagamentos.
- Notificações em tempo real dependem de WebSockets (Especificamente SignalR).

## Requisitos Funcionais

### **RF001: Cadastro de Adicional**

- **Descrição**: O sistema deve possibilitar o cadastro de novos adicionais, permitindo que o administrador defina características específicas, como o nome do adicional e a categoria à qual ele pertence. O cadastro de adicionais é fundamental para a personalização de produtos no sistema, possibilitando que os usuários selecionem opções extras conforme suas preferências.

- **Pré-condições**:

- O usuário deve estar autenticado com uma conta de administrador.

- As categorias já devem estar previamente cadastradas no sistema.

- **Entradas**:
    - Nome do adicional
    - Categoria do adicional

- **Saídas**:
    - Confirmação de cadastro

- **Regras de Negócio**:
- **RN001 - Permissão de Cadastro**: Apenas usuários com perfil de administrador podem cadastrar novos adicionais no sistema. Caso um usuário sem as permissões necessárias tente realizar essa operação, o sistema deve negar o acesso e exibir uma mensagem de erro informando a falta de permissão.

    - **RN002 - Nome Único**: O nome do adicional deve ser único no sistema. Antes de concluir o cadastro, o sistema deve verificar se o nome informado já está em uso. Caso o nome já exista, o sistema deve exibir uma mensagem de erro e solicitar a alteração do nome.

    - **RN003 - Categoria Válida**: O adicional deve estar associado a uma categoria previamente cadastrada no sistema. Caso o administrador tente associar o adicional a uma categoria inexistente ou inválida, o sistema deve impedir o cadastro e exibir uma mensagem de erro.

    - **RN004 - Validação de Campos Obrigatórios**: O nome do adicional e a categoria são campos obrigatórios. O sistema deve validar a presença desses campos antes de concluir o cadastro. Caso algum campo obrigatório esteja em branco, o sistema deve exibir uma mensagem de erro e impedir o cadastro até que todos os campos obrigatórios sejam preenchidos.

    - **RN005 - Limite de Caracteres no Nome**: O nome do adicional deve ter entre 3 e 50 caracteres. Caso o nome informado não atenda a esse critério, o sistema deve exibir uma mensagem de erro.

### **RF002: Atualização de Adicional**

- **Descrição**: O sistema deve permitir a atualização dos dados de um adicional existente, permitindo que administradores modifiquem tanto o nome quanto a categoria de um adicional previamente cadastrado.

- **Pré-condições**:
    - O usuário deve estar autenticado com uma conta de administrador.
    - O adicional deve estar previamente cadastrado no sistema.

- **Entradas**:
    - ID do adicional
    - Nome atualizado do adicional
    - Categoria atualizada do adicional

- **Saídas**:
    - Confirmação de que o adicional foi atualizado com sucesso.
    - Mensagens de erro em caso de falhas no processo.

- **Regras de Negócio**:
    - **RN001 - Permissão de Atualização**: Apenas usuários com perfil de administrador podem atualizar os adicionais no sistema.

    - **RN002 - Validação de Existência**: O sistema deve validar se o ID fornecido corresponde a um adicional existente. Caso não exista, uma mensagem de erro deve ser exibida.

    - **RN003 - Nome Único**: O nome atualizado do adicional deve ser único no sistema. O sistema deve verificar se o novo nome já está em uso e, se estiver, solicitar a alteração.

    - **RN004 - Categoria Válida**: A nova categoria do adicional deve ser uma categoria válida e já cadastrada no sistema. Caso contrário, o sistema deve exibir uma mensagem de erro.

    - **RN005 - Validação de Campos Obrigatórios**: O ID do adicional, o novo nome e a nova categoria são campos obrigatórios e devem ser validados pelo sistema.

### **RF003: Exclusão de Adicional**

- **Descrição**: O sistema deve permitir a exclusão de adicionais, removendo-os permanentemente do catálogo, desde que atendam às condições de exclusão.

- **Pré-condições**:
    - O usuário deve estar autenticado com uma conta de administrador.
    - O adicional deve estar previamente cadastrado no sistema.

- **Entradas**:
    - ID do adicional a ser excluído

- **Saídas**:
    - Confirmação de que o adicional foi excluído com sucesso.
    - Mensagem de erro caso a exclusão não possa ser realizada.

- **Regras de Negócio**:
    - **RN001 - Permissão de Exclusão**: Apenas usuários com perfil de administrador podem excluir adicionais no sistema.

    - **RN002 - Validação de Existência**: O sistema deve validar se o ID fornecido corresponde a um adicional existente no sistema. Caso contrário, uma mensagem de erro deve ser exibida.

    - **RN003 - Associação com Produtos/Pedidos**: O sistema deve verificar se o adicional está associado a algum produto ou pedido em andamento. Se estiver, a exclusão deve ser bloqueada, e uma mensagem de erro deve ser exibida.

### **RF004: Cadastro de Ingrediente**

- **Descrição**: O sistema deve permitir que administradores cadastrem novos ingredientes, inserindo apenas o nome do ingrediente. Essa funcionalidade é crucial para a gestão do cardápio, pois permite que novos itens sejam adicionados ao sistema, ampliando as opções de personalização para os clientes.

- **Pré-condições**:
    - O usuário deve estar autenticado como administrador.
    - O nome do ingrediente deve ser único e não vazio.

- **Entradas**:
    - Nome do ingrediente.

- **Saídas**:
    - Confirmação de que o ingrediente foi cadastrado com sucesso.
    - Mensagem de erro em caso de falhas no processo.

- **Regras de Negócio**:
    - **RN001 - Permissão de Cadastro**: Apenas usuários com perfil de administrador podem cadastrar novos ingredientes no sistema.

    - **RN002 - Nome Único**: O nome do ingrediente deve ser único. O sistema deve verificar se o nome informado já está em uso. Caso o nome já exista, o sistema deve exibir uma mensagem de erro e solicitar a alteração do nome.

    - **RN003 - Validação de Campos Obrigatórios**: O nome do ingrediente é um campo obrigatório. O sistema deve validar a presença desse campo antes de concluir o cadastro. Caso o campo esteja em branco, o sistema deve exibir uma mensagem de erro e impedir o cadastro.

    - **RN004 - Limite de Caracteres no Nome**: O nome do ingrediente deve ter entre 3 e 50 caracteres. Caso o nome informado não atenda a esse critério, o sistema deve exibir uma mensagem de erro.

### **RF005: Atualização de Ingrediente**

- **Descrição**: O sistema deve permitir a atualização dos dados de um ingrediente existente, permitindo que o administrador modifique apenas o nome do ingrediente.

- **Pré-condições**:
    - O usuário deve estar autenticado como administrador.
    - O ingrediente deve estar previamente cadastrado no sistema.

- **Entradas**:
    - ID do ingrediente a ser atualizado.
    - Nome atualizado do ingrediente.

- **Saídas**:
    - Confirmação de que o ingrediente foi atualizado com sucesso.
    - Mensagens de erro em caso de falhas no processo.

- **Regras de Negócio**:
    - **RN001 - Permissão de Atualização**: Apenas usuários com perfil de administrador podem atualizar os ingredientes no sistema.
    - **RN002 - Validação de Existência**: O sistema deve validar se o ID fornecido corresponde a um ingrediente existente. Caso não exista, uma mensagem de erro deve ser exibida.
    - **RN003 - Nome Único**: O nome atualizado do ingrediente deve ser único no sistema. O sistema deve verificar se o novo nome já está em uso e, se estiver, solicitar a alteração.
    - **RN004 - Validação de Campos Obrigatórios**: O nome atualizado do ingrediente é um campo obrigatório. O sistema deve validar a presença desse campo antes de concluir a atualização.
    - **RN005 - Limite de Caracteres no Nome**: O nome atualizado do ingrediente deve conter entre 3 e 50 caracteres. Caso o novo nome informado não atenda a esse critério, o sistema deve exibir uma mensagem de erro.

### **RF006: Exclusão de Ingrediente**

- **Descrição**: O sistema deve permitir a exclusão de ingredientes, removendo-os permanentemente do catálogo, desde que não estejam associados a produtos ou pedidos em andamento.

- **Pré-condições**:
    - O usuário deve estar autenticado como administrador.
    - O ingrediente deve estar previamente cadastrado no sistema.

- **Entradas**:
    - ID do ingrediente a ser excluído.

- **Saídas**:
    - Confirmação de que o ingrediente foi excluído com sucesso.
    - Mensagens de erro em caso de falhas no processo.

- **Regras de Negócio**:
    - **RN001 - Permissão de Exclusão**: Apenas usuários com perfil de administrador podem excluir ingredientes no sistema.

    - **RN002 - Validação de Existência**: O sistema deve validar se o ID fornecido corresponde a um ingrediente existente no sistema. Caso contrário, uma mensagem de erro deve ser exibida.

    - **RN003 - Associação com Produtos/Pedidos**: O sistema deve verificar se o ingrediente está associado a algum produto ou pedido em andamento. Se estiver, a exclusão deve ser bloqueada, e uma mensagem de erro deve ser exibida, orientando o administrador a desvincular o ingrediente antes de realizar a exclusão.

### **RF007: Listar Ingredientes**

- **Descrição**: O sistema deve permitir que apenas administradores visualizem a lista completa de ingredientes disponíveis. Esta funcionalidade é crucial para que a administração possa revisar e gerenciar os ingredientes cadastrados no sistema.

- **Pré-condições**:
    - O usuário deve estar autenticado e ter perfil de **administrador**.

- **Entradas**:
    - Nenhuma entrada necessária.

- **Saídas**:
    - Uma lista contendo os ingredientes cadastrados, com seus respectivos nomes e IDs.

- **Regras de Negócio**:
    - **RN001 - Acesso Restrito**: Apenas usuários com perfil de **administrador** podem acessar a lista de ingredientes. O sistema deve garantir que apenas usuários autorizados tenham permissão para visualizar esses dados.

    - **RN002 - Listagem Completa**: O sistema deve retornar todos os ingredientes cadastrados, sem filtros, incluindo nome e ID de cada ingrediente.

### **RF008: Buscar Todas as Categorias**
- **Descrição**: O sistema deve permitir que tanto administradores quanto clientes visualizem a lista de todas as categorias disponíveis, como "Comida", "Bebida", entre outras. Essa funcionalidade é essencial para que os usuários possam conhecer as opções disponíveis e facilitar a navegação e escolha de itens no sistema.

- **Entradas**:
    - Nenhuma entrada necessária.

- **Saídas**:
    - Uma lista contendo as categorias cadastradas, com seus respectivos nomes e IDs.

- **Regras de Negócio**:
    - **RN001 - Acesso Universal**: Tanto clientes quanto administradores podem acessar a lista de categorias.
    - **RN002 - Listagem Completa**: O sistema deve exibir todas as categorias cadastradas, sem filtros, e garantir que novas categorias ou alterações sejam imediatamente refletidas na listagem.

### **RF009: Buscar Categoria por ID**
- **Descrição**: O sistema deve permitir que tanto administradores quanto clientes busquem uma categoria específica informando o seu ID.

- **Entradas**:
    - ID da categoria a ser consultada.

- **Saídas**:
    - Nome e ID da categoria correspondente.

- **Regras de Negócio**:
    - **RN001 - Acesso Universal**: Tanto clientes quanto administradores podem realizar essa consulta.
    - **RN002 - Validação do ID**: O sistema deve validar se o ID fornecido corresponde a uma categoria existente. Caso contrário, deve exibir uma mensagem de erro.

### **RF010: Criar Nova Categoria**
- **Descrição**: O sistema deve permitir que apenas administradores possam criar novas categorias, definindo um nome para elas. Esta funcionalidade é crucial para a administração do sistema, garantindo flexibilidade no gerenciamento de categorias.

- **Pré-condições**:
    - O usuário deve estar autenticado e ter perfil de **administrador**.

- **Entradas**:
    - Nome da nova categoria.

- **Saídas**:
    - Confirmação de cadastro da nova categoria.

- **Regras de Negócio**:
    - **RN001 - Permissão de Criação**: Apenas administradores podem criar novas categorias. Caso um usuário sem permissão tente realizar essa operação, o sistema deve negar o acesso e exibir uma mensagem de erro.

    - **RN002 - Nome Único**: O nome da nova categoria deve ser único no sistema. O sistema deve validar se o nome informado já está em uso e, em caso afirmativo, exibir uma mensagem de erro.

    - **RN003 - Limite de Caracteres**: O nome da categoria deve ter entre 3 e 50 caracteres. Caso o nome não atenda a esse critério, o sistema deve exibir uma mensagem de erro.

### **RF011: Atualizar Categoria**
- **Descrição**: O sistema deve permitir que administradores atualizem o nome de uma categoria existente, informando o ID da categoria e o novo nome desejado. Essa funcionalidade permite a correção ou ajuste de categorias sem a necessidade de exclusão e recriação.

- **Pré-condições**:
    - O usuário deve estar autenticado e ter perfil de **administrador**.

- **Entradas**:
    - ID da categoria.
    - Novo nome da categoria.

- **Saídas**:
    - Confirmação da atualização da categoria.

- **Regras de Negócio**:
    - **RN001 - Permissão de Atualização**: Apenas administradores podem atualizar uma categoria. Caso um usuário sem permissão tente realizar essa operação, o sistema deve negar o acesso e exibir uma mensagem de erro.

    - **RN002 - Validação do ID**: O sistema deve validar se o ID fornecido corresponde a uma categoria existente. Caso contrário, deve exibir uma mensagem de erro.

    - **RN003 - Nome Único**: O novo nome da categoria deve ser único. O sistema deve validar se o novo nome já está em uso. Caso o nome já exista, o sistema deve exibir uma mensagem de erro.

    - **RN004 - Limite de Caracteres**: O novo nome da categoria deve ter entre 3 e 50 caracteres. Caso não atenda a esse critério, o sistema deve exibir uma mensagem de erro.

### **RF012: Deletar Categoria**
- **Descrição**: O sistema deve permitir que administradores excluam uma categoria existente, informando o ID da categoria. A exclusão de categorias é útil quando estas não são mais relevantes ou necessárias no sistema.

- **Pré-condições**:
    - O usuário deve estar autenticado e ter perfil de **administrador**.

- **Entradas**:
    - ID da categoria a ser excluída.

- **Saídas**:
    - Confirmação de exclusão da categoria.

- **Regras de Negócio**:
    - **RN001 - Permissão de Exclusão**: Apenas administradores podem excluir categorias. Caso um usuário sem permissão tente realizar essa operação, o sistema deve negar o acesso e exibir uma mensagem de erro.

    - **RN002 - Validação do ID**: O sistema deve validar se o ID fornecido corresponde a uma categoria existente. Caso contrário, deve exibir uma mensagem de erro.

    - **RN003 - Associação com Produtos**: O sistema deve verificar se a categoria está associada a algum produto. Caso esteja, a exclusão não deve ser permitida, e o sistema deve exibir uma mensagem de erro solicitando que os produtos sejam atualizados antes da exclusão.

### **RF013: Cadastrar Produto**

- **Descrição**: O sistema deve permitir que administradores possam cadastrar novos produtos, inserindo informações básicas como nome, preço, descrição, categoria e ingredientes. A imagem do produto será enviada posteriormente em uma operação separada.

- **Pré-condições**:
    - O usuário deve estar autenticado e ter perfil de **administrador**.
    - As categorias e ingredientes devem estar previamente cadastrados no sistema.

- **Entradas**:
    - Nome do produto.
    - Preço do produto.
    - Descrição do produto.
    - ID da categoria a qual o produto pertence.
    - IDs dos ingredientes que compõem o produto.

- **Saídas**:
    - Confirmação de cadastro do novo produto.

- **Regras de Negócio**:
    - **RN001 - Permissão de Criação**: Apenas administradores podem cadastrar produtos. Caso um usuário sem permissão tente realizar essa operação, o sistema deve negar o acesso e exibir uma mensagem de erro.

    - **RN002 - Validação de Campos Obrigatórios**: Nome, preço, descrição, categoria e ingredientes são obrigatórios. O sistema deve validar se todos os campos foram preenchidos corretamente.

    - **RN003 - Preço Positivo**: O preço do produto deve ser um valor positivo e maior que zero.

    - **RN004 - Limites de Caracteres**: O nome deve ter entre 3 e 100 caracteres, e a descrição pode ter no máximo 500 caracteres.

    - **RN005 - Verificação de Existência**: A categoria e os ingredientes fornecidos devem existir no sistema. O sistema deve validar os IDs e, em caso de inexistência, exibir uma mensagem de erro.

### **RF014: Upload de Imagem para Produto**
- **Descrição**: O sistema deve permitir que administradores façam upload de uma imagem para um produto já cadastrado. A imagem será usada para representar o produto em exibições públicas e administrativas.

- **Pré-condições**:
    - O usuário deve estar autenticado e ter perfil de **administrador**.
    - O produto deve estar previamente cadastrado no sistema.

- **Entradas**:
    - ID do produto.
    - Arquivo de imagem (PNG ou JPG, com no máximo 10 MB).

- **Saídas**:
    - Confirmação de upload da imagem.

- **Regras de Negócio**:
    - **RN001 - Permissão de Upload**: Apenas administradores podem enviar imagens para produtos.

    - **RN002 - Tamanho do Arquivo**: O arquivo de imagem deve ter no máximo 10 MB. Caso ultrapasse esse limite, o sistema deve exibir uma mensagem de erro.

    - **RN003 - Formato de Imagem**: A imagem deve estar no formato PNG ou JPG. Caso esteja em outro formato, o sistema deve exibir uma mensagem de erro.

    - **RN004 - Validação do Produto**: O produto para o qual a imagem será enviada deve existir no sistema. Caso contrário, o sistema deve exibir uma mensagem de erro.

### **RF015: Listar Produtos com Paginação e Filtros**

- **Descrição**: O sistema deve permitir que tanto clientes quanto administradores possam listar todos os produtos disponíveis, com suporte a paginação e filtros opcionais. Os usuários podem definir o número de itens por página e aplicar filtros como título, preço mínimo e preço máximo.

- **Entradas** (opcionais):
    - `Page` (página atual), valor padrão: 1.
    - `PageSize` (número de itens por página), valor padrão: 10.
    - `Title` (filtrar por título do produto), opcional.
    - `MinPrice` (filtrar por preço mínimo), opcional.
    - `MaxPrice` (filtrar por preço máximo), opcional.

- **Saídas**:
    - Uma lista paginada de produtos contendo nome, preço, descrição, ingredientes, categoria e imagem.

- **Regras de Negócio**:
    - **RN001 - Acesso Universal**: Tanto clientes quanto administradores podem listar os produtos.

    - **RN002 - Paginação**: Por padrão, a página inicial será a 1 e o tamanho da página será de 10 produtos. O sistema deve permitir que o usuário ajuste esses valores de acordo com suas necessidades.

    - **RN003 - Filtros Opcionais**: Os parâmetros `Title`, `MinPrice` e `MaxPrice` são opcionais. Se não forem fornecidos, o sistema deve exibir todos os produtos dentro da página especificada.

    - **RN004 - Filtro por Preço**: O sistema deve permitir que os usuários filtrem os produtos com base em um intervalo de preços, verificando se o preço dos produtos está entre `MinPrice` e `MaxPrice` fornecidos.

    - **RN005 - Filtro por Título**: Se o parâmetro `Title` for fornecido, o sistema deve retornar produtos cujo nome contenha o título informado, ignorando diferenças de maiúsculas e minúsculas.

    - **RN006 - Limites de Paginação**: O `PageSize` não pode ser maior que 100 para evitar sobrecarga no sistema. Caso o valor seja maior que 100, o sistema deve aplicar um limite e exibir uma mensagem informativa.

### **RF016: Atualizar Produto**

- **Descrição**: O sistema deve permitir que administradores atualizem as informações de um produto, incluindo nome, preço, descrição, categoria e ingredientes. A imagem do produto pode ser alterada separadamente.

- **Pré-condições**:
    - O usuário deve estar autenticado e ter perfil de **administrador**.
    - O produto deve estar previamente cadastrado no sistema.

- **Entradas**:
    - ID do produto a ser atualizado.
    - Nome do produto.
    - Preço do produto.
    - Descrição do produto.
    - ID da categoria.
    - IDs dos ingredientes.

- **Saídas**:
    - Confirmação de atualização das informações do produto.

- **Regras de Negócio**:
    - **RN001 - Permissão de Atualização**: Apenas administradores podem atualizar informações de produtos.

    - **RN002 - Validação de Campos**: Nome, preço, descrição, categoria e ingredientes são obrigatórios. O sistema deve validar se todos os campos foram preenchidos corretamente.

    - **RN003 - Validação de Produto Existente**: O sistema deve verificar se o ID do produto existe. Caso contrário, deve exibir uma mensagem de erro.

    - **RN004 - Limite de Caracteres**: O nome do produto deve ter entre 3 e 100 caracteres, e a descrição no máximo 500 caracteres.

    - **RN005 - Preço Positivo**: O preço informado deve ser maior que zero.

### **RF017: Deletar Produto**
- **Descrição**: O sistema deve permitir que administradores excluam um produto, removendo-o completamente do catálogo.

- **Pré-condições**:
    - O usuário deve estar autenticado e ter perfil de **administrador**.
    - O produto deve estar previamente cadastrado no sistema.

- **Entradas**:
    - ID do produto a ser excluído.

- **Saídas**:
    - Confirmação de exclusão do produto.

- **Regras de Negócio**:
    - **RN001 - Permissão de Exclusão**: Apenas administradores podem excluir produtos.

    - **RN002 - Validação de Produto Existente**: O sistema deve verificar se o ID do produto existe. Caso contrário, deve exibir uma mensagem de erro.

    - **RN003 - Associação com Pedidos**: O sistema deve verificar se o produto está associado a algum pedido ativo. Caso esteja, a exclusão não deve ser permitida, e o sistema deve exibir uma mensagem de erro.

### **RF018: Obter Configurações do Sistema**

- **Descrição**: O sistema deve permitir que os administradores possam visualizar as configurações atuais do sistema. Essas configurações incluem opções como aceitar pedidos automaticamente, número máximo de pedidos que podem ser aceitos automaticamente, tempo de entrega estimado e taxa de entrega.

- **Pré-condições**:
    - O usuário deve estar autenticado e ter perfil de **administrador**.

- **Entradas**:
    - Nenhuma.

- **Saídas**:
    - As configurações atuais do sistema:
        - Aceitar pedidos automaticamente: booleano.
        - Número máximo da fila de pedidos que podem ser aceitos automaticamente: número inteiro.
        - Tempo de entrega estimado em minutos: número inteiro.
        - Taxa de entrega: valor decimal.

- **Regras de Negócio**:
    - **RN001 - Acesso Restrito**: Somente administradores podem visualizar as configurações do sistema. Caso um usuário sem permissão tente acessar as configurações, o sistema deve negar o acesso e exibir uma mensagem de erro.

    - **RN002 - Retorno de Todas as Configurações**: O sistema deve retornar todas as configurações disponíveis, garantindo que estejam atualizadas e correspondam ao estado atual do sistema.

    - **RN003 - Padrões de Configurações**: Caso alguma configuração não esteja definida, o sistema deve retornar um valor padrão (por exemplo: `aceitar pedidos automaticamente = false`, `número máximo de pedidos = 5`, `tempo de entrega = 30 minutos`, `taxa de entrega = 0.00`).

### **RF019: Atualizar Configurações do Sistema**
- **Descrição**: O sistema deve permitir que administradores atualizem as configurações gerais do sistema, como aceitar pedidos automaticamente, número máximo de pedidos aceitos automaticamente, tempo de entrega estimado em minutos e taxa de entrega.

- **Pré-condições**:
    - O usuário deve estar autenticado e ter perfil de **administrador**.

- **Entradas**:
    - Aceitar pedidos automaticamente (booleano).
    - Número máximo de pedidos aceitos automaticamente (número inteiro).
    - Tempo de entrega estimado em minutos (número inteiro).
    - Taxa de entrega (valor decimal).

- **Saídas**:
    - Confirmação da atualização das configurações.

- **Regras de Negócio**:
    - **RN001 - Acesso Restrito**: Somente administradores podem atualizar as configurações do sistema. Caso um usuário sem permissão tente realizar essa operação, o sistema deve negar o acesso e exibir uma mensagem de erro.

    - **RN002 - Validação de Entradas**:
        - **Aceitar pedidos automaticamente**: Deve ser um valor booleano (`true` ou `false`).
        - **Número máximo de pedidos aceitos automaticamente**: Deve ser um número inteiro positivo. O sistema deve garantir que esse número não seja inferior a 1, pois precisa haver pelo menos um pedido aceito automaticamente.
        - **Tempo de entrega estimado**: Deve ser um número inteiro maior que 0, representando os minutos que o sistema estima para entregar um pedido.
        - **Taxa de entrega**: Deve ser um valor decimal positivo. O sistema não deve permitir valores negativos ou iguais a zero.

    - **RN003 - Limite de Pedidos Aceitos Automaticamente**: O sistema deve permitir que o administrador defina um limite de pedidos que podem ser aceitos automaticamente. Caso esse limite seja atingido, novos pedidos devem entrar em um estado de "aguardando aprovação" para evitar sobrecarga da cozinha.

    - **RN004 - Aplicação Imediata das Configurações**: Qualquer alteração nas configurações deve ser aplicada imediatamente, sem a necessidade de reiniciar o sistema. As mudanças devem refletir diretamente no comportamento de aceitação de pedidos, cálculo do tempo de entrega e taxa de entrega.

### **RF020: Obter Todos os Pedidos**
- **Descrição**: O sistema deve permitir que administradores obtenham uma lista paginada de todos os pedidos registrados. As informações básicas de cada pedido serão retornadas, como nome do cliente, endereço, total, status e data.

- **Pré-condições**:
    - O usuário deve estar autenticado e ter perfil de **administrador**.

- **Entradas**:
    - **Query Parameters**:
        - `PageNumber`: Número da página para paginação (valor padrão = 1).
        - `PageSize`: Número de pedidos por página (valor padrão = 10).
        - `Status`: Filtro pelo status do pedido (enumerador, opcional).

- **Saídas**:
    - Lista paginada de pedidos contendo as seguintes informações:
        - `Id`: Identificador único do pedido.
        - `CustomerName`: Nome completo do cliente.
        - `ShippingAddress`: String com as informações de endereço (sem referência ou complemento).
        - `Total`: Valor total do pedido (decimal).
        - `Status`: Status atual do pedido (enumerador).
        - `OrderDate`: Data em que o pedido foi feito.

- **Regras de Negócio**:
    - **RN001 - Acesso Restrito**: Somente administradores podem listar os pedidos.

    - **RN002 - Filtros Opcionais**: O filtro pelo status do pedido é opcional. Se não for informado, todos os pedidos devem ser listados.

    - **RN003 - Paginação**: A lista deve ser paginada com os valores padrão de `PageNumber = 1` e `PageSize = 10`. O administrador pode ajustar esses parâmetros para ver mais ou menos pedidos por página.

    - **RN004 - Informações Resumidas do Pedido**: A lista de pedidos deve conter apenas as informações básicas, sem detalhes adicionais como referência ou complemento de endereço, que são retornados apenas ao visualizar os detalhes de um pedido específico.

### **RF021: Obter Detalhes do Pedido**

- **Descrição**: O sistema deve permitir que administradores visualizem os detalhes completos de um pedido específico. Isso é útil para a cozinha, permitindo que os preparadores acompanhem o que precisa ser feito no pedido.

- **Pré-condições**:
    - O usuário deve estar autenticado e ter perfil de **administrador**.

- **Entradas**:
    - **Path Parameter**:
        - `Id`: O identificador único do pedido.

- **Saídas**:
    - Detalhes completos do pedido:
        - `Id`: Identificador único do pedido.
        - `Customer`: Nome completo do cliente.
        - `ShippingAddress`: String com o endereço completo, incluindo referências e complementos.
        - `Total`: Valor total do pedido (decimal).
        - `Items`: Coleção de itens do pedido:
            - Para cada item:
                - `ProductTitle`: Nome do produto.
                - `Quantity`: Quantidade solicitada do produto.
                - `Additionals`: Coleção de adicionais que o cliente escolheu para o item.
                - `RemovedIngredients`: Lista de ingredientes removidos.
        - `Status`: Status atual do pedido (enumerador).
        - `OrderDate`: Data em que o pedido foi feito.

- **Regras de Negócio**:
    - **RN001 - Acesso Restrito**: Somente administradores podem visualizar os detalhes de um pedido.

    - **RN002 - Detalhamento Completo do Endereço**: Ao contrário da listagem geral de pedidos, aqui o endereço do cliente deve incluir referência e complemento.

    - **RN003 - Detalhes dos Itens do Pedido**: Cada item do pedido deve conter informações sobre os adicionais escolhidos e os ingredientes removidos, permitindo que a cozinha prepare os itens conforme solicitado pelo cliente.


### **RF022: Atualizar Status do Pedido**

- **Descrição**: O sistema deve permitir que administradores atualizem o status de um pedido. Essa ação reflete o andamento do pedido e pode ser acompanhada pelo cliente em tempo real.

- **Pré-condições**:
    - O usuário deve estar autenticado e ter perfil de **administrador**.

- **Entradas**:
    - **Path Parameter**:
        - `Id`: O identificador único do pedido.
    - `Status`: Novo status do pedido (enumerador).

- **Saídas**:
    - Confirmação da atualização do status do pedido.

- **Regras de Negócio**:
    - **RN001 - Acesso Restrito**: Somente administradores podem atualizar o status do pedido.
    - **RN002 - Status Válido**: O status informado deve ser um valor válido do enumerador de status de pedidos.
    - **RN003 - Notificação ao Cliente**: Sempre que o status do pedido for atualizado, o cliente deve ser notificado em tempo real sobre a mudança.

### **RF023: Cancelar Pedido**

- **Descrição**: O sistema deve permitir que administradores cancelem um pedido. Quando um pedido é cancelado pelo administrador, uma razão pode ser fornecida opcionalmente, e o status do pedido será automaticamente definido como "Cancelado pelo sistema", com início do processo de reembolso.

- **Pré-condições**:
    - O usuário deve estar autenticado e ter perfil de **administrador**.

- **Entradas**:
    - **Path Parameter**:
        - `Id`: O identificador único do pedido.
    - **Body** (opcional):
        - `Reason`: Razão do cancelamento (opcional).

- **Saídas**:
    - Confirmação do cancelamento e status atualizado para "Cancelado pelo sistema".

- **Regras de Negócio**:
    - **RN001 - Acesso Restrito**: Somente administradores podem cancelar um pedido.

    - **RN002 - Status Automático**: Ao cancelar um pedido, o status deve ser atualizado automaticamente para "Cancelado pelo sistema".

    - **RN003 - Reembolso Automático**: Após o cancelamento, o sistema deve iniciar automaticamente o processo de reembolso para o cliente.

    - **RN004 - Razão Opcional**: A razão do cancelamento é opcional, mas pode ser fornecida para fins de auditoria ou explicação ao cliente.

### **RF024: Notificação em Tempo Real sobre Novos Pedidos e Atualizações para a Cozinha**

- **Descrição**: O sistema deve notificar a equipe da cozinha em tempo real sempre que um novo pedido for recebido ou quando houver atualizações importantes sobre o status de um pedido.

- **Entradas**:
    - O sistema deve automaticamente gerar a notificação quando:
        - Um novo pedido for criado e estiver pronto para preparação.
        - O status de um pedido for atualizado para "Preparação em andamento" ou "Pronto para entrega".

- **Saídas**:
    - Notificação em tempo real contendo as seguintes informações:
        - `Title`: Título da notificação (ex: "Novo Pedido Recebido").
        - `Message`: Mensagem detalhada sobre o pedido (ex: "Pedido #1234 - Cliente João - Preparação pendente.").
        - `Timestamp`: Data e hora do envio da notificação.

- **Regras de Negócio**:
    - **RN001 - Notificação Automática**: Sempre que um novo pedido for recebido ou o status for atualizado para indicar o andamento da preparação, o sistema deve gerar e enviar uma notificação automaticamente à equipe da cozinha.

    - **RN002 - Conteúdo da Notificação**: A notificação deve incluir um título claro, uma mensagem com as informações básicas do pedido (como número do pedido e nome do cliente), e o timestamp.

### **RF025: Adicionar um Produto/Item ao Carrinho**
- **Descrição**: O cliente pode adicionar um novo produto ao seu carrinho, especificando a quantidade desejada, adicionais e ingredientes removidos. O carrinho representa o estado temporário do pedido, permitindo ao cliente revisar e ajustar antes da finalização.

- **Entradas**:
    - Produto a ser adicionado
    - Quantidade
    - Adicionais selecionados
    - Ingredientes removidos (exceto obrigatórios)

- **Saídas**:
    - Confirmação de adição ao carrinho
    - Atualização do carrinho com o novo item, quantidade, adicionais e ingredientes removidos

- **Regras de Negócio**:
    - **RN001 - Validação de Quantidade**: A quantidade do item deve ser um número inteiro positivo. Se o cliente tentar adicionar uma quantidade inválida, o sistema deve exibir uma mensagem de erro.
    - **RN002 - Customização**: O cliente pode customizar o item com adicionais e remover ingredientes, com impacto no preço final.

### **RF026: Obter o Carrinho do Cliente**

- **Descrição**: O cliente deve poder visualizar o conteúdo do seu carrinho, que inclui detalhes sobre os itens, quantidades e o total do pedido.

- **Pré-condições**:
    - O usuário deve estar autenticado e ter perfil de **cliente**.

- **Entradas**:
    - ID do carrinho do cliente

- **Saídas**:
    - Detalhes do carrinho: Total do pedido e lista de itens, onde cada item inclui ID, título, URL da imagem, preço e quantidade.

- **Regras de Negócio**:
    - **RN001 - Exibição de Itens**: O sistema deve mostrar todos os itens no carrinho com informações completas, incluindo título, imagem, preço e quantidade.

    - **RN002 - Total do Carrinho**: O total do carrinho deve ser atualizado e exibido com precisão com base nas quantidades e personalizações dos itens.

### **RF027: Atualizar a Quantidade de um Item no Carrinho**
- **Descrição**: O cliente pode atualizar a quantidade de um item específico no carrinho.

- **Entradas**:
    - ID do item no carrinho
    - Nova quantidade desejada

- **Saídas**:
    - Confirmação de atualização da quantidade
    - Atualização do carrinho com a nova quantidade

- **Regras de Negócio**:
    - **RN001 - Quantidade Válida**: A nova quantidade deve ser um número inteiro positivo. Se a quantidade for menor que 1, o sistema deve exibir uma mensagem de erro.
    - **RN002 - Atualização de Preço**: O preço total do carrinho deve ser recalculado com base na nova quantidade do item.

### **RF028: Incrementar e Decrementar a Quantidade de um Item no Carrinho**
- **Descrição**: O cliente pode incrementar ou decrementar a quantidade de um item no carrinho, ajustando a quantidade em um (1) por vez.

- **Entradas**:
    - ID do item no carrinho
    - Ação (incrementar ou decrementar)

- **Saídas**:
    - Confirmação de ajuste da quantidade
    - Atualização do carrinho com a nova quantidade

- **Regras de Negócio**:
    - **RN001 - Incremento/Decremento**: A quantidade deve ser ajustada em um (1) de cada vez. A quantidade não pode ser menor que 1. Se a quantidade for 1 e o cliente tentar decrementar, o item deve ser removido do carrinho.

    - **RN002 - Atualização de Preço**: O preço total do carrinho deve ser recalculado após o ajuste da quantidade.

### **RF029: Deletar um Item do Carrinho**
- **Descrição**: O cliente pode remover um item específico do carrinho.

- **Entradas**:
    - ID do item a ser removido

- **Saídas**:
    - Confirmação de remoção do item
    - Atualização do carrinho sem o item removido

- **Regras de Negócio**:
    - **RN001 - Remoção de Item**: Ao remover um item, o sistema deve atualizar o carrinho e recalcular o total.
    - **RN002 - Validação de Remoção**: O sistema deve verificar se o ID do item é válido antes de realizar a remoção. Se o ID não existir, o sistema deve exibir uma mensagem de erro.

### **RF030: Checkout/Pagamento**
- **Descrição**: O cliente pode finalizar o pedido realizando o checkout através do sistema de pagamento integrado com o Stripe, usando apenas cartão de crédito. O cliente deve fornecer o ID do endereço para onde o pedido será enviado.

- **Entradas**:
    - ID do endereço de entrega (entre um dos vários endereços cadastrados pelo cliente)

- **Saídas**:
    - ID da sessão Stripe
    - URL para o pagamento

- **Regras de Negócio**:
    - **RN001 - Validação do Endereço**: O sistema deve verificar se o ID do endereço fornecido é válido e pertence ao cliente. Se o endereço for inválido, o sistema deve exibir uma mensagem de erro.

    - **RN002 - Criação da Sessão Stripe**: O sistema deve criar uma sessão de checkout no Stripe com base nas informações do carrinho do cliente e gerar uma URL para o pagamento.

    - **RN003 - URL de Pagamento**: A URL gerada deve permitir que o cliente finalize o pagamento de forma segura através do Stripe. Após o pagamento, o cliente deve ser redirecionado para uma página de confirmação.

### **RF031: Obter Todos os Endereços do Cliente**
- **Descrição**: O cliente pode visualizar todos os endereços que possui cadastrados no sistema. Esta funcionalidade permite ao cliente acessar e gerenciar seus endereços para facilitar o processo de checkout e a entrega.

- **Entradas**:
    - Nenhuma entrada necessária.

- **Saídas**:
    - Lista de endereços do cliente, incluindo:
        - ID do endereço
        - CEP
        - Número (opcional)
        - Rua
        - Bairro
        - Cidade
        - Estado
        - Complemento (opcional)
        - Referência (opcional)

- **Regras de Negócio**:
    - **RN001 - Autorização do Cliente**: Apenas o cliente autenticado pode acessar seus próprios endereços. O sistema deve validar a autenticidade do cliente antes de fornecer os dados.

### **RF032: Registrar um Novo Endereço do Cliente**
- **Descrição**: O cliente pode adicionar um novo endereço ao seu perfil. O sistema completa automaticamente os detalhes do endereço com base no CEP fornecido, e o número do endereço é opcional. Campos adicionais como referência e complemento são opcionais.

- **Entradas**:
    - CEP (obrigatório)
    - Número (opcional)
    - Referência (opcional)
    - Complemento (opcional)

- **Saídas**:
    - Confirmação do cadastro do endereço
    - ID do novo endereço

- **Regras de Negócio**:
    - **RN001 - Validação do CEP**: O sistema deve validar o CEP fornecido e completar os campos de endereço com base na consulta a um serviço de geolocalização ou base de dados.

    - **RN002 - Campos Opcionais**: O número do endereço é opcional, e campos como referência e complemento são opcionais. O sistema deve permitir o cadastro mesmo que esses campos não sejam preenchidos.

    - **RN003 - Autorização do Cliente**: Apenas o cliente autenticado pode registrar novos endereços. O sistema deve validar a identidade do cliente.

### **RF034: Deletar um Endereço**

- **Descrição**: O cliente pode excluir um endereço previamente cadastrado. O sistema deve garantir que apenas o cliente que cadastrou o endereço possa excluí-lo.


- **Pré-condições**:
    - O usuário deve estar autenticado e ter perfil de **cliente**.

- **Entradas**:
    - ID do endereço a ser excluído

- **Saídas**:
    - Confirmação da exclusão do endereço

- **Regras de Negócio**:
    - **RN001 - Autorização do Cliente**: Apenas o cliente autenticado pode excluir seus próprios endereços. O sistema deve verificar a identidade do cliente antes de permitir a exclusão.

### **RF035: Obter Todos os Pedidos Atuais do Cliente**
- **Descrição**: O cliente pode visualizar todos os pedidos atuais que estão pendentes, confirmados ou em preparo. Isso permite ao cliente acompanhar o status dos pedidos que ainda estão em processo.

- **Entradas**:
    - Nenhuma entrada necessária.

- **Saídas**:
    - Lista de pedidos atuais, incluindo:
        - ID do pedido
        - Data do pedido
        - Status do pedido (pendente, confirmado, em preparo)
        - Total do pedido

- **Regras de Negócio**:
    - **RN001 - Autorização do Cliente**: Apenas o cliente autenticado pode visualizar seus próprios pedidos atuais. O sistema deve validar a identidade do cliente antes de fornecer os dados.

    - **RN002 - Filtragem de Status**: Apenas pedidos com status "pendente", "confirmado" ou "em preparo" devem ser retornados. Pedidos com outros status devem ser excluídos da resposta.

### **RF036: Obter Detalhes do Pedido**

- **Descrição**: O cliente pode visualizar os detalhes completos de um pedido específico, incluindo os itens do pedido, adicionais, ingredientes removidos e informações de entrega.

- **Pré-condições**: O cliente deve estar autenticado para visualizar detalhes do pedido.

- **Entradas**:
    - ID do pedido

- **Saídas**:
    - Detalhes do pedido, incluindo:
        - ID do pedido
        - Nome completo do cliente
        - Endereço completo (incluindo referência e complemento)
        - Total do pedido
        - Status do pedido
        - Data do pedido
        - Itens do pedido:
            - Título do produto
            - Quantidade
            - Adicionais
            - Ingredientes removidos

- **Regras de Negócio**:
    - **RN001 - Autorização do Cliente**: Apenas o cliente autenticado pode visualizar os detalhes dos seus próprios pedidos. O sistema deve verificar a identidade do cliente.

    - **RN002 - Validação do Pedido**: O sistema deve garantir que o pedido exista e que o cliente tenha permissão para visualizar os detalhes.

### **RF037: Obter Histórico de Pedidos**
- **Descrição**: O cliente pode acessar o histórico de seus pedidos passados. O histórico é retornado de forma paginada com base nos parâmetros fornecidos pelo cliente.

- **Pré-condições**: O cliente deve estar autenticado e a sessão deve estar válida para acessar o histórico de pedidos.

- **Entradas**:
    - PageNumber (padrão = 1)
    - PageSize (padrão = 10)

- **Saídas**:
    - Lista paginada de pedidos históricos, incluindo:
        - ID do pedido
        - Data do pedido
        - Status do pedido
        - Total do pedido

- **Regras de Negócio**:
    - **RN001 - Autorização do Cliente**: Apenas o cliente autenticado pode acessar seu histórico de pedidos. O sistema deve garantir que o cliente tenha permissão para visualizar suas informações passadas.

    - **RN002 - Paginação**: Implementar paginação para gerenciar grandes volumes de pedidos históricos e garantir uma resposta eficiente.

### **RF038: Cancelar um Pedido**
- **Descrição**: O cliente pode cancelar um pedido que ainda não foi finalizado. O sistema deve processar o reembolso e registrar a razão fornecida pelo cliente para o cancelamento.
- **Pré-condições**: O cliente deve estar autenticado para cancelar um pedido.

- **Entradas**:
    - ID do pedido
    - Razão para o cancelamento (opcional)

- **Saídas**:
    - Confirmação do cancelamento do pedido
    - Status do pedido atualizado para "Cancelado pelo cliente"

- **Regras de Negócio**:
    - **RN001 - Autorização do Cliente**: Apenas o cliente autenticado pode cancelar seus próprios pedidos. O sistema deve verificar a identidade do cliente antes de processar o cancelamento.

    - **RN002 - Processamento do Reembolso**: O sistema deve iniciar o processo de reembolso automático para o cliente quando o pedido for cancelado.

    - **RN003 - Registro da Razão**: O sistema deve registrar a razão fornecida pelo cliente para o cancelamento, se fornecida, e atualizar o status do pedido para "Cancelado pelo cliente".

    - **RN004 - Validação do Pedido**: O sistema deve garantir que o pedido esteja em um estado que permita o cancelamento (por exemplo, não pode ser cancelado se já estiver em preparo ou finalizado).

### **RF039: Recomendação de Produtos com Base no Histórico de Pedidos**

- **Descrição**: O sistema deve fornecer recomendações personalizadas de produtos para o cliente com base no histórico de pedidos. O cliente pode acessar uma funcionalidade de recomendação através de um botão na interface de usuário.

- **Pré-condições**:
    - O cliente deve estar autenticado e ter um histórico de pedidos suficiente para gerar recomendações.

- **Saídas**:
    - Uma mensagem recomendando um pedido do menu com base no histórico de pedidos do cliente.

- **Regras de Negócio**:
    - **RN001 - Histórico de Pedidos**: As recomendações devem ser baseadas no histórico de pedidos do cliente. O sistema deve analisar os produtos mais frequentemente comprados ou visualizados.

    - **RN002 - Personalização**: O sistema deve gerar recomendações personalizadas para cada cliente com base em seu comportamento e histórico de compras.

### **RF040: Registro de Conta**

- **Descrição**: O sistema deve permitir que novos usuários se registrem, fornecendo nome completo, e-mail e senha. O registro cria uma nova conta de usuário no sistema.

- **Pré-condições**:
    - Nenhuma.

- **Entradas**:
    - `FullName`: Nome completo do usuário (obrigatório).
    - `Email`: Endereço de e-mail do usuário (obrigatório).
    - `Password`: Senha para a conta (obrigatório).

- **Saídas**:
    - Confirmação do registro.

- **Regras de Negócio**:
    - **RN001 - Validação do Email**: O e-mail fornecido deve ser único e seguir o formato válido. Caso contrário, o registro deve ser rejeitado.
    - **RN002 - Segurança da Senha**: A senha deve atender aos critérios de segurança definidos (por exemplo, comprimento mínimo, complexidade).
    - **RN003 - Nome Completo**: O nome completo deve ser fornecido e não pode estar vazio.

### **RF041: Redefinição de Senha**

- **Descrição**: O sistema deve permitir que os usuários solicitem a redefinição de senha enviando um token de 9 caracteres para o e-mail registrado. O token é utilizado para validar o pedido de redefinição de senha.

- **Pré-condições**:
    - O usuário deve ter uma conta registrada e fornecer um e-mail válido.

- **Entradas**:
    - `Email`: Endereço de e-mail do usuário para envio do token (obrigatório).

- **Saídas**:
    - Confirmação do envio do e-mail com o token de redefinição.

- **Regras de Negócio**:
    - **RN001 - Validação do Email**: O e-mail fornecido deve ser registrado no sistema. Caso contrário, o pedido de redefinição deve ser rejeitado.

    - **RN002 - Geração do Token**: Um token de 9 caracteres deve ser gerado e enviado para o e-mail fornecido.

    - **RN003 - Segurança do Token**: O token deve ser único e válido por um período de tempo limitado para garantir a segurança da redefinição de senha.

## Casos de Uso

### Caso de Uso: Cadastro de Adicional

**Ator(es):**

- Administrador

**Objetivo:**

- O administrador deseja cadastrar um novo adicional para ser utilizado na personalização de produtos.

#### Fluxo Principal

1. **O administrador acessa a página de cadastro de adicionais.**
    - Navega até a página de cadastro no sistema.

2. **O sistema exibe o formulário de cadastro de adicionais.**
    - O formulário contém campos para o nome do adicional e a categoria à qual ele pertence.

3. **O administrador preenche o formulário com as informações do adicional.**
    - Nome do adicional: Queijo Cheddar
    - Categoria do adicional: Lanche

4. **O administrador envia o formulário.**
    - O sistema valida as entradas e processa o cadastro.

5. **O sistema valida os dados fornecidos.**
    - Verifica se todos os campos obrigatórios foram preenchidos e se o nome do adicional é único.

6. **O sistema verifica se a categoria informada existe.**
    - Confirma que a categoria fornecida está cadastrada no sistema.

7. **O sistema realiza o cadastro do adicional e confirma a operação.**
    - O adicional é adicionado ao sistema e uma mensagem de sucesso é exibida.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: Nome do adicional já existente**
    - **Passo 1:** O sistema detecta que o nome do adicional já está em uso.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o nome já existe e solicita ao administrador para fornecer um nome diferente.
    - **Passo 3:** O administrador altera o nome e reenvia o formulário ou abandona o cadastro.

2. **Fluxo Alternativo 2: Categoria inválida**
    - **Passo 1:** O sistema detecta que a categoria fornecida não existe no sistema.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que a categoria é inválida e solicita ao administrador para selecionar uma categoria válida.
    - **Passo 3:** O administrador seleciona uma categoria válida e reenvia o formulário ou abandona o cadastro.

3. **Fluxo Alternativo 3: Campos obrigatórios não preenchidos**
    - **Passo 1:** O sistema detecta que um ou mais campos obrigatórios estão vazios.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando quais campos precisam ser preenchidos e solicita ao administrador para completar o formulário.
    - **Passo 3:** O administrador preenche os campos obrigatórios e reenvia o formulário.

4. **Fluxo Alternativo 4: Nome do adicional fora do intervalo de caracteres permitido**
    - **Passo 1:** O sistema detecta que o nome do adicional não está dentro do intervalo de 3 a 50 caracteres.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o nome deve ter entre 3 e 50 caracteres e solicita ao administrador para corrigir o nome.
    - **Passo 3:** O administrador ajusta o nome e reenvia o formulário.

### Caso de Uso: Atualização de Adicional

**Ator(es):**

- Administrador

**Objetivo:**

- O administrador deseja atualizar os dados de um adicional existente, modificando tanto o nome quanto a categoria do adicional previamente cadastrado.

#### Fluxo Principal

1. **O administrador acessa a página de atualização de adicionais.**
    - Navega até a página de atualização no sistema.

2. **O sistema exibe o formulário de atualização de adicionais.**
    - O formulário é pré-preenchido com os dados atuais do adicional e permite a edição do nome e da categoria.

3. **O administrador modifica o nome e/ou a categoria do adicional.**
    - Nome atualizado do adicional
    - Categoria atualizada do adicional

4. **O administrador envia o formulário de atualização.**
    - O sistema valida as entradas e processa a atualização.

5. **O sistema valida o ID do adicional fornecido.**
    - Verifica se o ID corresponde a um adicional existente no sistema.

6. **O sistema valida a unicidade do novo nome.**
    - Verifica se o novo nome do adicional não está em uso por outro adicional.

7. **O sistema valida a existência da nova categoria.**
    - Confirma que a nova categoria fornecida está cadastrada no sistema.

8. **O sistema realiza a atualização do adicional e confirma a operação.**
    - Os dados do adicional são atualizados e uma mensagem de sucesso é exibida.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: ID do adicional não existente**
    - **Passo 1:** O sistema detecta que o ID fornecido não corresponde a um adicional existente.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o adicional não foi encontrado e solicita ao administrador para fornecer um ID válido.
    - **Passo 3:** O administrador corrige o ID e reenvia o formulário ou abandona a atualização.

2. **Fluxo Alternativo 2: Nome do adicional já existente**
    - **Passo 1:** O sistema detecta que o novo nome do adicional já está em uso por outro adicional.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o nome já existe e solicita ao administrador para fornecer um nome diferente.
    - **Passo 3:** O administrador altera o nome e reenvia o formulário ou abandona a atualização.

3. **Fluxo Alternativo 3: Categoria inválida**
    - **Passo 1:** O sistema detecta que a nova categoria fornecida não existe no sistema.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que a categoria é inválida e solicita ao administrador para selecionar uma categoria válida.
    - **Passo 3:** O administrador seleciona uma categoria válida e reenvia o formulário ou abandona a atualização.

4. **Fluxo Alternativo 4: Campos obrigatórios não preenchidos**
    - **Passo 1:** O sistema detecta que um ou mais campos obrigatórios estão vazios.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando quais campos precisam ser preenchidos e solicita ao administrador para completar o formulário.
    - **Passo 3:** O administrador preenche os campos obrigatórios e reenvia o formulário.

5. **Fluxo Alternativo 5: Nome do adicional fora do intervalo de caracteres permitido**
    - **Passo 1:** O sistema detecta que o nome do adicional não está dentro do intervalo de 3 a 50 caracteres.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o nome deve ter entre 3 e 50 caracteres e solicita ao administrador para corrigir o nome.
    - **Passo 3:** O administrador ajusta o nome e reenvia o formulário.

### Caso de Uso: Exclusão de Adicional

**Ator(es):**

- Administrador

**Objetivo:**

- O administrador deseja excluir um adicional existente, removendo-o permanentemente do catálogo, desde que as condições de exclusão sejam atendidas.

#### Fluxo Principal

1. **O administrador acessa a lista de adicionais.**
    - Navega até a página onde os adicionais são listados.

2. **O sistema exibe a lista de adicionais com um ícone de lixeira para cada item.**
    - A lista mostra todos os adicionais cadastrados e um ícone de lixeira ao lado de cada adicional.

3. **O administrador clica no ícone de lixeira ao lado do adicional que deseja excluir.**
    - Um modal de confirmação de exclusão é exibido.

4. **O sistema exibe um modal de confirmação de exclusão.**
    - O modal contém uma mensagem de confirmação e solicita ao administrador a confirmação da exclusão.

5. **O administrador confirma a exclusão no modal.**
    - O administrador clica em "Confirmar" para prosseguir com a exclusão.

6. **O sistema valida o ID do adicional fornecido implicitamente pelo ícone de lixeira.**
    - Verifica se o ID do adicional selecionado corresponde a um adicional existente no sistema.

7. **O sistema verifica se o adicional está associado a produtos ou pedidos.**
    - Confirma se o adicional está associado a algum produto ou pedido em andamento.

8. **O sistema realiza a exclusão do adicional e confirma a operação.**
    - O adicional é removido do sistema e uma mensagem de sucesso é exibida.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: ID do adicional não existente**
    - **Passo 1:** O sistema detecta que o ID do adicional não corresponde a um adicional existente.
    - **Passo 2:** O sistema exibe uma mensagem de erro indicando que o adicional não foi encontrado e solicita ao administrador para tentar novamente.
    - **Passo 3:** O administrador tenta excluir um adicional existente ou abandona a exclusão.

2. **Fluxo Alternativo 2: Adicional associado a produtos ou pedidos**
    - **Passo 1:** O sistema detecta que o adicional está associado a produtos ou pedidos em andamento.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que a exclusão é bloqueada devido à associação com produtos ou pedidos.
    - **Passo 3:** O administrador revisa a associação e decide se deve remover a associação antes de tentar a exclusão novamente ou abandona a exclusão.

### Caso de Uso: Cadastro de Ingrediente

**Ator(es):**

- Administrador

**Objetivo:**

- O administrador deseja cadastrar um novo ingrediente, inserindo apenas o nome do ingrediente, para ampliar as opções de personalização no cardápio.

#### Fluxo Principal

1. **O administrador acessa a página de cadastro de ingredientes.**
    - Navega até a página onde é possível cadastrar novos ingredientes.

2. **O sistema exibe o formulário de cadastro de ingredientes.**
    - O formulário contém um campo para o nome do ingrediente.

3. **O administrador preenche o formulário com o nome do ingrediente.**
    - Nome do ingrediente: [Inserir nome do ingrediente]

4. **O administrador envia o formulário.**
    - O sistema valida as entradas e processa o cadastro.

5. **O sistema valida o nome do ingrediente.**
    - Verifica se o nome fornecido é único e se atende aos critérios de comprimento.

6. **O sistema realiza o cadastro do ingrediente e confirma a operação.**
    - O ingrediente é adicionado ao sistema e uma mensagem de sucesso é exibida.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: Nome do ingrediente já existente**
    - **Passo 1:** O sistema detecta que o nome do ingrediente já está em uso.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o nome já existe e solicita ao administrador para fornecer um nome diferente.
    - **Passo 3:** O administrador altera o nome e reenvia o formulário ou abandona o cadastro.

2. **Fluxo Alternativo 2: Nome do ingrediente vazio**
    - **Passo 1:** O sistema detecta que o nome do ingrediente está vazio.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o campo nome é obrigatório e solicita ao administrador para preencher o campo.
    - **Passo 3:** O administrador preenche o nome e reenvia o formulário ou abandona o cadastro.

3. **Fluxo Alternativo 3: Nome do ingrediente fora do intervalo de caracteres permitido**
    - **Passo 1:** O sistema detecta que o nome do ingrediente não está dentro do intervalo de 3 a 50 caracteres.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o nome deve ter entre 3 e 50 caracteres e solicita ao administrador para corrigir o nome.
    - **Passo 3:** O administrador ajusta o nome e reenvia o formulário ou abandona o cadastro.


### Caso de Uso: Atualização de Ingrediente

**Ator(es):**

- Administrador

**Objetivo:**

- O administrador deseja atualizar o nome de um ingrediente existente para refletir alterações ou correções.

#### Fluxo Principal

1. **O administrador acessa a página de edição de ingredientes.**
    - Navega até a página onde os ingredientes podem ser atualizados.

2. **O sistema exibe a lista de ingredientes existentes com uma opção de edição.**
    - A lista mostra todos os ingredientes cadastrados e uma opção para editar cada um.

3. **O administrador seleciona o ingrediente que deseja atualizar.**
    - Um formulário de edição é exibido com o nome atual do ingrediente.

4. **O administrador modifica o nome do ingrediente no formulário.**
    - Nome atualizado do ingrediente: [Inserir novo nome do ingrediente]

5. **O administrador envia o formulário de atualização.**
    - O sistema valida as entradas e processa a atualização.

6. **O sistema valida o ID do ingrediente e o novo nome.**
    - Verifica se o ID fornecido corresponde a um ingrediente existente e se o nome atualizado é único e atende aos critérios de comprimento.

7. **O sistema realiza a atualização do ingrediente e confirma a operação.**
    - O ingrediente é atualizado no sistema e uma mensagem de sucesso é exibida.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: ID do ingrediente não existente**
    - **Passo 1:** O sistema detecta que o ID do ingrediente não corresponde a um ingrediente existente.
    - **Passo 2:** O sistema exibe uma mensagem de erro indicando que o ingrediente não foi encontrado e solicita ao administrador para tentar novamente com um ID válido.

2. **Fluxo Alternativo 2: Nome do ingrediente já existente**
    - **Passo 1:** O sistema detecta que o nome atualizado já está em uso por outro ingrediente.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o nome já existe e solicita ao administrador para fornecer um nome diferente.

3. **Fluxo Alternativo 3: Nome do ingrediente vazio**
    - **Passo 1:** O sistema detecta que o nome do ingrediente está vazio.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o campo nome é obrigatório e solicita ao administrador para preencher o campo.

4. **Fluxo Alternativo 4: Nome do ingrediente fora do intervalo de caracteres permitido**
    - **Passo 1:** O sistema detecta que o nome do ingrediente não está dentro do intervalo de 3 a 50 caracteres.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o nome deve ter entre 3 e 50 caracteres e solicita ao administrador para corrigir o nome.

### Caso de Uso: Exclusão de Ingrediente

**Ator(es):**

- Administrador

**Objetivo:**

- O administrador deseja excluir um ingrediente existente do sistema, removendo-o permanentemente do catálogo, desde que o ingrediente não esteja associado a produtos ou pedidos em andamento.

#### Fluxo Principal

1. **O administrador acessa a página de gerenciamento de ingredientes.**
    - Navega até a seção do sistema onde os ingredientes são gerenciados.

2. **O sistema exibe a lista de ingredientes existentes com opções de exclusão.**
    - A lista mostra todos os ingredientes cadastrados e um ícone de lixeira para cada ingrediente.

3. **O administrador seleciona o ingrediente que deseja excluir clicando no ícone de lixeira.**
    - Um modal de confirmação é exibido, solicitando a confirmação da exclusão.

4. **O administrador confirma a exclusão no modal.**
    - O sistema processa a solicitação de exclusão.

5. **O sistema valida o ID do ingrediente.**
    - Verifica se o ID fornecido corresponde a um ingrediente existente.

6. **O sistema verifica se o ingrediente está associado a produtos ou pedidos em andamento.**
    - Se o ingrediente estiver associado, a exclusão é bloqueada e uma mensagem de erro é exibida, solicitando ao administrador que desvincule o ingrediente.

7. **O sistema realiza a exclusão do ingrediente e confirma a operação.**
    - O ingrediente é removido do sistema e uma mensagem de sucesso é exibida.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: ID do ingrediente não existente**
    - **Passo 1:** O sistema detecta que o ID do ingrediente não corresponde a um ingrediente existente.
    - **Passo 2:** O sistema exibe uma mensagem de erro indicando que o ingrediente não foi encontrado e solicita ao administrador para tentar novamente com um ID válido.

2. **Fluxo Alternativo 2: Ingrediente associado a produtos/pedidos**
    - **Passo 1:** O sistema detecta que o ingrediente está associado a um ou mais produtos ou pedidos em andamento.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o ingrediente não pode ser excluído enquanto estiver associado e orienta o administrador a desvincular o ingrediente antes de tentar novamente.

### Caso de Uso: Listar Ingredientes

**Ator(es):**

- Administrador

**Objetivo:**

- O administrador deseja visualizar a lista completa de todos os ingredientes cadastrados no sistema para revisar e gerenciar os itens disponíveis.

#### Fluxo Principal

1. **O administrador acessa a página de listagem de ingredientes.**
    - Navega até a seção do sistema onde os ingredientes são listados.

2. **O sistema verifica se o usuário está autenticado e possui o perfil de administrador.**
    - Confirma que o usuário tem as permissões necessárias para acessar a lista de ingredientes.

3. **O sistema recupera todos os ingredientes cadastrados.**
    - Obtém dados sobre todos os ingredientes presentes no sistema.

4. **O sistema exibe a lista de ingredientes.**
    - Mostra uma lista completa contendo o nome e o ID de cada ingrediente.

5. **O administrador revisa a lista de ingredientes.**
    - O administrador pode visualizar os dados e, se necessário, tomar ações adicionais, como edição ou exclusão de ingredientes.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: Acesso não autorizado**
    - **Passo 1:** O administrador tenta acessar a lista de ingredientes, mas não está autenticado ou não possui perfil de administrador.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o acesso à lista de ingredientes é restrito a administradores e solicita o login ou a alteração de permissões, se necessário.

2. **Fluxo Alternativo 2: Erro na recuperação de dados**
    - **Passo 1:** O sistema encontra um erro ao tentar recuperar a lista de ingredientes (por exemplo, falha na conexão com o banco de dados).
    - **Passo 2:** O sistema exibe uma mensagem de erro indicando que não foi possível carregar a lista de ingredientes no momento e se ele acredita que isso é um erro para contatar o suporte.

### Caso de Uso: Buscar Todas as Categorias

**Ator(es):**

- Cliente
- Administrador

**Objetivo:**

- O cliente ou administrador deseja visualizar a lista completa de todas as categorias disponíveis no sistema, como "Comida", "Bebida", entre outras. Esta funcionalidade permite conhecer as opções disponíveis e facilitar a navegação e escolha de itens.

#### Fluxo Principal

1. **O usuário (cliente ou administrador) acessa a página de categorias.**
    - Navega até a seção do sistema onde as categorias são listadas.

2. **O sistema recupera todas as categorias cadastradas.**
    - Obtém dados sobre todas as categorias presentes no sistema.

3. **O sistema exibe a lista de categorias.**
    - Mostra uma lista completa contendo o nome e o ID de cada categoria.

4. **O usuário revisa a lista de categorias.**
    - O usuário pode visualizar as categorias disponíveis e, se necessário, utilizar essas informações para navegação ou associação de produtos.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: Erro na recuperação de dados**
    - **Passo 1:** O sistema encontra um erro ao tentar recuperar a lista de categorias (por exemplo, falha na conexão com o banco de dados).
    - **Passo 2:** O sistema exibe uma mensagem de erro indicando que não foi possível carregar a lista de categorias no momento e solicita ao usuário para tentar novamente mais tarde.

2. **Fluxo Alternativo 2: Lista de categorias vazia**
    - **Passo 1:** O sistema recupera a lista de categorias, mas não existem categorias cadastradas.
    - **Passo 2:** O sistema exibe uma mensagem informando que nenhuma categoria está disponível no momento.

### Caso de Uso: Buscar Categoria por ID

**Ator(es):**

- Cliente
- Administrador

**Objetivo:**

- O cliente ou administrador deseja buscar informações sobre uma categoria específica fornecendo o seu ID. Isso facilita a localização de detalhes sobre categorias específicas, como nome e ID.

#### Fluxo Principal

1. **O usuário (cliente ou administrador) solicita a busca de uma categoria.**
    - Fornece o ID da categoria que deseja consultar.

2. **O sistema recebe o ID da categoria e realiza a consulta.**
    - O sistema procura a categoria correspondente ao ID fornecido.

3. **O sistema valida se o ID fornecido corresponde a uma categoria existente.**
    - Verifica se a categoria com o ID fornecido está cadastrada no sistema.
4. **O sistema retorna as informações da categoria.**
    - Exibe o nome e o ID da categoria correspondente.

5. **O usuário visualiza as informações da categoria.**
    - O usuário pode ver o nome e ID da categoria solicitada.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: ID da categoria não encontrado**
    - **Passo 1:** O sistema não encontra uma categoria correspondente ao ID fornecido.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que a categoria com o ID fornecido não foi encontrada.

### Caso de Uso: Criar Nova Categoria

**Ator(es):**

- Administrador

**Objetivo:**

- O administrador deseja criar uma nova categoria, fornecendo um nome único para a mesma, permitindo uma maior flexibilidade na gestão das categorias do sistema.

#### Fluxo Principal

1. **O administrador acessa a funcionalidade de criação de categorias.**
    - Fornece o nome da nova categoria a ser criada.

2. **O sistema valida as permissões do usuário.**
    - O sistema verifica se o usuário tem perfil de administrador.
    - Caso o usuário não tenha permissão, o sistema exibe uma mensagem de erro e interrompe o processo (Fluxo Alternativo 1).

3. **O sistema valida o nome da categoria.**
    - O sistema verifica se o nome da nova categoria é único e se atende ao limite de caracteres (entre 3 e 50 caracteres).
    - Caso o nome já exista ou esteja fora dos limites, o sistema exibe uma mensagem de erro (Fluxo Alternativo 2).

4. **O sistema cria a nova categoria.**
    - O sistema cadastra a nova categoria no banco de dados.

5. **O sistema exibe uma confirmação.**
    - O sistema informa ao administrador que a nova categoria foi cadastrada com sucesso.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: Usuário sem permissão de criação**
    - **Passo 1:** O sistema verifica que o usuário não possui perfil de administrador.
    - **Passo 2:** O sistema nega o acesso à funcionalidade e exibe uma mensagem de erro, informando que apenas administradores podem criar novas categorias.

2. **Fluxo Alternativo 2: Nome da categoria inválido**
    - **Passo 1:** O sistema valida o nome fornecido para a nova categoria e detecta que ele já existe ou não atende ao limite de caracteres.
    - **Passo 2:** O sistema exibe uma mensagem de erro, informando que o nome da categoria já está em uso ou que não atende aos critérios de tamanho (mínimo de 3 e máximo de 50 caracteres).

3. **Fluxo Alternativo 3: Erro no cadastro**
    - **Passo 1:** O sistema encontra um erro ao tentar criar a nova categoria (por exemplo, falha no banco de dados).
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que ocorreu um problema ao cadastrar a nova categoria e solicita ao administrador para entrar em contato com o suporte.

### Caso de Uso: Atualizar Categoria

**Ator(es):**

- Administrador

**Objetivo:**

- O administrador deseja atualizar o nome de uma categoria existente no sistema, corrigindo ou ajustando o nome de uma categoria sem precisar excluí-la e recriá-la.

#### Fluxo Principal

1. **O administrador acessa a funcionalidade de atualização de categoria.**
    - Fornece o ID da categoria a ser atualizada e o novo nome desejado.

2. **O sistema valida as permissões do usuário.**
    - O sistema verifica se o usuário tem perfil de administrador.
    - Caso o usuário não tenha permissão, o sistema exibe uma mensagem de erro e interrompe o processo (Fluxo Alternativo 1).

3. **O sistema valida o ID da categoria.**
    - O sistema verifica se o ID fornecido corresponde a uma categoria existente.
    - Caso o ID não corresponda a nenhuma categoria, o sistema exibe uma mensagem de erro (Fluxo Alternativo 2).

4. **O sistema valida o novo nome da categoria.**
    - O sistema verifica se o novo nome da categoria é único e se atende ao limite de caracteres (entre 3 e 50 caracteres).
    - Caso o nome já exista ou esteja fora dos limites, o sistema exibe uma mensagem de erro (Fluxo Alternativo 3).

5. **O sistema atualiza o nome da categoria.**
    - O sistema altera o nome da categoria no banco de dados.

6. **O sistema exibe uma confirmação.**
    - O sistema informa ao administrador que a categoria foi atualizada com sucesso.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: Usuário sem permissão de atualização**
    - **Passo 1:** O sistema verifica que o usuário não possui perfil de administrador.
    - **Passo 2:** O sistema nega o acesso à funcionalidade e exibe uma mensagem de erro, informando que apenas administradores podem atualizar categorias.

2. **Fluxo Alternativo 2: ID inválido**
    - **Passo 1:** O sistema valida o ID fornecido e detecta que ele não corresponde a nenhuma categoria existente.
    - **Passo 2:** O sistema exibe uma mensagem de erro, informando que o ID fornecido é inválido.

3. **Fluxo Alternativo 3: Nome da categoria inválido**
    - **Passo 1:** O sistema valida o novo nome da categoria e detecta que ele já está em uso ou não atende ao limite de caracteres.

    - **Passo 2:** O sistema exibe uma mensagem de erro, informando que o nome da categoria já está em uso ou que não atende aos critérios de tamanho (mínimo de 3 e máximo de 50 caracteres).

4. **Fluxo Alternativo 4: Erro no processo de atualização**
    - **Passo 1:** O sistema encontra um erro ao tentar atualizar a categoria (ex: falha no banco de dados).
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que ocorreu um problema ao atualizar a categoria e solicita ao administrador para tentar novamente mais tarde.

### Caso de Uso: Deletar Categoria

**Ator(es):**

- Administrador

**Objetivo:**

- O administrador deseja excluir uma categoria existente que não é mais necessária no sistema.

#### Fluxo Principal

1. **O administrador acessa a funcionalidade de exclusão de categoria.**
    - Fornece o ID da categoria a ser excluída.

2. **O sistema valida as permissões do usuário.**
    - O sistema verifica se o usuário tem perfil de administrador.
    - Caso o usuário não tenha permissão, o sistema exibe uma mensagem de erro e interrompe o processo

3. **O sistema valida o ID da categoria.**
    - O sistema verifica se o ID fornecido corresponde a uma categoria existente.
    - Caso o ID não corresponda a nenhuma categoria, o sistema exibe uma mensagem de erro

4. **O sistema verifica se a categoria está associada a algum produto.**
    - Caso a categoria esteja associada a um ou mais produtos, o sistema não permite a exclusão e exibe uma mensagem de erro.

5. **O sistema exclui a categoria.**
    - Caso todas as validações sejam bem-sucedidas, o sistema remove a categoria do banco de dados.

6. **O sistema exibe uma confirmação.**
    - O sistema informa ao administrador que a categoria foi excluída com sucesso.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: Usuário sem permissão de exclusão**
    - **Passo 1:** O sistema detecta que o usuário não possui perfil de administrador.
    - **Passo 2:** O sistema nega o acesso à funcionalidade e exibe uma mensagem de erro, informando que apenas administradores podem excluir categorias.

2. **Fluxo Alternativo 2: ID inválido**
    - **Passo 1:** O sistema valida o ID fornecido e detecta que ele não corresponde a nenhuma categoria existente.
    - **Passo 2:** O sistema exibe uma mensagem de erro, informando que o ID fornecido é inválido.

3. **Fluxo Alternativo 3: Categoria associada a produtos**
    - **Passo 1:** O sistema verifica que a categoria está associada a um ou mais produtos.
    - **Passo 2:** O sistema exibe uma mensagem de erro, informando que a categoria não pode ser excluída até que todos os produtos associados sejam realocados ou removidos da categoria.

4. **Fluxo Alternativo 4: Erro no processo de exclusão**
    - **Passo 1:** O sistema encontra um erro ao tentar excluir a categoria (ex: falha no banco de dados).
    - **Passo 2:** O sistema exibe uma mensagem de erro, solicitando ao administrador que entre em contato com o suporte.

### Caso de Uso: Cadastrar Produto

**Ator(es):**

- Administrador

**Objetivo:**

- O administrador deseja cadastrar um novo produto no sistema, fornecendo suas informações básicas como nome, preço, descrição, categoria e ingredientes.

#### Fluxo Principal

1. **O administrador acessa a funcionalidade de cadastro de produto.**
    - O administrador insere os seguintes dados: nome, preço, descrição, ID da categoria e IDs dos ingredientes.

2. **O sistema valida as permissões do usuário.**
    - O sistema verifica se o usuário possui perfil de administrador.
    - Caso o usuário não tenha permissão, o sistema exibe uma mensagem de erro e interrompe o processo (Fluxo Alternativo 1).

3. **O sistema valida os campos obrigatórios.**
    - O sistema verifica se os campos nome, preço, descrição, categoria e ingredientes foram preenchidos corretamente.
    - Caso algum campo obrigatório esteja vazio ou inválido, o sistema exibe uma mensagem de erro (Fluxo Alternativo 2).

4. **O sistema valida o preço do produto.**
    - O sistema verifica se o preço é um valor positivo e maior que zero.
    - Caso o preço seja inválido, o sistema exibe uma mensagem de erro (Fluxo Alternativo 3).

5. **O sistema valida os limites de caracteres.**
    - O sistema verifica se o nome do produto possui entre 3 e 100 caracteres e se a descrição tem no máximo 500 caracteres.
    - Caso os limites não sejam atendidos, o sistema exibe uma mensagem de erro (Fluxo Alternativo 4).

6. **O sistema verifica a existência da categoria e dos ingredientes.**
    - O sistema valida os IDs da categoria e dos ingredientes fornecidos.
    - Caso algum ID seja inválido, o sistema exibe uma mensagem de erro (Fluxo Alternativo 5).

7. **O sistema cadastra o novo produto.**
    - Se todas as validações forem bem-sucedidas, o sistema salva o produto no banco de dados.

8. **O sistema exibe uma confirmação.**
    - O sistema informa ao administrador que o produto foi cadastrado com sucesso.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: Usuário sem permissão de cadastro**
    - **Passo 1:** O sistema detecta que o usuário não possui perfil de administrador.
    - **Passo 2:** O sistema nega o acesso à funcionalidade e exibe uma mensagem de erro, informando que apenas administradores podem cadastrar produtos.

2. **Fluxo Alternativo 2: Campos obrigatórios não preenchidos**
    - **Passo 1:** O sistema valida os campos obrigatórios e detecta que algum campo está vazio ou inválido.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando o campo que deve ser corrigido.

3. **Fluxo Alternativo 3: Preço inválido**
    - **Passo 1:** O sistema verifica o preço e detecta que ele não é um valor positivo ou maior que zero.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o preço deve ser um valor positivo.

4. **Fluxo Alternativo 4: Limite de caracteres não atendido**
    - **Passo 1:** O sistema valida o tamanho do nome ou da descrição e detecta que algum deles está fora dos limites permitidos.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o nome deve ter entre 3 e 100 caracteres e que a descrição pode ter no máximo 500 caracteres.

5. **Fluxo Alternativo 5: Categoria ou ingredientes inexistentes**
    - **Passo 1:** O sistema valida os IDs da categoria e dos ingredientes e detecta que algum deles não existe no sistema.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que a categoria ou os ingredientes fornecidos são inválidos.

### Caso de Uso: Upload de Imagem para Produto

**Ator(es):**

- Administrador

**Objetivo:**

- O administrador deseja fazer o upload de uma imagem para representar um produto previamente cadastrado no sistema.

#### Fluxo Principal

1. **O administrador acessa a funcionalidade de upload de imagem.**
    - O administrador seleciona o produto pelo seu ID e escolhe o arquivo de imagem (PNG ou JPG).

2. **O sistema valida as permissões do usuário.**
    - O sistema verifica se o usuário possui perfil de administrador.
    - Caso o usuário não tenha permissão, o sistema exibe uma mensagem de erro e interrompe o processo (Fluxo Alternativo 1).

3. **O sistema valida o ID do produto.**
    - O sistema verifica se o produto existe no banco de dados com base no ID fornecido.
    - Caso o produto não exista, o sistema exibe uma mensagem de erro (Fluxo Alternativo 2).

4. **O sistema valida o tamanho do arquivo.**
    - O sistema verifica se o arquivo de imagem possui no máximo 10 MB.
    - Caso o arquivo ultrapasse o limite de tamanho, o sistema exibe uma mensagem de erro (Fluxo Alternativo 3).

5. **O sistema valida o formato do arquivo.**
    - O sistema verifica se a imagem está no formato PNG ou JPG.
    - Caso o arquivo esteja em outro formato, o sistema exibe uma mensagem de erro (Fluxo Alternativo 4).

6. **O sistema realiza o upload da imagem.**
    - Se todas as validações forem bem-sucedidas, o sistema faz o upload da imagem e a associa ao produto.

7. **O sistema exibe uma confirmação.**
    - O sistema informa ao administrador que o upload da imagem foi realizado com sucesso.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: Usuário sem permissão para upload**
    - **Passo 1:** O sistema detecta que o usuário não possui perfil de administrador.
    - **Passo 2:** O sistema nega o acesso à funcionalidade e exibe uma mensagem de erro, informando que apenas administradores podem realizar o upload de imagens.

2. **Fluxo Alternativo 2: Produto não existente**
    - **Passo 1:** O sistema valida o ID do produto e detecta que o produto não existe no banco de dados.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o produto selecionado não foi encontrado.

3. **Fluxo Alternativo 3: Arquivo excede o tamanho permitido**
    - **Passo 1:** O sistema verifica o tamanho do arquivo de imagem e detecta que ele ultrapassa o limite de 10 MB.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o arquivo de imagem deve ter no máximo 10 MB.

4. **Fluxo Alternativo 4: Formato de arquivo inválido**
    - **Passo 1:** O sistema verifica o formato da imagem e detecta que o arquivo não está no formato PNG ou JPG.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o formato do arquivo deve ser PNG ou JPG.

### Caso de Uso: Listar Produtos com Paginação e Filtros

**Ator(es):**

- Clientes
- Administradores

**Objetivo:**

- O usuário deseja listar os produtos disponíveis no sistema, podendo aplicar filtros e definir a quantidade de itens por página.

#### Fluxo Principal

1. **O usuário acessa a funcionalidade de listagem de produtos.**
    - O usuário define a página atual (`Page`), o número de itens por página (`PageSize`), e, opcionalmente, parâmetros de filtro como `Title`, `MinPrice` e `MaxPrice`.

2. **O sistema aplica a paginação com base nos parâmetros fornecidos.**
    - O sistema verifica se os parâmetros `Page` e `PageSize` foram fornecidos e, caso contrário, aplica os valores padrão (página 1 e 10 produtos por página).

3. **O sistema aplica os filtros opcionais.**
    - O sistema verifica se foram fornecidos filtros para `Title`, `MinPrice` e `MaxPrice`:
        - Se o `Title` for fornecido, o sistema busca produtos cujo nome contenha o termo informado (ignorando maiúsculas/minúsculas).
        - Se `MinPrice` e/ou `MaxPrice` forem fornecidos, o sistema busca produtos que estejam dentro do intervalo de preços.

4. **O sistema lista os produtos paginados de acordo com os filtros aplicados.**
    - O sistema retorna a lista de produtos com os campos: nome, preço, descrição, ingredientes, categoria e imagem.

5. **O sistema exibe a lista de produtos ao usuário.**
    - O usuário visualiza os produtos de acordo com a página e os filtros selecionados.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: Parâmetro `PageSize` maior que o limite permitido**
    - **Passo 1:** O sistema verifica que o parâmetro `PageSize` fornecido é maior que 100.
    - **Passo 2:** O sistema aplica o valor máximo permitido de 100 itens por página.
    - **Passo 3:** O sistema exibe uma mensagem informando que o limite de 100 itens por página foi aplicado.

2. **Fluxo Alternativo 2: Filtros inválidos**
    - **Passo 1:** O usuário fornece um valor inválido para `MinPrice` ou `MaxPrice` (por exemplo, `MinPrice` maior que `MaxPrice`).
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que os valores fornecidos para o filtro de preço são inválidos e solicita a correção dos parâmetros.

3. **Fluxo Alternativo 3: Nenhum produto encontrado**
    - **Passo 1:** O sistema verifica que não existem produtos que correspondem aos filtros aplicados.
    - **Passo 2:** O sistema exibe uma mensagem informando que nenhum produto foi encontrado com os filtros selecionados.

4. **Fluxo Alternativo 4: Nenhum parâmetro fornecido**
    - **Passo 1:** O usuário acessa a listagem de produtos sem fornecer nenhum filtro ou parâmetros de paginação.
    - **Passo 2:** O sistema aplica os valores padrão (`Page = 1` e `PageSize = 10`) e exibe todos os produtos paginados.

### Caso de Uso: Atualizar Produto

**Ator(es):**

- Administradores

**Objetivo:**

- O administrador deseja atualizar as informações de um produto já cadastrado no sistema.

#### Fluxo Principal

1. **O administrador acessa a funcionalidade de atualização de produto.**
    - O administrador fornece o ID do produto a ser atualizado e os novos valores para nome, preço, descrição, categoria e ingredientes.

2. **O sistema valida o ID do produto.**
    - O sistema verifica se o ID fornecido corresponde a um produto existente.
    - Se o ID não existir, o sistema exibe uma mensagem de erro e encerra o fluxo.

3. **O sistema valida os campos fornecidos.**
    - O sistema verifica se todos os campos obrigatórios foram preenchidos corretamente:
        - Nome (3 a 100 caracteres)
        - Preço (maior que zero)
        - Descrição (máximo de 500 caracteres)
        - ID da categoria
        - IDs dos ingredientes
    - Se algum campo não atender aos critérios, o sistema exibe uma mensagem de erro correspondente.

4. **O sistema atualiza as informações do produto.**
    - O sistema realiza a atualização das informações do produto com os novos valores fornecidos.

5. **O sistema confirma a atualização.**
    - O sistema retorna uma mensagem de confirmação informando que as informações do produto foram atualizadas com sucesso.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: ID do produto não existe**
    - **Passo 1:** O administrador fornece um ID de produto que não está cadastrado.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o ID não corresponde a nenhum produto.

2. **Fluxo Alternativo 2: Campos obrigatórios não preenchidos**
    - **Passo 1:** O administrador não preenche um ou mais campos obrigatórios.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando quais campos precisam ser preenchidos.

3. **Fluxo Alternativo 3: Nome do produto fora do limite de caracteres**
    - **Passo 1:** O administrador fornece um nome que não atende aos critérios de limite de caracteres.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o nome deve ter entre 3 e 100 caracteres.

4. **Fluxo Alternativo 4: Preço inválido**
    - **Passo 1:** O administrador fornece um preço que é igual ou menor que zero.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o preço deve ser maior que zero.

5. **Fluxo Alternativo 5: Categoria ou ingredientes inválidos**
    - **Passo 1:** O administrador fornece um ID de categoria ou de ingrediente que não existe no sistema.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que a categoria ou os ingredientes não foram encontrados.

### Caso de Uso: Deletar Produto

**Ator(es):**

- Administradores

**Objetivo:**

- O administrador deseja remover um produto do catálogo do sistema.

#### Fluxo Principal

1. **O administrador acessa a funcionalidade de exclusão de produto.**
    - O administrador fornece o ID do produto que deseja excluir.

2. **O sistema valida o ID do produto.**
    - O sistema verifica se o ID fornecido corresponde a um produto existente.
    - Se o ID não existir, o sistema exibe uma mensagem de erro e encerra o fluxo.

3. **O sistema verifica a associação do produto com pedidos.**
    - O sistema verifica se o produto está associado a algum pedido ativo.
    - Se o produto estiver associado a um pedido, o sistema exibe uma mensagem de erro informando que a exclusão não é permitida.

4. **O sistema exclui o produto.**
    - O sistema remove o produto do catálogo.

5. **O sistema confirma a exclusão.**
    - O sistema retorna uma mensagem de confirmação informando que o produto foi excluído com sucesso.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: ID do produto não existe**
    - **Passo 1:** O administrador fornece um ID de produto que não está cadastrado.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o ID não corresponde a nenhum produto.

2. **Fluxo Alternativo 2: Produto associado a pedidos ativos**
    - **Passo 1:** O administrador tenta excluir um produto que está associado a um pedido ativo.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que a exclusão não é permitida enquanto o produto estiver associado a pedidos.

### Caso de Uso: Obter Configurações do Sistema

**Ator(es):**

- Administradores

**Objetivo:**

- O administrador deseja visualizar as configurações atuais do sistema.

#### Fluxo Principal

1. **O administrador acessa a funcionalidade de visualização de configurações.**
    - O sistema não requer entradas específicas.

2. **O sistema verifica as permissões do administrador.**
    - Se o usuário não tiver perfil de administrador, o sistema exibe uma mensagem de erro e encerra o fluxo.

3. **O sistema recupera as configurações atuais.**
    - O sistema obtém todas as configurações, incluindo:
        - Aceitar pedidos automaticamente (booleano).
        - Número máximo de pedidos que podem ser aceitos automaticamente (número inteiro).
        - Tempo de entrega estimado em minutos (número inteiro).
        - Taxa de entrega (valor decimal).

4. **O sistema retorna as configurações.**
    - O sistema apresenta as configurações atuais ao administrador.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: Acesso negado a usuário não administrador**
    - **Passo 1:** O usuário tenta acessar as configurações sem ser um administrador.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o acesso é restrito a administradores.

2. **Fluxo Alternativo 2: Configurações não definidas**
    - **Passo 1:** O sistema recupera as configurações e verifica se alguma delas não está definida.
    - **Passo 2:** O sistema retorna valores padrão para as configurações ausentes, conforme definido nas regras de negócio:
        - Aceitar pedidos automaticamente: `false`
        - Número máximo de pedidos: `5`
        - Tempo de entrega: `30 minutos`
        - Taxa de entrega: `0.00`

### Caso de Uso: Atualizar Configurações do Sistema

**Ator(es):**

- Administradores

**Objetivo:**

- O administrador deseja atualizar as configurações gerais do sistema.

#### Fluxo Principal

1. **O administrador acessa a funcionalidade de atualização de configurações.**
    - O sistema solicita as entradas necessárias.

2. **O administrador fornece as entradas:**
    - Aceitar pedidos automaticamente (booleano).
    - Número máximo de pedidos aceitos automaticamente (número inteiro).
    - Tempo de entrega estimado em minutos (número inteiro).
    - Taxa de entrega (valor decimal).

3. **O sistema verifica as permissões do administrador.**
    - Se o usuário não tiver perfil de administrador, o sistema exibe uma mensagem de erro e encerra o fluxo.

4. **O sistema valida as entradas fornecidas.**
    - Se alguma entrada não atender às regras de validação, o sistema exibe uma mensagem de erro correspondente e solicita correção.

5. **O sistema atualiza as configurações.**
    - As novas configurações são aplicadas imediatamente, sem necessidade de reiniciar o sistema.

6. **O sistema retorna uma confirmação de atualização.**
    - O administrador recebe uma mensagem confirmando que as configurações foram atualizadas com sucesso.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: Acesso negado a usuário não administrador**
    - **Passo 1:** O usuário tenta acessar a funcionalidade sem ser um administrador.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o acesso é restrito a administradores.

2. **Fluxo Alternativo 2: Validação de entradas falha**
    - **Passo 1:** O administrador fornece entradas inválidas.
    - **Passo 2:** O sistema exibe mensagens de erro específicas para cada entrada inválida, como:
        - Para "aceitar pedidos automaticamente": deve ser `true` ou `false`.
        - Para "número máximo de pedidos": deve ser um número inteiro positivo, não inferior a 1.
        - Para "tempo de entrega estimado": deve ser maior que 0.
        - Para "taxa de entrega": deve ser um valor decimal positivo.

### Caso de Uso: Obter Todos os Pedidos

**Ator(es):**

- Administradores

**Objetivo:**

- O administrador deseja obter uma lista paginada de todos os pedidos registrados no sistema.

#### Fluxo Principal

1. **O administrador acessa a funcionalidade de listar pedidos.**
    - O sistema solicita as entradas necessárias (parâmetros de consulta).

1. **O administrador fornece os parâmetros de consulta:**
    - `PageNumber` (opcional, padrão = 1).
    - `PageSize` (opcional, padrão = 10).
    - `Status` (opcional).

2. **O sistema verifica as permissões do administrador.**
    - Se o usuário não tiver perfil de administrador, o sistema exibe uma mensagem de erro e encerra o fluxo.

3. **O sistema busca os pedidos no banco de dados.**
    - Filtra os pedidos de acordo com o status, se fornecido.

4. **O sistema retorna uma lista paginada de pedidos.**
    - Inclui informações básicas: `Id`, `CustomerName`, `ShippingAddress`, `Total`, `Status`, e `OrderDate`.

5. **O sistema retorna a lista paginada de pedidos.**
    - O administrador recebe a lista com as informações solicitadas.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: Acesso negado a usuário não administrador**
    - **Passo 1:** O usuário tenta acessar a funcionalidade sem ser um administrador.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o acesso é restrito a administradores.

2. **Fluxo Alternativo 3: Filtro pelo status do pedido**
    - **Passo 1:** O administrador fornece um status específico para filtrar os pedidos.
    - **Passo 2:** O sistema retorna apenas os pedidos que correspondem ao status fornecido.

3. **Fluxo Alternativo 4: Sem pedidos para exibir**
    - **Passo 1:** O sistema não encontra pedidos correspondentes aos filtros aplicados.
    - **Passo 2:** O sistema retorna uma mensagem informativa indicando que não há pedidos para exibir.

### Caso de Uso: Obter Detalhes do Pedido

**Ator(es):**

- Administradores

**Objetivo:**

- O administrador deseja visualizar os detalhes completos de um pedido específico para auxiliar na preparação na cozinha.

#### Fluxo Principal

1. **O administrador navega até a lista de pedidos.**
    - O sistema exibe uma lista paginada de pedidos com informações resumidas.

2. **O administrador clica em um card correspondente ao pedido desejado.**
    - O sistema solicita os detalhes do pedido selecionado.

3. **O sistema valida as permissões do administrador.**
    - Se o usuário não tiver perfil de administrador, o sistema exibe uma mensagem de erro e encerra o fluxo.

4. **O sistema busca os detalhes do pedido no banco de dados.**
    - Inclui informações sobre o cliente, endereço, itens do pedido e status.

5. **O sistema gera a resposta com os detalhes completos do pedido.**
    - Inclui: `Id`, `Customer`, `ShippingAddress`, `Total`, `Items`, `Status`, e `OrderDate`.

6. **O sistema retorna os detalhes do pedido ao administrador.**
    - O administrador recebe as informações solicitadas para facilitar a preparação do pedido.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: Acesso negado a usuário não administrador**
    - **Passo 1:** O usuário tenta acessar a funcionalidade sem ser um administrador.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o acesso é restrito a administradores.

2. **Fluxo Alternativo 2: Pedido não encontrado**
    - **Passo 1:** O sistema não encontra o pedido selecionado.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o pedido não foi encontrado.

### Caso de Uso: Atualizar Status do Pedido

**Ator(es):**

- Administradores

**Objetivo:**

- O administrador deseja atualizar o status de um pedido para refletir seu andamento e informar o cliente sobre as mudanças.

#### Fluxo Principal

1. **O administrador navega até a lista de pedidos.**
    - O sistema exibe uma lista paginada de pedidos com informações resumidas.

2. **O administrador clica em um card correspondente ao pedido que deseja atualizar.**
    - O sistema exibe os detalhes do pedido selecionado.

3. **O administrador localiza a opção para atualizar o status do pedido.**
    - O sistema apresenta uma lista suspensa ou botões para selecionar o novo status.

4. **O administrador seleciona o novo status do pedido.**

5. **O administrador confirma a atualização.**
    - O sistema valida as permissões do administrador.

6. **O sistema valida se o novo status é válido.**
    - Se o status não for válido, o sistema exibe uma mensagem de erro e retorna ao passo 3.

7. **O sistema atualiza o status do pedido no banco de dados.**

8. **O sistema notifica o cliente sobre a atualização do status em tempo real.**

9. **O sistema retorna uma confirmação ao administrador.**
    
    - Inclui uma mensagem de sucesso informando que o status foi atualizado com êxito.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: Acesso negado a usuário não administrador**
    - **Passo 1:** O usuário tenta acessar a funcionalidade sem ser um administrador.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o acesso é restrito a administradores.

2. **Fluxo Alternativo 2: Status inválido**
    - **Passo 1:** O administrador seleciona um status que não está definido no enumerador.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o status selecionado não é válido.

3. **Fluxo Alternativo 3: Falha na notificação ao cliente**
    - **Passo 1:** O sistema atualiza o status do pedido com sucesso.
    - **Passo 2:** O sistema falha ao enviar a notificação ao cliente.
    - **Passo 3:** O sistema registra a falha e exibe uma mensagem ao administrador informando que a notificação não pôde ser enviada.

### Caso de Uso: Cancelar Pedido

**Ator(es):**

- Administradores

**Objetivo:**

- O administrador deseja cancelar um pedido, definindo seu status como "Cancelado pelo sistema" e iniciando o processo de reembolso.

#### Fluxo Principal

1. **O administrador navega até a lista de pedidos.**
    - O sistema exibe uma lista paginada de pedidos com informações resumidas.

2. **O administrador clica em um card correspondente ao pedido que deseja cancelar.**
    - O sistema exibe os detalhes do pedido selecionado.

3. **O administrador localiza a opção para cancelar o pedido.**

4. **O administrador confirma o cancelamento.**
    - O sistema solicita, opcionalmente, a razão do cancelamento.

5. **O administrador insere a razão do cancelamento (opcional) e confirma.**
6. **O sistema valida as permissões do administrador.**
7. **O sistema atualiza o status do pedido para "Cancelado pelo sistema".**

8. **O sistema inicia automaticamente o processo de reembolso para o cliente.**

9. **O sistema retorna uma confirmação ao administrador.**
    - Inclui uma mensagem de sucesso informando que o pedido foi cancelado e que o reembolso foi iniciado.

#### Fluxos Alternativos

1. **Fluxo Alternativo: Razão do cancelamento não fornecida**
    - **Passo 1:** O administrador opta por não fornecer uma razão ao cancelar o pedido.
    - **Passo 2:** O sistema continua o processo de cancelamento sem a razão e registra a ação.

### Caso de Uso: Notificação em Tempo Real sobre Novos Pedidos e Atualizações para a Cozinha

**Ator(es):**

- Equipe da cozinha

**Objetivo:**

- A equipe da cozinha deseja ser notificada em tempo real sobre novos pedidos e atualizações de status para otimizar o processo de preparação.

#### Fluxo Principal

1. **Um novo pedido é criado no sistema.**
    - O sistema armazena as informações do pedido.

2. **O sistema gera uma notificação automática.**
    - O sistema compõe a notificação com as seguintes informações:
        - `Title`: "Novo Pedido Recebido"
        - `Message`: "Pedido #1234 - Cliente João - Preparação pendente."
        - `Timestamp`: Data e hora atual.

3. **A notificação é enviada à equipe da cozinha.**
4. **A equipe da cozinha recebe a notificação em tempo real.**
5. **A equipe da cozinha inicia a preparação do pedido.**

### Caso de Uso: Adicionar um Produto/Item ao Carrinho

**Ator(es):**

- Cliente

**Objetivo:**

- Permitir que o cliente adicione produtos ao carrinho de forma personalizada.

#### Fluxo Principal

1. **O cliente acessa a página do produto.**
    - O sistema exibe as informações do produto, incluindo preço, descrição e opções de personalização.

2. **O cliente especifica a quantidade desejada.**
    - O sistema valida a quantidade informada.

3. **O cliente seleciona adicionais.**
    - O sistema apresenta as opções de adicionais disponíveis.

4. **O cliente escolhe ingredientes a serem removidos.**
    - O sistema permite que o cliente remova ingredientes não desejados.

5. **O cliente clica em "Adicionar ao Carrinho".**
    - O sistema processa a solicitação.

6. **O sistema atualiza o carrinho.**
    - O item é adicionado ao carrinho com a quantidade, adicionais e ingredientes removidos.

7. **O sistema exibe uma confirmação de adição ao carrinho.**

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: Quantidade Inválida**
    - **Passo 1:** O cliente informa uma quantidade negativa ou zero.
    - **Passo 2:** O sistema exibe uma mensagem de erro: "A quantidade deve ser um número inteiro positivo."

2. **Fluxo Alternativo 3: Remoção de Ingredientes Obrigatórios**
    - **Passo 1:** O cliente tenta remover um ingrediente que é obrigatório.
    - **Passo 2:** O sistema exibe uma mensagem de erro: "Este ingrediente não pode ser removido."

### Caso de Uso: Cliente Visualizar seu Carrinho

**Ator(es):**

- Cliente

**Objetivo:**

- Permitir que o cliente visualize o conteúdo atualizado do seu carrinho, incluindo detalhes de cada item e o total do pedido.

#### Fluxo Principal

1. **O cliente acessa a opção "Carrinho" no sistema.**
    - O sistema verifica se o cliente está autenticado e possui perfil de **cliente**.
    - Se o cliente não estiver autenticado, o sistema redireciona para a tela de login.

2. **O cliente solicita a visualização do carrinho.**
    - O sistema recupera o ID do carrinho associado ao cliente autenticado.

3. **O sistema recupera os detalhes do carrinho.**
    - O sistema busca os itens contidos no carrinho, incluindo quantidades, preços e personalizações.

4. **O sistema exibe os itens no carrinho.**
    - Cada item inclui:
        - **ID do produto**
        - **Título**
        - **URL da imagem**
        - **Preço unitário**
        - **Quantidade**

5. **O sistema calcula o total do pedido.**
    - O total é baseado nas quantidades e personalizações de cada item.

6. **O sistema exibe o total do carrinho.**
    - O cliente visualiza o valor total do pedido, considerando todos os itens e modificações.

7. **O cliente pode optar por editar ou finalizar o pedido.**
    - O sistema oferece opções para o cliente ajustar itens ou continuar para a finalização do pedido.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: Carrinho Vazio**
    - **Passo 1:** O cliente acessa o carrinho, mas ele está vazio.
    - **Passo 2:** O sistema exibe uma mensagem: "Seu carrinho está vazio."
    - **Passo 3:** O sistema sugere que o cliente navegue pelo catálogo de produtos para adicionar itens ao carrinho.

2. **Fluxo Alternativo 2: Erro ao Recuperar o Carrinho**
    - **Passo 1:** O sistema encontra um problema ao recuperar o carrinho do cliente (ex: falha na conexão com o banco de dados).
    - **Passo 2:** O sistema exibe uma mensagem de erro: "Houve um problema ao carregar seu carrinho. Tente novamente mais tarde."

3. **Fluxo Alternativo 3: Cliente Não Autenticado**
    - **Passo 1:** O cliente tenta visualizar o carrinho sem estar autenticado.
    - **Passo 2:** O sistema redireciona o cliente para a página de login.

### Caso de Uso: Atualizar a Quantidade de um Item no Carrinho

**Ator(es):**

- Cliente

**Objetivo:**

- Permitir que o cliente ajuste a quantidade de um item específico no carrinho, com recalculação automática do preço total.

#### Fluxo Principal

1. **O cliente acessa o carrinho.**
    - O sistema verifica se o cliente está autenticado.
    - Se o cliente não estiver autenticado, o sistema redireciona para a tela de login.

2. **O cliente seleciona o item que deseja atualizar.**
    - O sistema exibe a lista de itens no carrinho, com suas quantidades atuais.

3. **O cliente insere a nova quantidade para o item.**
    - A nova quantidade é inserida pelo cliente no campo correspondente ao item no carrinho.

4. **O cliente confirma a atualização.**
    - O sistema recebe a nova quantidade informada e valida a entrada.

5. **O sistema valida a nova quantidade.**
    - Se a quantidade for menor que 1 ou não for um número válido:
        - O sistema exibe uma mensagem de erro: "Quantidade inválida. Insira um valor positivo."
    - Se a quantidade for válida, o sistema prossegue com a atualização.

6. **O sistema atualiza a quantidade do item no carrinho.**
    - O sistema altera a quantidade do item no banco de dados associado ao carrinho do cliente.

7. **O sistema recalcula o preço total do carrinho.**
    - O preço total do carrinho é ajustado automaticamente com base na nova quantidade do item.

8. **O sistema exibe a atualização ao cliente.**
    - O sistema exibe uma confirmação de que a quantidade foi alterada com sucesso.
    - O carrinho é atualizado com a nova quantidade e o preço total recalculado.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: Quantidade Inválida**
    - **Passo 1:** O cliente tenta inserir uma quantidade inválida (ex: menor que 1 ou um valor não numérico).
    - **Passo 2:** O sistema exibe uma mensagem de erro: "Quantidade inválida. Insira um valor positivo."

2. **Fluxo Alternativo 2: Erro ao Atualizar a Quantidade**
    - **Passo 1:** O sistema encontra um problema ao atualizar a quantidade no banco de dados (ex: falha na conexão).
    - **Passo 2:** O sistema exibe uma mensagem de erro: "Houve um problema ao atualizar a quantidade. Tente novamente mais tarde."

### Caso de Uso: Incrementar e Decrementar a Quantidade de um Item no Carrinho

**Ator(es):**

- Cliente

**Objetivo:**

- Permitir que o cliente ajuste a quantidade de um item no carrinho incrementando ou decrementando a quantidade em um (1) por vez.

#### Fluxo Principal

1. **O cliente acessa o carrinho.**
    - O sistema verifica se o cliente está autenticado.

2. **O cliente seleciona o item no carrinho que deseja ajustar.**
    - O sistema exibe a lista de itens no carrinho, incluindo a quantidade atual de cada item.

3. **O cliente escolhe a ação (incrementar ou decrementar) para o item.**
    - O cliente pressiona o botão de incremento (+) ou decremento (-) associado ao item.

4. **O sistema valida a ação.**
    - Se a ação for de incremento, o sistema aumenta a quantidade do item em 1.
    - Se a ação for de decremento:
        - Se a quantidade do item for maior que 1, o sistema diminui a quantidade em 1.
        - Se a quantidade for 1, o item é removido do carrinho.

5. **O sistema atualiza a quantidade do item no carrinho.**
    - A nova quantidade do item é atualizada no banco de dados.

6. **O sistema recalcula o preço total do carrinho.**
    - O sistema ajusta o total do carrinho automaticamente com base na nova quantidade do item.

7. **O sistema exibe o carrinho atualizado ao cliente.**
    - O carrinho é atualizado com a nova quantidade, ou o item é removido se a quantidade for zero, e o preço total recalculado é exibido.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: Tentativa de Decremento com Quantidade Igual a 1**
    - **Passo 1:** O cliente tenta decrementar a quantidade de um item que está com quantidade igual a 1.
    - **Passo 2:** O sistema remove o item do carrinho.

2. **Fluxo Alternativo 2: Erro ao Atualizar Quantidade**
    - **Passo 1:** O sistema encontra um erro ao atualizar a quantidade no banco de dados (ex: falha na conexão).
    - **Passo 2:** O sistema exibe uma mensagem de erro: "Houve um problema ao ajustar a quantidade. Tente novamente mais tarde."

### **Caso de Uso: Deletar um Item do Carrinho**

**Ator(es):**

- Cliente

**Objetivo:**

- Permitir que o cliente remova um item específico do seu carrinho, atualizando automaticamente o total do pedido.

#### Fluxo Principal

1. **O cliente acessa o carrinho.**
    - O sistema verifica se o cliente está autenticado.

2. **O cliente seleciona o item a ser removido.**
    - O sistema exibe a lista de itens no carrinho, permitindo que o cliente selecione qual item deseja remover.

3. **O cliente solicita a remoção do item.**
    - O cliente clica no botão de remoção ao lado do item desejado.

4. **O sistema valida a solicitação.**
    - O sistema verifica se o ID do item selecionado existe no carrinho do cliente.
    - Se o item não for encontrado, o sistema exibe uma mensagem de erro: "Item não encontrado no carrinho."

5. **O sistema remove o item do carrinho.**
    - O item é removido do banco de dados e do carrinho virtual do cliente.

6. **O sistema recalcula o total do carrinho.**
    - O preço total do carrinho é atualizado com base nos itens restantes.

7. **O sistema exibe o carrinho atualizado ao cliente.**
    - O carrinho é exibido sem o item removido, e o novo total do pedido é mostrado.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: Item Não Encontrado**
    - **Passo 1:** O sistema não encontra o item no carrinho.
    - **Passo 2:** O sistema exibe a mensagem: "Item não encontrado no carrinho."

2. **Fluxo Alternativo 2: Erro ao Remover o Item**
    - **Passo 1:** O sistema encontra um erro ao tentar remover o item (ex: falha na conexão com o banco de dados).
    - **Passo 2:** O sistema exibe uma mensagem de erro: "Houve um problema ao remover o item. Tente novamente mais tarde."

### **Caso de Uso: Checkout/Pagamento**

**Ator(es):**

- Cliente

**Objetivo:**

- Permitir que o cliente finalize o pedido realizando o pagamento via Stripe, fornecendo o ID do endereço de entrega selecionado a partir de uma lista de endereços cadastrados.

#### Fluxo Principal


1. **O sistema exibe a lista de endereços do cliente.**
    - O sistema consulta os endereços cadastrados pelo cliente.
    - A lista de endereços é exibida, com as seguintes informações:
        - ID do endereço
        - Rua, número, bairro, cidade, estado, e CEP
    - Se o cliente não tiver endereços cadastrados, o sistema exibe uma mensagem: "Nenhum endereço cadastrado. Adicione um novo endereço."

2. **O cliente seleciona o endereço de entrega.**
    - O cliente escolhe um dos endereços disponíveis da lista.
    - Se o cliente quiser adicionar um novo endereço, ele pode optar por cadastrar um novo antes de continuar com o checkout.

3. **O cliente solicita a finalização do pedido.**
    - Após selecionar o endereço de entrega, o cliente confirma a seleção e solicita o checkout.

4. **O sistema valida o ID do endereço selecionado.**
    - O sistema verifica se o ID do endereço fornecido pertence ao cliente e é válido.
    - Se o ID do endereço não for válido ou não pertencer ao cliente, o sistema exibe uma mensagem de erro: "Endereço inválido. Selecione um endereço válido."

5. **O sistema cria a sessão de pagamento no Stripe.**
    - O sistema gera uma sessão de checkout no Stripe com base no conteúdo do carrinho e no endereço de entrega selecionado.

    - O sistema armazena temporariamente as informações do pedido como "Pendente de pagamento".

6. **O sistema retorna a URL de pagamento.**
    - O sistema retorna a URL gerada pelo Stripe, permitindo que o cliente seja redirecionado para o Stripe e complete o pagamento.

7. **O cliente é redirecionado para o Stripe.**
    - O cliente clica no botão e é redirecionado para o Stripe, onde ele pode fornecer as informações do cartão de crédito e realizar o pagamento.

8. **O sistema recebe a confirmação de pagamento do Stripe.**
    - Após o pagamento bem-sucedido, o Stripe redireciona o cliente para a página de confirmação no sistema.

9. **O sistema atualiza o status do pedido.**
    - O sistema muda o status do pedido para "Pagamento confirmado" e encaminha o pedido para processamento (por exemplo, para a cozinha ou setor de entregas, conforme aplicável).

10. **O sistema exibe a confirmação do pedido ao cliente.**
    - O cliente visualiza a confirmação do pedido, com detalhes do pedido, valor total pago e o tempo estimado para entrega.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: Cliente Sem Endereços Cadastrados**
    - **Passo 1:** O cliente tenta prosseguir para o checkout, mas não tem nenhum endereço cadastrado.
    - **Passo 2:** O sistema exibe uma mensagem: "Nenhum endereço cadastrado. Adicione um novo endereço."
    - **Passo 3:** O cliente é redirecionado para a página de cadastro de novo endereço antes de continuar o processo de checkout.

2. **Fluxo Alternativo 2: Cancelamento do Pagamento no Stripe**
    - **Passo 1:** O cliente acessa a página de pagamento no Stripe, mas decide cancelar a transação.
    - **Passo 2:** O Stripe redireciona o cliente para a página de cancelamento no sistema, informando que o pagamento foi cancelado e o pedido não foi processado.

### **Caso de Uso: Obter Todos os Endereços do Cliente**

**Ator(es):**

- Cliente

**Objetivo:**

- Permitir que o cliente visualize todos os endereços cadastrados no sistema, facilitando o gerenciamento para o processo de checkout e entrega.

#### Fluxo Principal

1. **O cliente solicita a visualização dos endereços.**
    - O cliente acessa a opção de visualizar endereços no menu.

2. **O sistema verifica a autenticação do cliente.**
    - O sistema valida se o cliente está autenticado.
    - Se o cliente não estiver autenticado, o sistema redireciona para a tela de login.
3. **O sistema consulta os endereços cadastrados.**
    
    - O sistema busca no banco de dados todos os endereços associados ao cliente autenticado.

4. **O sistema exibe a lista de endereços.**
    - O sistema apresenta a lista com os seguintes detalhes para cada endereço:
        - ID do endereço
        - CEP
        - Número (opcional)
        - Rua
        - Bairro
        - Cidade
        - Estado
        - Complemento (opcional)
        - Referência (opcional)
    - Se o cliente não tiver endereços cadastrados, o sistema exibe uma mensagem: "Nenhum endereço cadastrado. Adicione um novo endereço."

5. **O cliente visualiza os endereços.**
    - O cliente pode revisar seus endereços e decidir se deseja editar ou adicionar novos.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: Cliente Não Autenticado**
    - **Passo 1:** O cliente tenta acessar a lista de endereços sem estar autenticado.
    - **Passo 2:** O sistema redireciona o cliente para a tela de login.
    - **Passo 3:** Após o login, o cliente é redirecionado novamente para a lista de endereços.

2. **Fluxo Alternativo 2: Cliente Sem Endereços Cadastrados**
    - **Passo 1:** O cliente acessa a lista de endereços, mas não possui nenhum endereço cadastrado.
    - **Passo 2:** O sistema exibe a mensagem: "Nenhum endereço cadastrado. Adicione um novo endereço."
    - **Passo 3:** O cliente é oferecido a opção de adicionar um novo endereço.

3. **Fluxo Alternativo 3: Erro ao Consultar Endereços**
    - **Passo 1:** O sistema encontra um erro ao tentar recuperar os endereços do banco de dados (ex: falha na conexão).
    - **Passo 2:** O sistema exibe uma mensagem de erro: "Houve um problema ao carregar seus endereços. Tente novamente mais tarde."

### **Caso de Uso: Registrar um Novo Endereço do Cliente**

**Ator(es):**

- Cliente

**Objetivo:**

- Permitir que o cliente adicione um novo endereço ao seu perfil, completando automaticamente os detalhes com base no CEP fornecido.

#### Fluxo Principal

1. **O cliente acessa a opção de registrar um novo endereço.**
    - O cliente navega até a seção de endereços em seu perfil.

2. **O sistema verifica a autenticação do cliente.**
    - O sistema valida se o cliente está autenticado.
    - Se o cliente não estiver autenticado, o sistema redireciona para a tela de login.

3. **O cliente fornece o CEP.**
    - O cliente insere o CEP no campo correspondente.

4. **O sistema valida o CEP.**
    - O sistema consulta um serviço de geolocalização ou base de dados para verificar a validade do CEP.
    - Se o CEP for inválido, o sistema exibe uma mensagem de erro: "CEP inválido. Verifique e tente novamente."

5. **O sistema completa os detalhes do endereço.**
    - Com base no CEP válido, o sistema preenche automaticamente os campos de endereço (rua, bairro, cidade, estado).

6. **O cliente insere informações adicionais (opcionais).**
    - O cliente pode fornecer o número, referência e complemento, se desejar.

7. **O cliente confirma o registro do endereço.**
    - O cliente clica no botão de registrar para salvar o novo endereço.

8. **O sistema armazena o novo endereço.**
    - O sistema salva as informações do endereço no banco de dados.

9. **O sistema retorna a confirmação do cadastro.**
    - O sistema exibe uma mensagem de sucesso: "Endereço registrado com sucesso."
    - O sistema fornece o ID do novo endereço registrado.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: CEP Inválido**
    - **Passo 1:** O cliente insere um CEP inválido.
    - **Passo 2:** O sistema exibe uma mensagem de erro: "CEP inválido. Verifique e tente novamente."
    - **Passo 3:** O cliente é solicitado a corrigir o CEP.

2. **Fluxo Alternativo 2: Cliente Não Autenticado**

    - **Passo 1:** O cliente tenta acessar a página de registro de endereço sem estar autenticado.
    - **Passo 2:** O sistema redireciona o cliente para a tela de login.
    - **Passo 3:** Após o login, o cliente é redirecionado para a página de registro de endereço.

3. **Fluxo Alternativo 3: Erro ao Salvar Endereço**
    - **Passo 1:** O sistema encontra um erro ao tentar salvar o novo endereço no banco de dados (ex: falha na conexão).
    - **Passo 2:** O sistema exibe uma mensagem de erro: "Houve um problema ao registrar seu endereço. Tente novamente mais tarde."

### **Caso de Uso: Registrar um Novo Endereço do Cliente**

**Ator(es):**

- Cliente

**Objetivo:**

- Permitir que o cliente adicione um novo endereço ao seu perfil, completando automaticamente os detalhes com base no CEP fornecido.

#### Fluxo Principal

1. **O cliente acessa a opção de registrar um novo endereço.**
    - O cliente navega até a seção de endereços em seu perfil.

2. **O sistema verifica a autenticação do cliente.**
    - O sistema valida se o cliente está autenticado.
    - Se o cliente não estiver autenticado, o sistema redireciona para a tela de login.

3. **O cliente fornece o CEP.**
    - O cliente insere o CEP no campo correspondente (obrigatório).

4. **O sistema valida o CEP.**
    - O sistema consulta um serviço de geolocalização via CEP para verificar a validade do CEP e obter os detalhes do endereço.
    - Se o CEP for inválido, o sistema exibe uma mensagem de erro: "CEP inválido. Verifique e tente novamente."

5. **O sistema completa os detalhes do endereço.**
    - Com base no CEP válido, o sistema preenche automaticamente os campos de endereço (rua, bairro, cidade, estado).

6. **O cliente insere informações adicionais (opcionais).**
    - O cliente pode fornecer o número (opcional), referência (opcional) e complemento (opcional), se desejar.

7. **O cliente confirma o registro do endereço.**
    - O cliente clica no botão de registrar para salvar o novo endereço.

8. **O sistema armazena o novo endereço.**
    - O sistema salva as informações do endereço no banco de dados.

9. **O sistema retorna a confirmação do cadastro.**
    - O sistema exibe uma mensagem de sucesso: "Endereço registrado com sucesso."
    - O sistema fornece o ID do novo endereço registrado.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: CEP Inválido**
    - **Passo 1:** O cliente insere um CEP inválido.
    - **Passo 2:** O sistema exibe uma mensagem de erro: "CEP inválido. Verifique e tente novamente."
    - **Passo 3:** O cliente é solicitado a corrigir o CEP.

2. **Fluxo Alternativo 2: Cliente Não Autenticado**
    - **Passo 1:** O cliente tenta acessar a página de registro de endereço sem estar autenticado.
    - **Passo 2:** O sistema redireciona o cliente para a tela de login.
    - **Passo 3:** Após o login, o cliente é redirecionado para a página de registro de endereço.

3. **Fluxo Alternativo 3: Erro ao Salvar Endereço**
    - **Passo 1:** O sistema encontra um erro ao tentar salvar o novo endereço no banco de dados (ex: falha na conexão).
    - **Passo 2:** O sistema exibe uma mensagem de erro: "Houve um problema ao registrar seu endereço. Tente novamente mais tarde."

### **Caso de Uso: Deletar um Endereço**

**Ator(es):**

- Cliente

**Objetivo:**

- Permitir que o cliente exclua um endereço previamente cadastrado, garantindo que apenas o cliente que o cadastrou possa realizá-la.


#### Fluxo Principal

1. **O cliente acessa a opção de gerenciar endereços.**
    - O cliente navega até a seção de endereços em seu perfil.

2. **O cliente seleciona o endereço a ser excluído.**
    - O cliente visualiza a lista de endereços cadastrados e escolhe o endereço desejado.

3. **O sistema verifica a autenticação do cliente.**
    - O sistema valida se o cliente está autenticado.
    - Se o cliente não estiver autenticado, o sistema redireciona para a tela de login.

4. **O sistema verifica a propriedade do endereço.**
    - O sistema valida se o cliente que está tentando excluir o endereço é o mesmo que o cadastrou.
    - Se o cliente não for o proprietário, o sistema exibe uma mensagem de erro: "Você não tem permissão para excluir este endereço."

5. **O cliente confirma a exclusão do endereço.**
    - O cliente clica no botão de confirmar exclusão.

6. **O sistema exclui o endereço.**
    - O sistema remove as informações do endereço do banco de dados.

7. **O sistema retorna a confirmação da exclusão.**
    - O sistema exibe uma mensagem de sucesso: "Endereço excluído com sucesso."

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: Cliente Não Autenticado**
    - **Passo 1:** O cliente tenta acessar a opção de deletar um endereço sem estar autenticado.

### **Caso de Uso: Obter Todos os Pedidos Atuais do Cliente**

**Ator(es):**

- Cliente

**Objetivo:**

- Permitir que o cliente visualize todos os pedidos atuais que estão com status pendente, confirmado ou em preparo, a fim de acompanhar o progresso desses pedidos.

#### Pré-condições

- O cliente deve estar autenticado para visualizar seus pedidos.

#### Fluxo Principal

1. **O cliente acessa a opção de visualizar pedidos atuais.**
    - O cliente navega até a seção de pedidos em seu perfil.

2. **O sistema verifica a autenticação do cliente.**
    - O sistema valida se o cliente está autenticado.
    - Se o cliente não estiver autenticado, o sistema redireciona para a tela de login.

3. **O sistema recupera os pedidos atuais do cliente.**
    - O sistema busca todos os pedidos do cliente com status "pendente", "confirmado" ou "em preparo".

4. **O sistema exibe a lista de pedidos atuais.**
    - O sistema exibe ao cliente a lista de pedidos que ainda estão em processo, incluindo:
        - **ID do pedido**
        - **Data do pedido**
        - **Status do pedido** (pendente, confirmado, em preparo)
        - **Total do pedido**

#### Fluxos Alternativos

1. **Fluxo Alternativo 2: Cliente Sem Pedidos Atuais**
    - **Passo 1:** O cliente acessa a página de pedidos, mas não possui nenhum pedido pendente, confirmado ou em preparo.
    - **Passo 2:** O sistema exibe uma mensagem: "Você não possui pedidos atuais no momento."

### **Caso de Uso: Obter Detalhes do Pedido**

**Ator(es):**

- Cliente

**Objetivo:**

- Permitir que o cliente visualize os detalhes completos de um pedido específico, incluindo itens, adicionais, ingredientes removidos e informações de entrega.

#### Pré-condições

- O cliente deve estar autenticado para visualizar os detalhes do pedido.

#### Fluxo Principal

1. **O cliente acessa a página de pedidos.**
    - O cliente navega até a seção de pedidos em seu perfil e visualiza uma lista de pedidos.

2. **O cliente seleciona um pedido para visualizar os detalhes.**
    - O cliente clica em um **card** de um dos pedidos exibidos na lista.

3. **O sistema verifica a autenticação do cliente.**
    - O sistema valida se o cliente está autenticado.
    - Se o cliente não estiver autenticado, o sistema redireciona para a tela de login.

4. **O sistema valida o pedido.**
    - O sistema garante que o pedido existe e que pertence ao cliente autenticado.

5. **O sistema exibe os detalhes do pedido.**
    - O sistema exibe as seguintes informações do pedido:
        - **ID do pedido**
        - **Nome completo do cliente**
        - **Endereço completo**, incluindo referência e complemento
        - **Total do pedido**
        - **Status do pedido**
        - **Data do pedido**
        - **Itens do pedido**:
            - **Título do produto**
            - **Quantidade**
            - **Adicionais**
            - **Ingredientes removidos**

#### Fluxos Alternativos


1. **Fluxo Alternativo 2: Pedido Não Encontrado**
    - **Passo 1:** O cliente seleciona um pedido inválido ou inexistente.
    - **Passo 2:** O sistema exibe uma mensagem de erro: "Pedido não encontrado. Verifique e tente novamente."

### **Caso de Uso: Obter Histórico de Pedidos**

**Ator(es):**

- Cliente

**Objetivo:**

- Permitir que o cliente visualize o histórico de seus pedidos passados de forma paginada, facilitando a navegação entre grandes volumes de pedidos.

#### Pré-condições

- O cliente deve estar autenticado e a sessão deve estar válida para acessar o histórico de pedidos.

#### Fluxo Principal

1. **O cliente acessa a página de histórico de pedidos.**
    - O cliente navega até a seção de "Histórico de Pedidos" em seu perfil.

2. **O cliente solicita o histórico de pedidos.**
    
    - O cliente acessa a página e o sistema exibe os parâmetros de paginação, com `PageNumber` definido para 1 (padrão) e `PageSize` para 10 (padrão).

3. **O sistema verifica a autenticação do cliente.**
    - O sistema valida se o cliente está autenticado.
    - Se o cliente não estiver autenticado, o sistema redireciona para a tela de login.

4. **O sistema consulta o histórico de pedidos.**
    - O sistema filtra os pedidos do cliente no banco de dados, organizando-os de forma paginada com base nos parâmetros fornecidos (`PageNumber` e `PageSize`).

5. **O sistema retorna a lista paginada de pedidos.**
    - O sistema exibe a lista de pedidos históricos, contendo:
        - **ID do pedido**
        - **Data do pedido**
        - **Status do pedido**
        - **Total do pedido**

6. **O cliente pode navegar pelas páginas do histórico.**
    - O cliente pode solicitar diferentes páginas do histórico de pedidos ajustando os parâmetros de `PageNumber` e `PageSize` se desejar.

#### Fluxos Alternativos

1. **Fluxo Alternativo 2: Nenhum Pedido no Histórico**
    - **Passo 1:** O cliente acessa o histórico, mas não possui pedidos passados.
    - **Passo 2:** O sistema exibe uma mensagem informativa: "Nenhum pedido encontrado no histórico."

### **Caso de Uso: Cancelar um Pedido**

**Ator(es):**

- Cliente

**Objetivo:**

- Permitir que o cliente cancele um pedido que ainda não foi finalizado e, caso necessário, processe o reembolso automaticamente.

#### Pré-condições

- O cliente deve estar autenticado para cancelar um pedido.
- O pedido deve estar em um estado que permita o cancelamento (pendente ou confirmado).

#### Fluxo Principal

1. **O cliente acessa a página de pedidos.**
    - O cliente navega até a seção de pedidos em seu perfil.

2. **O cliente seleciona o pedido que deseja cancelar.**
    - O cliente escolhe um pedido que está em andamento (status: pendente ou confirmado).

3. **O cliente solicita o cancelamento.**
    - O cliente clica na opção "Cancelar Pedido" e pode opcionalmente fornecer uma razão para o cancelamento.

4. **O sistema valida a autenticidade do cliente.**
    - O sistema verifica se o cliente está autenticado e se o pedido pertence ao cliente.

5. **O sistema valida o estado do pedido.**
    - O sistema checa se o pedido ainda está em um estado que permite o cancelamento (status pendente ou confirmado).
    - Se o pedido já estiver em preparo ou finalizado, o sistema bloqueia o cancelamento e informa ao cliente que a ação não pode ser realizada.

6. **O sistema atualiza o status do pedido.**
    - O status do pedido é atualizado para "Cancelado pelo cliente".

7. **O sistema processa o reembolso.**
    - Se aplicável, o sistema inicia automaticamente o processo de reembolso para o cliente, de acordo com o método de pagamento utilizado.

8. **O sistema registra a razão para o cancelamento.**
    - Se o cliente forneceu uma razão para o cancelamento, o sistema a armazena juntamente com as informações do pedido.

9. **O sistema confirma o cancelamento.**
    - O cliente recebe uma confirmação de que o pedido foi cancelado com sucesso, juntamente com a atualização de status.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: Cliente Não Autenticado**
    - **Passo 1:** O cliente tenta cancelar um pedido sem estar autenticado.
    - **Passo 2:** O sistema redireciona o cliente para a página de login antes de continuar com o processo.

2. **Fluxo Alternativo 2: Pedido Já em Preparo ou Finalizado**
    - **Passo 1:** O cliente tenta cancelar um pedido que já está em preparo ou foi finalizado.
    - **Passo 2:** O sistema exibe uma mensagem informando que o pedido não pode mais ser cancelado.

### **Caso de Uso: Recomendação de Produtos com Base no Histórico de Pedidos**

**Ator(es):**

- Cliente

**Objetivo:**

- Oferecer recomendações personalizadas de produtos ao cliente, baseadas no histórico de pedidos, para incentivar novas compras.

#### Pré-condições

- O cliente deve estar autenticado para acessar as recomendações.
- O cliente deve ter um histórico de pedidos relevante para que o sistema possa gerar recomendações.

#### Fluxo Principal

1. **O cliente acessa o sistema de recomendações.**
    - O cliente navega até a seção de "Recomendações" através da interface de usuário.

2. **O sistema valida o histórico de pedidos do cliente.**
    - O sistema verifica se o cliente possui um histórico de pedidos suficiente para fornecer recomendações.

3. **O sistema analisa o histórico de pedidos.**
    - O sistema avalia os produtos mais comprados ou visualizados, frequência de compras e categorias preferidas.

4. **O sistema gera recomendações personalizadas.**
    - Com base nos dados analisados, o sistema sugere produtos semelhantes ou complementares aos já adquiridos pelo cliente.

### Caso de Uso: Registro de Conta

**Ator(es):**

- Novo Usuário (cliente)

**Objetivo:**

- Permitir que novos usuários se registrem, criando uma conta no sistema.

#### Fluxo Principal

1. **O usuário acessa a página de registro.**
    - Navega até a seção de registro na interface.

2. **O sistema exibe o formulário de registro.**
    - O formulário contém campos para nome completo, e-mail e senha.

3. **O usuário preenche o formulário.**
    - Insere o nome completo, e-mail e senha.

4. **O usuário envia o formulário de registro.**
    - O sistema valida as entradas fornecidas.

5. **O sistema valida o e-mail.**
    - Verifica se o e-mail é único e está em um formato válido.
    - Se o e-mail não for válido, o sistema exibe uma mensagem de erro.

6. **O sistema valida a senha.**
    - Verifica se a senha atende aos critérios de segurança definidos (comprimento mínimo, complexidade).
    - Se a senha não for segura, o sistema exibe uma mensagem de erro.

7. **O sistema valida o nome completo.**
    - Confirma que o nome completo foi fornecido e não está vazio.
    - Se estiver vazio, o sistema exibe uma mensagem de erro.

8. **O sistema registra a nova conta.**
    - Se todas as validações forem bem-sucedidas, a conta é criada e os dados são salvos no banco de dados.

9. **O sistema retorna a confirmação do registro.**
    - Exibe uma mensagem de sucesso informando que a conta foi criada.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: E-mail já existente**
    - **Passo 1:** O sistema detecta que o e-mail já está cadastrado.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o e-mail já está em uso e solicita um e-mail diferente.
    - **Passo 3:** O usuário altera o e-mail e reenvia o formulário ou abandona o registro.

2. **Fluxo Alternativo 2: Senha não atende aos critérios de segurança**
    - **Passo 1:** O sistema detecta que a senha não atende aos critérios de segurança.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que a senha deve ter um comprimento mínimo e complexidade.
    - **Passo 3:** O usuário ajusta a senha e reenvia o formulário ou abandona o registro.

3. **Fluxo Alternativo 3: Nome completo vazio**
    - **Passo 1:** O sistema detecta que o nome completo está vazio.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o nome completo é obrigatório.
    - **Passo 3:** O usuário preenche o nome completo e reenvia o formulário ou abandona o registro.

### Caso de Uso: Redefinição de Senha

**Ator(es):**

- Usuário Registrado

**Objetivo:**

- Permitir que os usuários solicitem a redefinição de senha através do envio de um token para o e-mail registrado.

#### Fluxo Principal

1. **O usuário acessa a página de redefinição de senha.**
    - Navega até a seção onde pode solicitar a redefinição de senha.

2. **O sistema exibe o formulário de solicitação de redefinição.**
    - O formulário contém um campo para o e-mail do usuário.

3. **O usuário preenche o formulário com o e-mail.**
    - Insere o endereço de e-mail associado à sua conta.

4. **O usuário envia o formulário de solicitação.**
    - O sistema valida a entrada fornecida.

5. **O sistema valida o e-mail.**
    - Verifica se o e-mail fornecido está registrado no sistema.
    - Se o e-mail não for encontrado, o sistema exibe uma mensagem de erro.

6. **O sistema gera um token de redefinição.**
    - Um token de 9 caracteres é gerado de forma única.

7. **O sistema envia o token para o e-mail fornecido.**
    - O token é enviado ao endereço de e-mail registrado.

8. **O sistema retorna a confirmação do envio.**
    - Exibe uma mensagem de sucesso informando que o e-mail com o token foi enviado.

#### Fluxos Alternativos

1. **Fluxo Alternativo 1: E-mail não registrado**
    - **Passo 1:** O sistema detecta que o e-mail não está registrado.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o e-mail não foi encontrado.
    - **Passo 3:** O usuário pode tentar novamente com um e-mail diferente ou abandonar a solicitação.

2. **Fluxo Alternativo 2: Erro no envio do e-mail**
    - **Passo 1:** O sistema encontra um erro ao tentar enviar o e-mail.
    - **Passo 2:** O sistema exibe uma mensagem de erro informando que o envio falhou.
    - **Passo 3:** O usuário pode tentar novamente após corrigir o problema ou contatar o suporte.
USE Comanda;
GO

INSERT INTO [Categories] ([Name]) VALUES (N'Bebidas'); --         ID: 1
INSERT INTO [Categories] ([Name]) VALUES (N'Lanches'); --         ID: 2
INSERT INTO [Categories] ([Name]) VALUES (N'Acompanhamentos'); -- ID: 3
INSERT INTO [Categories] ([Name]) VALUES (N'Sobremesas'); --      ID: 4
INSERT INTO [Categories] ([Name]) VALUES (N'Promoções'); --       ID: 5

GO

DECLARE @LanchesCategoryId INT;
DECLARE @BebidasCategoryId INT;

SELECT @LanchesCategoryId = [Id] FROM [Categories] WHERE Name = N'Lanches';
SELECT @BebidasCategoryId = [Id] FROM [Categories] WHERE Name = N'Bebidas';

INSERT INTO [Products] ([Title], [Description], [Price], [CategoryId], [ImagePath]) VALUES
(N'Cheeseburger', N'Hambúrguer com queijo, alface e tomate.', 12.50, @LanchesCategoryId, N'/images/cheeseburger.jpg'),
(N'X-Bacon', N'Hambúrguer com bacon crocante.', 14.00, @LanchesCategoryId, N'/images/x-bacon.jpg'),
(N'Frango Empanado', N'Sanduíche de frango empanado.', 11.50, @LanchesCategoryId, N'/images/frango-empanado.jpg'),
(N'Veggie Burger', N'Hambúrguer vegetariano com legumes frescos.', 13.00, @LanchesCategoryId, N'/images/veggie-burger.jpg'),
(N'Hot Dog', N'Cachorro-quente com salsicha, molho e batata-palha.', 10.00, @LanchesCategoryId, N'/images/hot-dog.jpg'),
(N'Bauru', N'Sanduíche com presunto, queijo e tomate.', 9.50, @LanchesCategoryId, N'/images/bauru.jpg'),
(N'Misto Quente', N'Pão com queijo e presunto tostado.', 8.00, @LanchesCategoryId, N'/images/misto-quente.jpg'),
(N'Wrap de Frango', N'Wrap com frango grelhado e vegetais.', 11.00, @LanchesCategoryId, N'/images/wrap-frango.jpg'),
(N'Tapioca de Queijo', N'Tapioca recheada com queijo.', 7.50, @LanchesCategoryId, N'/images/tapioca-queijo.jpg'),
(N'Açaí na Tigela', N'Açaí com banana, granola e mel.', 15.00, @LanchesCategoryId, N'/images/acai-tigela.jpg');


INSERT INTO [Products] ([Title], [Description], [Price], [CategoryId], [ImagePath]) VALUES
(N'Refrigerante', N'Refrigerante de cola.', 5.00, @BebidasCategoryId, N'/images/refrigerante.jpg'),
(N'Suco de Laranja', N'Suco natural de laranja.', 6.50, @BebidasCategoryId, N'/images/suco-laranja.jpg'),
(N'Água Mineral', N'Água mineral sem gás.', 3.00, @BebidasCategoryId, N'/images/agua-mineral.jpg'),
(N'Cerveja', N'Cerveja artesanal.', 8.00, @BebidasCategoryId, N'/images/cerveja.jpg'),
(N'Milkshake de Chocolate', N'Milkshake cremoso de chocolate.', 10.00, @BebidasCategoryId, N'/images/milkshake-chocolate.jpg'),
(N'Café Expresso', N'Café expresso forte.', 4.50, @BebidasCategoryId, N'/images/cafe-expresso.jpg'),
(N'Chá Gelado', N'Chá gelado de limão.', 5.50, @BebidasCategoryId, N'/images/cha-gelado.jpg'),
(N'Smoothie de Morango', N'Smoothie de morango fresco.', 9.00, @BebidasCategoryId, N'/images/smoothie-morango.jpg'),
(N'Capuccino', N'Café com leite vaporizado e chocolate.', 6.00, @BebidasCategoryId, N'/images/capuccino.jpg'),
(N'Vinho Tinto', N'Taça de vinho tinto.', 12.00, @BebidasCategoryId, N'/images/vinho-tinto.jpg');
GO
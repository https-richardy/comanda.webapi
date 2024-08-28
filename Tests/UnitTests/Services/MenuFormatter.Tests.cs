namespace Comanda.TestingSuite.UnitTests.Services;

public sealed class MenuFormatterServiceTests
{
    private readonly IMenuFormatter _menuFormatterService;

    public MenuFormatterServiceTests()
    {
        _menuFormatterService = new MenuFormatterService();
    }

    [Fact(DisplayName = "Should format a menu with one product without ingredients")]
    public void ShouldFormatAMenuWithOneProductWithoutIngredients()
    {
        var product = new Product
        {
            Title = "Product Title",
            Description = "Product Description",
            Price = 9.99m,
            Ingredients = new List<ProductIngredient>()
        };

        var products = new List<Product> { product };

        var formattedMenu = _menuFormatterService.Format(products);
        Assert.Equal("Menu:\n- Product Title: R$ 9,99\n  Description: Product Description\n\n", formattedMenu);
    }

    [Fact(DisplayName = "Should format a menu with multiple products without ingredients")]
    public void ShouldFormatAMenuWithMultipleProductsWithoutIngredients()
    {
        var product1 = new Product
        {
            Title = "Product 1 Title",
            Description = "Product 1 Description",
            Price = 19.99m,
            Ingredients = new List<ProductIngredient>()
        };

        var product2 = new Product
        {
            Title = "Product 2 Title",
            Description = "Product 2 Description",
            Price = 29.99m,
            Ingredients = new List<ProductIngredient>()
        };

        var products = new List<Product> { product1, product2 };

        var formattedMenu = _menuFormatterService.Format(products);
        Assert.Equal("Menu:\n- Product 1 Title: R$ 19,99\n  Description: Product 1 Description\n\n- Product 2 Title: R$ 29,99\n  Description: Product 2 Description\n\n", formattedMenu);
    }

    [Fact(DisplayName = "Should format a menu with multiple products with multiple ingredients")]
    public void ShouldFormatAMenuWithMultipleProductsWithMultipleIngredients()
    {
        var ingredient1 = new Ingredient { Name = "Ingredient 1" };
        var ingredient2 = new Ingredient { Name = "Ingredient 2" };

        var product1 = new Product
        {
            Title = "Product 1 Title",
            Description = "Product 1 Description",
            Price = 19.99m,
            Ingredients = new List<ProductIngredient>
            {
                new ProductIngredient { Ingredient = ingredient1 },
                new ProductIngredient { Ingredient = ingredient2 }
            }
        };

        var product2 = new Product
        {
            Title = "Product 2 Title",
            Description = "Product 2 Description",
            Price = 29.99m,
            Ingredients = new List<ProductIngredient>()
        };

        var products = new List<Product> { product1, product2 };

        var formattedMenu = _menuFormatterService.Format(products);
        Assert.Equal("Menu:\n- Product 1 Title: R$ 19,99\n  Description: Product 1 Description\n  Ingredients:\n    - Ingredient 1\n    - Ingredient 2\n\n- Product 2 Title: R$ 29,99\n  Description: Product 2 Description\n\n", formattedMenu);
    }
}
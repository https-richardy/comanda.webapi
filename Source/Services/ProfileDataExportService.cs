namespace Comanda.WebApi.Services;

public sealed class ProfileDataExportService(
    UserManager<Account> userManager,
    IOrderRepository orderRepository,
    ICustomerRepository customerRepository,
    IAddressRepository addressRepository
) : IProfileDataExportService
{
    public async Task<ProfileExportData> ExportDataAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        var customer = await customerRepository.FindCustomerByUserIdAsync(userId);

        var orders = await orderRepository.FindAllAsync(order => order.Customer.Account.Id == user!.Id);
        var addresses = await addressRepository.GetAddressesByCustomerIdAsync(customer!.Id);

        var data = new ProfileExportData
        {
            Name = user!.UserName!,
            Email = user!.Email!,

            Orders = orders
                .Select(order => (OrderHistoryExportFormatted) order)
                .ToList(),
    
            Addresses = addresses
                .Select(address => (AddressExportFormatted) address)
                .ToList()
        };

        return data;
    }
}
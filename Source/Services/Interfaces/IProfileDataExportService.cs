namespace Comanda.WebApi.Services;

public interface IProfileDataExportService
{
    Task<ProfileExportData> ExportDataAsync(string userId);
}
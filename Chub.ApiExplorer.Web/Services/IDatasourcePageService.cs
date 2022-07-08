namespace Chub.ApiExplorer.Web.Services
{
    using System.Globalization;
    using System.Threading.Tasks;
    using Chub.ApiExplorer.Web.Models;

    public interface IDatasourcePageService
    {
        Task<DataSource?> GetDataSource(string name, CultureInfo? culture = null);
    }
}
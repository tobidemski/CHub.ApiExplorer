namespace Chub.ApiExplorer.Web.Services
{
    using System.Threading.Tasks;
    using Chub.ApiExplorer.Web.Interfaces;

    public interface ITabSearchModelBuilder<T> : ITabSearchModelBuilder where T : ITabModel
    {
        new Task<T> GetModel(string searchTerm, bool countOnly = false, int skip = 0, int take = 25);
    }

    public interface ITabSearchModelBuilder
    {
        string TabIdentifier { get; }
        string TabTitle { get; }
        string IconCssClass { get; }

        Task<ITabModel> GetModel(string searchTerm, bool countOnly = false, int skip = 0, int take = 25);
    }
}

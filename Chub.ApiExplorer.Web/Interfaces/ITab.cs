namespace Chub.ApiExplorer.Web.Interfaces
{
    public interface ITab
    {
        string Id { get; set; }
        string Title { get; set; }
        string IconCssClass { get; set; }
        ITabModel? TabModel { get; }
        bool IsActive { get; set; }
    }

    public interface ITab<T> : ITab where T : ITabModel
    {
        new T? TabModel { get; set; }
    }

    public interface ITabModel<T> : ITabModel
    {
        new T? Model { get; set; }
    }

    public interface ITabModel
    {
        object? Model { get; }
        string ViewPath { get; set; }
        long TotalItemCount { get; set; }
    }
}

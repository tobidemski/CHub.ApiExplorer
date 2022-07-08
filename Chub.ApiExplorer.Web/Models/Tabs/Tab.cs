namespace Chub.ApiExplorer.Web.Models.Tabs
{
    using Chub.ApiExplorer.Web.Interfaces;

    public abstract class TabModel<T> : ITabModel<T> where T : class
    {
        public string ViewPath { get; set; } = string.Empty;
        public long TotalItemCount { get; set; }

        //public T Model { get => ((ITab)this) as T; set => ((ITab)this).Model = value; }
        public T? Model { get; set; }

        object? ITabModel.Model => this.Model;
    }

    public class Tab : ITab
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string IconCssClass { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public ITabModel? TabModel { get; set; }
    }

    public class Tab<T> : ITab<T> where T : ITabModel
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string IconCssClass { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public T? TabModel { get; set; }

        ITabModel? ITab.TabModel => this.TabModel as ITabModel;
    }
}

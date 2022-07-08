namespace Chub.ApiExplorer.Web.Models
{
    public class Page
    {
        public long Id { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string Url { get; set; }

        public bool IsTitleVisible { get; set; }
        public bool IsVisible { get; set; }
        public bool IsInSiteMap { get; set; }
        public bool IsInMenu { get; set; }
        public bool IsInBreadcrumbs { get; set; }
        public bool IsTemplate { get; set; }

        public bool? IsHomepage { get; set; }
    }
}

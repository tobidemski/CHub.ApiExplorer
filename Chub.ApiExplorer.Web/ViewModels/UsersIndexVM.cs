namespace Chub.ApiExplorer.Web.ViewModels
{
    using System.Collections.Generic;
    using Chub.ApiExplorer.Web.Models;

    public class UsersIndexVM : Pagination
    {
        public List<User> Users { get; set; } = new List<User>();
    }
}

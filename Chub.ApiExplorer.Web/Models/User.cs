namespace Chub.ApiExplorer.Web.Models
{
    using System;
    using System.Collections.Generic;

    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        public bool? HasToken { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTimeOffset? LastLoginDateTime { get; set; }

        public List<Group> Groups { get; set; } = new List<Group>();
    }
}

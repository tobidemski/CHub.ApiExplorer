namespace Chub.ApiExplorer.Web.Models
{
    using System.Collections.Generic;

    public class MemberGroup
    {
        public string Name { get; set; }
        public string Label { get; set; }

        public List<Member> Members { get; set; } = new List<Member>();
    }
}

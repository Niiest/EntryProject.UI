using System;
using System.Collections.Generic;

namespace EntryProject.UI.DataModels
{
    public partial class GroupType
    {
        public GroupType()
        {
            Groups = new HashSet<Group>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Group> Groups { get; set; }
    }
}

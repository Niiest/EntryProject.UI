using System;
using System.Collections.Generic;

namespace EntryProject.UI.DataModels
{
    public partial class Group
    {
        public Group()
        {
            GroupUsers = new HashSet<GroupUser>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int GroupTypeId { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual ICollection<GroupUser> GroupUsers { get; set; }
        public virtual GroupType GroupType { get; set; }
    }
}

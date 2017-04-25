using System;
using System.Collections.Generic;

namespace EntryProject.UI.DataModels
{
    public partial class GroupUser
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public int GroupId { get; set; }

        public virtual Group Group { get; set; }
    }
}

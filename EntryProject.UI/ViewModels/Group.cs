using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntryProject.UI.ViewModels
{
    public class Group
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }

        public int? GroupTypeId { get; set; }
    }
}

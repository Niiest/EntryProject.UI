using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntryProject.UI.ViewModels;

namespace EntryProject.UI.Repositories
{
    public interface IGroupRepository
    {
        Task<bool> ExistsAsync(string name);
        Task<IEnumerable<GroupType>> ListGroupTypesAsync();
        Task CreateAsync(string name, string description, int groupTypeId, string[] userPhoneNumbers);
    }
}

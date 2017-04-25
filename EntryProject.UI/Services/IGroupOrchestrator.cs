using EntryProject.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntryProject.UI.Services
{
    public interface IGroupOrchestrator
    {
        Task<List<GroupType>> ListGroupTypesAsync();
        Task<SimpleResponse> CreateAsync(CreateGroupViewModel viewModel, string[] phoneNumbers);
    }
}

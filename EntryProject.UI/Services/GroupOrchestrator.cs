using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntryProject.UI.ViewModels;
using EntryProject.UI.Repositories;
using System.Text.RegularExpressions;

namespace EntryProject.UI.Services
{
    public class GroupOrchestrator : IGroupOrchestrator
    {
        private readonly IGroupRepository _groupRepository;

        public const string DefaultSelectionText = "укажите тип группы";

        public GroupOrchestrator(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<List<GroupType>> ListGroupTypesAsync()
        {
            List<GroupType> groupTypes = (await _groupRepository.ListGroupTypesAsync()).ToList();

            groupTypes.Insert(0, new GroupType { Name = DefaultSelectionText });

            return groupTypes;
        }

        public async Task<SimpleResponse> CreateAsync(CreateGroupViewModel viewModel, string[] phoneNumbers)
        {
            if (await _groupRepository.ExistsAsync(viewModel.Name))
            { 
                return new SimpleResponse().AddErrorMessage("Группа с таким именем уже существует");
            }

            var phoneNumberValidator = new Regex("^[0-9]{11}$");

            if (!phoneNumbers.All(num => phoneNumberValidator.IsMatch(num)))
            {
                return new SimpleResponse().AddErrorMessage("Некоторые номера телефонов содержат неверный формат");
            }

            await _groupRepository.CreateAsync(viewModel.Name, viewModel.Description, viewModel.SelectedGroupTypeId.Value, phoneNumbers);
            
            return new SimpleResponse();
        }

    }
}

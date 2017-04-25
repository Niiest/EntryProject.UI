using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EntryProject.UI.ViewModels
{
    public class CreateGroupViewModel
    {
        [DisplayName("Имя группы")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Name { get; set; }

        [DisplayName("Описание")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Description { get; set; }

        [DisplayName("Тип")]
        [Required]
        public int? SelectedGroupTypeId { get; set; }

        public IEnumerable<GroupType> GroupTypes { get; set; }

        [DisplayName("Телефоны пользователей")]
        public string UserPhones { get; set; }
    }
}

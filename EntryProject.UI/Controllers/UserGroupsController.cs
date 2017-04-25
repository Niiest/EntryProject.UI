using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EntryProject.UI.ViewModels;
using System.IO;
using EntryProject.UI.Services;

namespace EntryProject.UI.Controllers
{
    public class UserGroupsController : Controller
    {
        private readonly IGroupOrchestrator _groupOrchestrator;

        public const int MaxFileSize = 10 * 1048576; // 10 Mb
        public static string[] AllowedFileExtensions { get; } = new[] { ".csv", ".txt" };

        public UserGroupsController(IGroupOrchestrator groupOrchestrator)
        {
            _groupOrchestrator = groupOrchestrator;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var viewModel = new CreateGroupViewModel();

            viewModel.GroupTypes = await _groupOrchestrator.ListGroupTypesAsync();
            
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CreateGroupViewModel viewModel)
        {
            var createResult = new SimpleResponse();

            if (ModelState.IsValid)
            {
                string fileFieldName = nameof(CreateGroupViewModel.UserPhones);

                var selectedFile = Request.Form.Files.FirstOrDefault(f => f.Name == fileFieldName);
                if (selectedFile == null || selectedFile.Length == 0)
                {
                    createResult.AddErrorMessage("Выберите непустой файл");
                }
                else if (selectedFile.Length > MaxFileSize)
                {
                    createResult.AddErrorMessage("Слишком большой файл");
                }
                else
                {
                    using (Stream stream = Request.Form.Files[fileFieldName].OpenReadStream())
                    {
                        var reader = new StreamReader(stream);
                        string[] phoneNumbers = reader
                            .ReadToEnd()
                            .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                        createResult = await _groupOrchestrator.CreateAsync(viewModel, phoneNumbers);
                    }
                }
            }
            else
            {
                foreach (var errorEntry in ModelState)
                {
                    if (errorEntry.Value != null && errorEntry.Value.Errors != null)
                    {
                        foreach (var error in errorEntry.Value.Errors)
                        {
                            createResult.AddErrorMessage(error.ErrorMessage);
                        }
                    }
                }
            }
            
            return Json(createResult);
        }

    }
}

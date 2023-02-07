using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using JobPortalMud.Client;
using JobPortalMud.Client.Shared;
using JobPortalMud.Shared;
using MudBlazor;
using JobPortalMud.Client.Services.JobService;
using JobPortalMud.Client.Services.ContactService;
using JobPortalMud.Client.Services.UserService;
using JobPortalMud.Client.Services.AppliedJobService;
using System.Net.Http.Headers;

namespace JobPortalMud.Client.Pages
{
    public partial class EditProfile
    {
        [Parameter]
        public string? UserName { get; set; }

        User user = new User();
        private List<FileUpload> fileUploads = new();
        public List<Countries>? countries = new List<Countries>();

        private string filename = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            var result = await Http.GetFromJsonAsync<List<Countries>>("api/JobList/countries");
            if (result != null)
            {
                countries = result;
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            if (UserName != null)
            {
                user = await UserService.GetSingleUser(UserName);
            }
        }

        async Task HandleSubmit()
        {
                var fileUpload = fileUploads.SingleOrDefault(f => f.OriginalFileName == filename);
                if (fileUpload != null)
                {
                    user.Resume = fileUpload.Resume;
                    user.FileContentType = fileUpload.FileContentType;
                    user.OriginalFileName = fileUpload.OriginalFileName;
                    user.StoredFileName = fileUpload.StoredFileName;
                }

                if (user.Resume.Length > 0)
                {
                    await UserService.UpdateUser(user, UserName);
                    navigation.NavigateTo($"profile/{UserName}");
                    snackBar.Add("Profile Updated Successfully", Severity.Success);
                }
                else
                {
                    snackBar.Add("Please select Resume...", Severity.Warning);
                }
        }

        private async Task OnFileChangeDoc(InputFileChangeEventArgs e)
        {
            if (IsValidExtensionDoc(e.File))
            {
                using var content = new MultipartFormDataContent();
                foreach (var file in e.GetMultipleFiles(int.MaxValue))
                {
                    var fileContent = new StreamContent(file.OpenReadStream(long.MaxValue));
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    filename = file.Name;
                    content.Add(content: fileContent, name: "\"files\"", fileName: file.Name);
                }

                var response = await Http.PostAsync("/api/File", content);
                var newUploadresults = await response.Content.ReadFromJsonAsync<List<FileUpload>>();
                if (newUploadresults != null)
                {
                    fileUploads = fileUploads.Concat(newUploadresults).ToList();
                }
            }
            else
            {
                snackBar.Add("Please select .doc, .docx, .pdf files only...", Severity.Warning);
            }
        }

        bool IsValidExtensionDoc(IBrowserFile file2)
        {
            bool isValid = false;
            string[] fileExtension = {".doc", ".docx", ".pdf"};
            for (int i = 0; i <= fileExtension.Length - 1; i++)
            {
                if (file2.Name.Contains(fileExtension[i]))
                {
                    isValid = true;
                    break;
                }
                else
                {
                    isValid = false;
                }
            }

            return isValid;
        }
    }
}
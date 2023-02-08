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
using Microsoft.AspNetCore.Authorization;

namespace JobPortalMud.Client.Pages
{
    public partial class Index
    {
        public List<JobPortalMud.Shared.JobList> jobs = new List<JobPortalMud.Shared.JobList>();
        public List<JobPortalMud.Shared.JobList> activeJobs = new List<JobPortalMud.Shared.JobList>();
        public List<User> users = new List<User>();
        Role role = new Role();
        string user = "";
        protected override async Task OnInitializedAsync()
        {
            var userDetails = (await Auth.GetAuthenticationStateAsync()).User.Identity;
            if (userDetails != null && userDetails.IsAuthenticated)
            {
                user = userDetails.Name;

                var result1 = await Http.GetFromJsonAsync<List<JobPortalMud.Shared.JobList>>("api/JobList");
                if (result1 != null)
                {
                    jobs = result1;
                }

                var result2 = await Http.GetFromJsonAsync<List<JobPortalMud.Shared.JobList>>("api/JobList/Activejobs");
                if (result2 != null)
                {
                    activeJobs = result2;
                }

                var result3 = await Http.GetFromJsonAsync<List<User>>("api/User");
                if (result3 != null)
                {
                    users = result3;
                }

                var result4 = await Http.GetFromJsonAsync<Role>($"api/User/role/{user}");
                if (result4 != null)
                {
                    role = result4;
                }
            }
        }
    }
}
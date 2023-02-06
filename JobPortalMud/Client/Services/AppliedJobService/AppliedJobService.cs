using JobPortalMud.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace JobPortalMud.Client.Services.AppliedJobService
{
    public class AppliedJobService : IAppliedJobService
    {
        public List<AppliedJob> AppliedJobs { get; set; } = new List<AppliedJob>();
        public Paging Paging { get; set; } = new Paging();

        private readonly HttpClient _http;
        private readonly NavigationManager _navigationManager;

        public AppliedJobService(HttpClient http, NavigationManager navigationManager)
        {
            _http = http;
            _navigationManager = navigationManager;
        }
        public async Task DeleteAppliedJob(int id, string user)
        {
            var result = await _http.DeleteAsync($"api/AppliedJob/{id}/{user}");
            var response = await result.Content.ReadFromJsonAsync<List<AppliedJob>>();
            AppliedJobs = response;
            _navigationManager.NavigateTo($"appliedjoblist/{user}");
        }

        public async Task GetAllAppliedJobs(string user)
        {
            var result = await _http.GetFromJsonAsync<List<AppliedJob>>($"api/AppliedJob/employer/{user}");
            if (result != null)
            {
                AppliedJobs = result;
            }
        }

        public async Task CreateAppliedJob(AppliedJob appliedJob, int id, string user)
        {
            var result = await _http.PostAsJsonAsync("api/AppliedJob", appliedJob);
            var response = await result.Content.ReadFromJsonAsync<List<AppliedJob>>();
            AppliedJobs = response;
            _navigationManager.NavigateTo($"viewjob/{id}/{user}");
        }

        public async Task<AppliedJob> GetSingleAppliedJob(int id)
        {
            var result = await _http.GetFromJsonAsync<AppliedJob>($"api/AppliedJob/{id}");
            if (result != null)
            {
                return result;
            }
            else
            {
                throw new Exception("application not found");
            }
        }

        public async Task GetSingleUserAppliedJob(string user, int currentPage)
        {
            var result = await _http.GetFromJsonAsync<List<AppliedJob>>($"api/AppliedJob/user/{user}");
            if (result != null)
            {
                var list = result;
                int pageSize = 5;
                int count = list.Count;
                int TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                list = list.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                Paging.PageSize = pageSize;
                Paging.CurrentPage = currentPage;
                Paging.TotalPages = TotalPages;
                AppliedJobs = list;
            }
        }

        public async Task DeleteUserAppliedJob(int id, string user)
        {
            var result = await _http.DeleteAsync($"api/AppliedJob/user/{id}/{user}");
            var response = await result.Content.ReadFromJsonAsync<List<AppliedJob>>();
            AppliedJobs = response;
            _navigationManager.NavigateTo($"myjobs/{user}");
        }

        public async Task CreateAppliedJobBook(AppliedJob appliedJob)
        {
            var result = await _http.PostAsJsonAsync("api/AppliedJob", appliedJob);
            var response = await result.Content.ReadFromJsonAsync<List<AppliedJob>>();
            AppliedJobs = response;
        }
    }
}

using JobPortalMud.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace JobPortalMud.Client.Services.JobService
{
	public class JobService : IJobService
	{
		private readonly HttpClient _http;
        private readonly NavigationManager _navigationManager;

        public JobService(HttpClient http, NavigationManager navigationManager)
		{
			_http = http;
            _navigationManager = navigationManager;
        }
		public List<JobList> jobs { get; set; } = new List<JobList>();

        public async Task CreateJob(JobList job)
        {
			var result = await _http.PostAsJsonAsync("api/JobList", job);
            var response = await result.Content.ReadFromJsonAsync<List<JobList>>();
			jobs = response;
            _navigationManager.NavigateTo("joblist");
        }

        public async Task<JobList> GetSingleJob(int id)
		{
			var result = await _http.GetFromJsonAsync<JobList>($"api/JobList/{id}");
			if (result != null)
			{
				return result;
			}
			else
			{
				throw new Exception("Job not found");
			}
		}

        public async Task UpdateJob(JobList job)
        {
            var result = await _http.PutAsJsonAsync("api/JobList", job);
            var response = await result.Content.ReadFromJsonAsync<List<JobList>>();
            jobs = response;
            _navigationManager.NavigateTo("joblist");
        }
    }
}

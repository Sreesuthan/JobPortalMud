using JobPortalMud.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace JobPortalMud.Client.Services.BookmarkedJobService
{
    public class BookmarkedJobService : IBookmarkedJobService
    {
        public List<BookMark> BookmaredJobs { get; set; } = new List<BookMark>();
        public Paging Paging { get; set; } = new Paging();
        private readonly HttpClient _http;
        private readonly NavigationManager _navigationManager;

        public BookmarkedJobService(HttpClient http, NavigationManager navigationManager)
        {
            _http = http;
            _navigationManager = navigationManager;
        }

        public async Task CreateBookMarkedJob(BookMark BookmarkedJob, int id, string user)
        {
            var result = await _http.PostAsJsonAsync("api/BookMark", BookmarkedJob);
            var response = await result.Content.ReadFromJsonAsync<List<BookMark>>();
            BookmaredJobs = response;
        }

        public async Task GetAllBookMarkedJobs(string user, int currentPage)
        {
            var result = await _http.GetFromJsonAsync<List<BookMark>>($"api/BookMark/{user}");
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
                BookmaredJobs = list;
            }
        }

        public async Task DeleteBookMarkedJob(int id, string user)
        {
            var result = await _http.DeleteAsync($"api/BookMark/{id}/{user}");
            var response = await result.Content.ReadFromJsonAsync<List<BookMark>>();
            BookmaredJobs = response;
            _navigationManager.NavigateTo($"favorites/{user}");
        }
    }
}

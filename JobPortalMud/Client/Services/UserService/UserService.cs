using JobPortalMud.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace JobPortalMud.Client.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly HttpClient _http;
        private readonly NavigationManager _navigationManager;

        public List<User> users { get; set; } = new List<User>();
        public UserService(HttpClient http, NavigationManager navigationManager)
        {
            _http = http;
            _navigationManager = navigationManager;
        }
        public async Task DeleteUser(string user)
        {
            var result = await _http.DeleteAsync($"api/User/{user}");
            var response = await result.Content.ReadFromJsonAsync<List<User>>();
            users = response;
            _navigationManager.NavigateTo("UserList");
        }

        public async Task GetAllUsers()
        {
            var result = await _http.GetFromJsonAsync<List<User>>("api/User");
            if (result != null)
            {
                users = result;
            }
        }
    }
}

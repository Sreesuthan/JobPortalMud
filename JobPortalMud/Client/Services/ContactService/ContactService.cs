using JobPortalMud.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace JobPortalMud.Client.Services.ContactService
{
    public class ContactService : IContactService
    {
        private readonly HttpClient _http;
        private readonly NavigationManager _navigationManager;

        public List<Contact> contacts { get; set; } = new List<Contact>();

        public ContactService(HttpClient http, NavigationManager navigationManager)
        {
            _http = http;
            _navigationManager = navigationManager;
        }
        public async Task GetAllContacts()
        {
            var result = await _http.GetFromJsonAsync<List<Contact>>("api/Contact");
            if (result != null)
            {
                contacts = result;
            }
        }

        public async Task DeleteContact(int id)
        {
            var result = await _http.DeleteAsync($"api/Contact/{id}");
            var response = await result.Content.ReadFromJsonAsync<List<Contact>>();
            contacts = response;
            _navigationManager.NavigateTo("contactlist");
        }

        public async Task CreateContact(Contact contact)
        {
            var result = await _http.PostAsJsonAsync("api/Contact", contact);
            var response = await result.Content.ReadFromJsonAsync<List<Contact>>();
            contacts = response;
            _navigationManager.NavigateTo("contact");
        }

        public async Task<Contact> GetSingleContact(int id)
        {
            var result = await _http.GetFromJsonAsync<Contact>($"api/Contact/{id}");
            if (result != null)
            {
                return result;
            }
            else
            {
                throw new Exception("contact not found");
            }
        }
    }
}

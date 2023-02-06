using JobPortalMud.Shared;

namespace JobPortalMud.Client.Services.ContactService
{
    public interface IContactService
    {
        List<Contact> contacts { get; set; }
        Task GetAllContacts();
        Task<Contact> GetSingleContact(int id);
        Task DeleteContact(int id);
        Task CreateContact(Contact contact);
    }
}

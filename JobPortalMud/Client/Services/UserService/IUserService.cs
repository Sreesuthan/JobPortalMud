using JobPortalMud.Shared;

namespace JobPortalMud.Client.Services.UserService
{
    public interface IUserService
    {
        List<User> users { get; set; }
        Task GetAllUsers();
        Task DeleteUser(string user);
    }
}

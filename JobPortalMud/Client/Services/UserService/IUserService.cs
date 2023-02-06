using JobPortalMud.Shared;

namespace JobPortalMud.Client.Services.UserService
{
    public interface IUserService
    {
        List<User> users { get; set; }
        Task GetAllUsers();
        Task<User> GetSingleUser(string username);
        Task UpdateUser(User user, string username);
        Task DeleteUser(string user);
    }
}

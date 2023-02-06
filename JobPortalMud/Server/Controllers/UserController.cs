using Dapper;
using JobPortalMud.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace JobPortalMud.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;

        public UserController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<User> users = await SelectAllUsers(connection);
            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<User>> GetUser(string username)
        {
            try
            {
                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var user = await connection.QueryFirstAsync<User>("select Name, UserName, Address, mobile, Email, Country, WorksOn, Experience, Resume, StoredFileName, OriginalFileName, FileContenttype, ProfileImage from AspNetUsers where UserName=@username",
                    new { username = username });
                return Ok(user);
            }
            catch
            {
                return BadRequest("user details not found...");
            }
        }

        [HttpGet("role/{user}")]
        public async Task<ActionResult<Role>> GetUserRole(string user)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var userRole = await connection.QueryFirstAsync<Role>("select u.UserName, r.Name from AspNetUserRoles as ur inner join AspNetUsers as u on ur.UserId = u.Id inner join AspNetRoles as r on ur.RoleId = r.Id where u.UserName = @username",
                new { username = user });
            return Ok(userRole);
        }

        [HttpGet("external/{user}")]
        public async Task<ActionResult<Role>> GetExternalProvider(string user)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var userRole = await connection.QueryFirstAsync<Role>("select u.UserName, ul.LoginProvider from AspNetUserLogins as ul inner join AspNetUsers as u on ul.UserId = u.Id where u.UserName =@username",
                new { username = user });
            return Ok(userRole);
        }

        [HttpPut]
        public async Task<ActionResult<List<User>>> UpdateJob(User user)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update AspNetUsers set Name=@Name, UserName=@UserName, Address=@Address, mobile=@mobile, Email=@Email, Country=@Country, WorksOn=@WorksOn, Experience=@Experience, ProfileImage=@ProfileImage, Resume=@Resume, OriginalFileName=@OriginalFileName, FileContentType = @FileContentType, StoredFileName=@StoredFileName where UserName=@UserName", user);
            return Ok(await SelectAllUsers(connection));
        }

        [HttpDelete("{user}")]
        public async Task<ActionResult<List<User>>> DeleteUser(string user)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Delete from AspNetUsers where UserName=@Id", new { Id = user });
            return Ok(await SelectAllUsers(connection));
        }

        private static async Task<IEnumerable<User>> SelectAllUsers(SqlConnection connection)
        {
            return await connection.QueryAsync<User>("select ROW_NUMBER() over(order by (select 1)) as [SrNo], Name, UserName, Email, mobile, Address from AspNetUsers");
        }
    }
}

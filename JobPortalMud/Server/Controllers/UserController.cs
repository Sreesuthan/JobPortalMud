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

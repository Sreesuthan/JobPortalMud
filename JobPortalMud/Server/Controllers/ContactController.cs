using Dapper;
using JobPortalMud.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace JobPortalMud.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ContactController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<List<Contact>>> GetAllContacts()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Contact> contacts = await SelectAllContacts(connection);
            return Ok(contacts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> Getcontact(int id)
        {
            try
            {
                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var job = await connection.QueryFirstAsync<Contact>("select * from Contact where id=@id",
                    new { id = id });
                return Ok(job);
            }
            catch
            {
                return BadRequest("contact details not found...");
            }
        }

        [HttpPost]
        public async Task<ActionResult<List<Contact>>> CreateContact(Contact contact)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Insert into Contact values(@FirstName, @LastName, @Email, @Subject, @Message)", contact);
            return Ok(await SelectAllContacts(connection));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<Contact>>> DeleteContact(int id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Delete from Contact where Id=@Id", new { Id = id });
            return Ok(await SelectAllContacts(connection));
        }


        private static async Task<IEnumerable<Contact>> SelectAllContacts(SqlConnection connection)
        {
            return await connection.QueryAsync<Contact>("select ROW_NUMBER() over(order by (select 1)) as [SrNo], Id, FirstName, LastName, Email, Subject, Message from Contact");
        }
    }
}

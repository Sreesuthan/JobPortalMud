using Dapper;
using JobPortalMud.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace JobPortalMud.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookMarkController : ControllerBase
    {
        private readonly IConfiguration _config;

        public BookMarkController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("{user}")]
        public async Task<ActionResult<List<BookMark>>> GetAllBookmarkedjob(string user)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<BookMark> bookmarkedJobs = await connection.QueryAsync<BookMark>("Select bj.Id, j.CompanyName, bj.JobId, j.Title, bj.UserName, j.JobType, j.CompanyName, j.Address, j.country, State from BookmarkedJobs as bj inner join AspNetUsers as u on bj.UserName = u.UserName inner join jobs as j on bj.JobId = j.JobId where u.UserName=@UserName order by bj.Id desc", new { UserName = user });
            return Ok(bookmarkedJobs);
        }

        [HttpGet("bookmarked/{Id}/{user}")]
        public async Task<ActionResult<List<BookMark>>> GetBookmarkedJobs(int Id, string user)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<BookMark> bookmarkedJobs = await connection.QueryAsync<BookMark>("Select * from BookmarkedJobs Where JobId=@JobId and UserName=@UserName", new { JobId = Id, UserName = user });
            return Ok(bookmarkedJobs);
        }

        [HttpPost]
        public async Task<ActionResult<List<BookMark>>> AddBookmark(BookMark bookMark)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("insert into BookmarkedJobs values(@JobId, @UserName)", bookMark);
            return Ok(await SelectAllBookemaredJobs(connection));
        }

        [HttpDelete("{id}/{user}")]
        public async Task<ActionResult<List<BookMark>>> DeleteBookmark(int id, string user)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Delete from BookmarkedJobs where Id=@Id", new { Id = id });
            return Ok(await connection.QueryAsync<BookMark>("Select bj.Id, j.CompanyName, bj.JobId, j.Title, bj.UserName, j.JobType, j.CompanyName, j.Address, j.country, State from BookmarkedJobs as bj inner join AspNetUsers as u on bj.UserName = u.UserName inner join jobs as j on bj.JobId = j.JobId where u.UserName=@UserName", new { UserName = user }));
        }

        private static async Task<IEnumerable<BookMark>> SelectAllBookemaredJobs(SqlConnection connection)
        {
            return await connection.QueryAsync<BookMark>("Select bj.Id, j.CompanyName, bj.JobId, j.Title, bj.UserName, j.JobType, j.CompanyName, j.Address, j.country, State from BookmarkedJobs as bj inner join AspNetUsers as u on bj.UserName = u.UserName inner join jobs as j on bj.JobId = j.JobId");
        }
    }
}

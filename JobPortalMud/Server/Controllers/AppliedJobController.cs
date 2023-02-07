using Dapper;
using JobPortalMud.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace JobPortalMud.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppliedJobController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AppliedJobController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("user/{user}")]
        public async Task<ActionResult<List<AppliedJob>>> GetUserAllAppliedJobs(string user)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<AppliedJob> appliedJobs = await connection.QueryAsync<AppliedJob>("Select aj.Id, j.CompanyName, aj.JobId, j.Title, aj.UserName, j.JobType, j.CompanyName, j.Address, j.country, State from AppliedJobs as aj inner join AspNetUsers as u on aj.UserName = u.UserName inner join jobs as j on aj.JobId = j.JobId where u.UserName=@UserName order by aj.Id desc", new { UserName = user });
            return Ok(appliedJobs);
        }

        [HttpGet("employer/{user}")]
        public async Task<ActionResult<List<AppliedJob>>> GetAllAppliedJobs(string user)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<AppliedJob> appliedJobs = await connection.QueryAsync<AppliedJob>("Select Row_Number() over(order by (select 1)) as [SrNo], aj.Id, j.CompanyName, aj.JobId, j.Employer, j.Title, u.mobile, u.Name,u.Email, aj.UserName from AppliedJobs as aj inner join AspNetUsers as u on aj.UserName = u.UserName inner join jobs as j on aj.JobId = j.JobId where j.Employer=@Employer order by aj.Id desc", new { Employer = user });
            return Ok(appliedJobs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppliedJob>> GetAppliedJob(int id)
        {
            try
            {
                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var appliedJob = await connection.QueryFirstAsync<AppliedJob>("Select u.Resume, u.OriginalFileName, u.FileContentType, u.StoredFileName from AppliedJobs as aj inner join AspNetUsers as u on aj.UserName = u.UserName inner join jobs as j on aj.JobId = j.JobId where aj.Id=@id",
                    new { id = id });
                return Ok(appliedJob);
            }
            catch
            {
                return BadRequest("job details not found...");
            }
        }

        [HttpGet("apply/{Id}/{user}")]
        public async Task<ActionResult<List<AppliedJob>>> GetAppliedJobs(int Id, string user)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<AppliedJob> appliedJobs = await connection.QueryAsync<AppliedJob>("Select * from AppliedJobs Where JobId=@JobId and UserName=@UserName", new { JobId = Id, UserName = user });
            return Ok(appliedJobs);
        }

        [HttpPost]
        public async Task<ActionResult<List<AppliedJob>>> CreateJob(AppliedJob appliedJob)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("insert into AppliedJobs values(@JobId, @UserName)", appliedJob);
            return Ok(await SelectAllAppliedJobs(connection));
        }

        [HttpDelete("{id}/{user}")]
        public async Task<ActionResult<List<AppliedJob>>> DeleteAppliedJob(int id, string user)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Delete from AppliedJobs where Id=@Id", new { Id = id });
            return Ok(await connection.QueryAsync<AppliedJob>("Select Row_Number() over(order by (select 1)) as [SrNo], aj.Id, j.CompanyName, aj.JobId, j.Employer, j.Title, u.mobile, u.Name,u.Email, aj.UserName from AppliedJobs as aj inner join AspNetUsers as u on aj.UserName = u.UserName inner join jobs as j on aj.JobId = j.JobId where j.Employer=@Employer", new { Employer = user }));
        }

        [HttpDelete("user/{id}/{user}")]
        public async Task<ActionResult<List<BookMark>>> DeleteUserAppliedJob(int id, string user)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Delete from AppliedJobs where Id=@Id and UserName=@UserName", new { Id = id, UserName = user });
            return Ok(await connection.QueryAsync<BookMark>("Select bj.Id, j.CompanyName, bj.JobId, j.Title, bj.UserName, j.JobType, j.CompanyName, j.Address, j.country, State from AppliedJobs as bj inner join AspNetUsers as u on bj.UserName = u.UserName inner join jobs as j on bj.JobId = j.JobId where u.UserName=@UserName", new { UserName = user }));
        }

        private static async Task<IEnumerable<AppliedJob>> SelectAllAppliedJobs(SqlConnection connection)
        {
            return await connection.QueryAsync<AppliedJob>("Select Row_Number() over(order by (select 1)) as [SrNo], aj.Id, j.CompanyName, aj.JobId, j.Title, u.mobile, u.Name, u.Email, j.Employer, aj.UserName from AppliedJobs as aj inner join AspNetUsers as u on aj.UserName = u.UserName inner join jobs as j on aj.JobId = j.JobId ");
        }
    }
}

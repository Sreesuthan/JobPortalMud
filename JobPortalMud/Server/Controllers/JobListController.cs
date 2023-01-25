using Dapper;
using JobPortalMud.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace JobPortalMud.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobListController : ControllerBase
    {
        private readonly IConfiguration _config;

        public JobListController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<List<JobList>>> GetAllJobs()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<JobList> jobs = await SelectAllJobs(connection);
            return Ok(jobs);
        }

        [HttpGet("jobtypes")]
        public async Task<ActionResult<List<JobTypes>>> GetAllJobType()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<JobTypes> jobTypes = await connection.QueryAsync<JobTypes>("select * from JobType");
            return Ok(jobTypes);
        }

        [HttpGet("countries")]
        public async Task<ActionResult<List<Countries>>> GetAllCountry()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Countries> countries = await connection.QueryAsync<Countries>("select * from Country");
            return Ok(countries);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<JobList>> GetJob(int id)
        {
            try
            {
                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var job = await connection.QueryFirstAsync<JobList>("select * from jobs where JobId=@id",
                    new { id = id });
                return Ok(job);
            }
            catch
            {
                return BadRequest("job details not found...");
            }
        }

        [HttpPost]
        public async Task<ActionResult<List<JobList>>> CreateJob(JobList job)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Insert into jobs values(@Title, @NoOfPost, @Description, @Qualification, @Experience, @Specialization, @LastDateToApply, @Salary, @JobType, @CompanyName, @CompanyImage, @Website, @Email, @Address, @country, @State, @CreateDate)", job);
            return Ok(await SelectAllJobs(connection));
        }

        [HttpPut]
        public async Task<ActionResult<List<JobList>>> UpdateJob(JobList job)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update jobs set Title=@Title, NoOfPost=@NoOfPost, Description=@Description, Qualification=@Qualification, Experience=@Experience, Specialization=@Specialization, lastDateToApply=@lastDateToApply, Salary=@Salary, JobType=@JobType, CompanyName=@CompanyName, CompanyImage=@CompanyImage, Website=@Website, Email=@Email, Address=@Address, country=@Country, State=@State where JobId = @JobId", job);
            return Ok(await SelectAllJobs(connection));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<JobList>>> DeleteJob(int id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("delete from jobs where JobId=@Id", new { Id = id });
            return Ok(await SelectAllJobs(connection));
        }
        private static async Task<IEnumerable<JobList>> SelectAllJobs(SqlConnection connection)
        {
            return await connection.QueryAsync<JobList>("select ROW_NUMBER() over(order by (select 1)) as [SrNo], JobId, Title, noofpost, qualification, Experience, LastDateToApply, CompanyName, Country, State, CreateDate, JobType from jobs");
        }
    }
}

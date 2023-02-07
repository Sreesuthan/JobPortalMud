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

        [HttpGet("jobs/{user}")]
        public async Task<ActionResult<List<JobList>>> GetAllJobs(string user)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<JobList> jobs = await connection.QueryAsync<JobList>("select ROW_NUMBER() over(order by (select 1)) as [SrNo], j.JobId, j.Title, j.noofpost, j.Experience, j.LastDateToApply, j.CompanyName, j.Country, j.State, j.CreateDate, j.JobType, j.Active, j.CategoryId, c.Category from jobs as j inner join Category as c on j.CategoryId=c.Id where j.Employer=@Employer order by j.JobId desc", new { Employer = user });
            return Ok(jobs);
        }

        [HttpGet]
        public async Task<ActionResult<List<JobList>>> GetAllJobsCount()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<JobList> jobs = await connection.QueryAsync<JobList>("select ROW_NUMBER() over(order by (select 1)) as [SrNo], JobId, Title, noofpost, Experience, LastDateToApply, CompanyName, Country, State, CreateDate, JobType, Active from jobs");
            return Ok(jobs);
        }

        [HttpGet("Activejobs")]
        public async Task<ActionResult<List<JobList>>> GetAllActiveJobs()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<JobList> jobs = await connection.QueryAsync<JobList>("select ROW_NUMBER() over(order by (select 1)) as [SrNo], JobId, Title, noofpost, Experience, LastDateToApply, CompanyName, Country, State, CreateDate, JobType, Active from jobs where Active='true'");
            return Ok(jobs);
        }

        [HttpGet("search/{Id}")]
		public async Task<ActionResult<List<JobList>>> GetAllJobsSearch(int id)
		{
			using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
			IEnumerable<JobList> jobs = await connection.QueryAsync<JobList>("select ROW_NUMBER() over(order by (select 1)) as [SrNo], JobId, Title, CompanyName, Address, State, Country, JobType, CompanyImage, CreateDate, DATEDIFF(MINUTE, CreateDate, GETDATE()) as DateDiffMin from jobs where Active='true' and CategoryId=@Id", new { Id = id });
			return Ok(jobs);
		}

        [HttpGet("find/{Id}/{title}/{country}")]
        public async Task<ActionResult<List<JobList>>> GetJobsAfterSearch(int id, string title, string country)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<JobList> jobs = await connection.QueryAsync<JobList>($"select ROW_NUMBER() over(order by (select 1)) as [SrNo], JobId, Title, CompanyName, Address, State, Country, JobType, CompanyImage from jobs where Title like '%{title}%' and Country like '%{country}%' and CategoryId=@Id and Active='true'", new { Id = id});
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

        [HttpGet("categories")]
        public async Task<ActionResult<List<Categories>>> GetAllCategories()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Categories> categories = await connection.QueryAsync<Categories>("select * from Category Order by Category");
            return Ok(categories);
        }

        [HttpGet("DateDiff")]
        public async Task<ActionResult<List<JobList>>> GetDateDiff()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<JobList> jobs = await connection.QueryAsync<JobList>("select JobId, Title, CreateDate, DATEDIFF(MINUTE, CreateDate, GETDATE()) as DateDiffMin from jobs order by DateDiffMin");
            return Ok(jobs);
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
            await connection.ExecuteAsync("Insert into jobs values(@Title, @NoOfPost, @Description, @Experience, @Specialization, @LastDateToApply, @Salary, @JobType, @CompanyName, @CompanyImage, @Website, @Email, @Address, @country, @State, @CreateDate, @Active, @Employer, @CategoryId)", job);
            return Ok(await SelectAllJobs(connection));
        }

        [HttpPut]
        public async Task<ActionResult<List<JobList>>> UpdateJob(JobList job)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update jobs set Title=@Title, NoOfPost=@NoOfPost, Description=@Description, Experience=@Experience, Specialization=@Specialization, lastDateToApply=@lastDateToApply, Salary=@Salary, JobType=@JobType, CompanyName=@CompanyName, CompanyImage=@CompanyImage, Website=@Website, Email=@Email, Address=@Address, country=@Country, State=@State, Active=@Active, CategoryId=@CategoryId where JobId = @JobId", job);
            return Ok(await SelectAllJobs(connection));
        }

        [HttpDelete("{id}/{user}")]
        public async Task<ActionResult<List<JobList>>> DeleteJob(int id, string user)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("delete from jobs where JobId=@Id", new { Id = id });
            return Ok(await connection.QueryAsync<JobList>("select ROW_NUMBER() over(order by (select 1)) as [SrNo], JobId, Title, noofpost, Experience, LastDateToApply, CompanyName, Country, State, CreateDate, JobType, Active from jobs where Employer=@Employer", new { Employer = user }));
        }
        private static async Task<IEnumerable<JobList>> SelectAllJobs(SqlConnection connection)
        {
            return await connection.QueryAsync<JobList>("select ROW_NUMBER() over(order by (select 1)) as [SrNo], JobId, Title, noofpost, Experience, LastDateToApply, CompanyName, Country, State, CreateDate, JobType, Active from jobs");
        }
    }
}

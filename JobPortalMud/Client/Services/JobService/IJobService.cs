using JobPortalMud.Shared;

namespace JobPortalMud.Client.Services.JobService
{
    public interface IJobService
    {
        List<JobList> jobs { get; set; }
		Task<JobList> GetSingleJob(int id);
        Task CreateJob(JobList job, string user);
        Task UpdateJob(JobList job, string user);
    }
}

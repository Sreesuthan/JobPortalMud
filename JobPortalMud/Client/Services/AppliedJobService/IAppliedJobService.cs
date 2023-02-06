using JobPortalMud.Shared;

namespace JobPortalMud.Client.Services.AppliedJobService
{
    public interface IAppliedJobService
    {
        List<AppliedJob> AppliedJobs { get; set; }
        Paging Paging { get; set; }
        Task GetAllAppliedJobs(string user);
        Task<AppliedJob> GetSingleAppliedJob(int id);
        Task GetSingleUserAppliedJob(string user, int currentPage);
        Task CreateAppliedJob(AppliedJob appliedJob, int id, string user);
        Task CreateAppliedJobBook(AppliedJob appliedJob);
        Task DeleteAppliedJob(int id, string user);
        Task DeleteUserAppliedJob(int id, string user);
    }
}

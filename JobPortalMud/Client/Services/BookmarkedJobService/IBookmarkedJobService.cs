using JobPortalMud.Shared;

namespace JobPortalMud.Client.Services.BookmarkedJobService
{
    public interface IBookmarkedJobService
    {
        List<BookMark> BookmaredJobs { get; set; }
        Paging Paging { get; set; }
        Task GetAllBookMarkedJobs(string user, int currentPage);
        Task CreateBookMarkedJob(BookMark BookmarkedJob, int id, string user);
        Task DeleteBookMarkedJob(int id, string user);
    }
}

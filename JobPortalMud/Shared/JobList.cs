using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPortalMud.Shared
{
    public class JobList
    {
        public int SrNo { get; set; }
        public int JobId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int NoOfPost { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public DateTime? LastDateToApply { get; set; }
        public string Salary { get; set; } = string.Empty;
        public JobTypes? JobTypes { get; set; }
        public string JobType { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyImage { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public Countries? Countries { get; set; }
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public DateTime? CreateDate { get; set; }
        public string LastDate { get; set; } = string.Empty;
        public string NoPost { get; set; } = string.Empty;
        public bool Active { get; set; }
        public string Employer { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string? Category { get; set; }
        public long DateDiffMin { get; set; }
    }
}

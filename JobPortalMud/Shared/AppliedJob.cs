using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPortalMud.Shared
{
    public class AppliedJob
    {
        public int SrNo { get; set; }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public long mobile { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Resume { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public string FileContentType { get; set; } = string.Empty;
        public int JobId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string JobType { get; set; } = string.Empty;
        public string CompanyImage { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string EmploymentType { get; set; } = string.Empty;
    }
}

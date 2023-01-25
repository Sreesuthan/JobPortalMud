using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPortalMud.Shared
{
    public class User
    {
        public int SrNo { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TenthGrade { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string TwelfththGrade { get; set; } = string.Empty;
        public string GraduaionGrade { get; set; } = string.Empty;
        public string PostGraduaionGrade { get; set; } = string.Empty;
        public string PhD { get; set; } = string.Empty;
        public string WorksOn { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public string Resume { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string FileContentType { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public long mobile { get; set; }
    }
}

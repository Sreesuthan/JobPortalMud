using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobPortalMud.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public string WorksOn { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public string Resume { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public string FileContentType { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public long mobile { get; set; }
        public string ProfileImage { get; set; } = string.Empty;
    }
}
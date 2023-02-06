using JobPortalMud.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortalMud.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public FileController(IWebHostEnvironment env)
        {
            _env = env;
        }
        [HttpGet("{path}/{contentType}")]
        public async Task<IActionResult> DownloadFile(string path, string contentType)
        {
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, contentType, Path.GetFileName(path));
        }

        [HttpPost]
        public async Task<ActionResult<List<FileUpload>>> UploadFile(List<IFormFile> files)
        {
            List<FileUpload> uploadResults = new List<FileUpload>();
            foreach (var file in files)
            {
                var uploadResult = new FileUpload();
                var untrustedFileName = file.FileName;

                string trustedFileName = Path.ChangeExtension(Path.GetRandomFileName(), Path.GetExtension(file.FileName));
                var path = Path.Combine(_env.ContentRootPath, "Resume", trustedFileName);

                await using FileStream fileStream = new(path, FileMode.Create);
                await file.CopyToAsync(fileStream);

                uploadResult.OriginalFileName = untrustedFileName;
                uploadResult.StoredFileName = trustedFileName;
                uploadResult.FileContentType = file.ContentType;
                uploadResult.Resume = path;     
                uploadResults.Add(uploadResult);

            }
            return Ok(uploadResults);
        }
    }
}

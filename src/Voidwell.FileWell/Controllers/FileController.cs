using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Voidwell.FileWell.Data.Models;
using Voidwell.FileWell.Models;
using Voidwell.FileWell.Services;

namespace Voidwell.FileWell.Controllers
{
    [Route("/")]
    public class FileController : Controller
    {
        private readonly IFileService _fileService;
        private readonly AppOptions _options;
        private readonly ILogger<FileController> _logger;

        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        public FileController(IFileService fileService, IOptions<AppOptions> options, ILogger<FileController> logger)
        {
            _fileService = fileService;
            _options = options.Value;
            _logger = logger;
        }

        [HttpGet("{fileId:guid}.{fileType}")]
        public async Task<IActionResult> Get(Guid fileId, string fileType)
        {
            FileRecord file = await _fileService.GetFile(fileId);
            if (file == null)
            {
                return NotFound();
            }

            var filePath = GetFilePath(file);

            var stream = new FileStream(filePath, FileMode.Open);
            if (stream == null)
            {
                return NotFound();
            }

            return File(stream, file.FileMimeType);
        }

        [HttpDelete("{fileId:guid}")]
        [Authorize(Roles = "Administrator")]
        public Task Delete(Guid fileId)
        {
            var userId = Guid.NewGuid();

            return _fileService.DeleteFile(fileId, userId);
        }

        [HttpPost(Name = "FileUpload")]
        [Authorize(Roles = "Administrator")]
        [DisableFormValueModelBinding]
        public async Task<IActionResult> Upload()
        {
            var userId = Guid.NewGuid();

            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }

            var fileRecord = new FileRecord
            {
                FileId = Guid.NewGuid(),
                UploadedDate = DateTime.UtcNow
            };

            // Used to accumulate all the form url encoded key value pairs in the 
            // request.
            string targetFilePath = null;

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);

            try
            {

                var section = await reader.ReadNextSectionAsync();
                while (section != null)
                {
                    ContentDispositionHeaderValue contentDisposition;
                    var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

                    if (hasContentDispositionHeader && MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        fileRecord.FileName = contentDisposition.FileName.ToString();
                        fileRecord.FileType = fileRecord.FileName.Split('.').LastOrDefault();
                        fileRecord.FileMimeType = section.ContentType;

                        targetFilePath = Path.GetTempFileName();
                        using (var targetStream = System.IO.File.Create(targetFilePath))
                        {
                            await section.Body.CopyToAsync(targetStream);

                            fileRecord.FileSize += (uint)section.Body.Length;
                        }
                    }

                    section = await reader.ReadNextSectionAsync();
                }

                var destFilePath = GetFilePath(fileRecord);

                System.IO.File.Move(targetFilePath, destFilePath);

                var createdFile = await _fileService.UploadFile(fileRecord, userId);
                var protocol = Request.IsHttps ? "https" : "http";
                var result = new UploadResult
                {
                    FileId = createdFile.FileId,
                    Url = $"{protocol}://{Request.Host.ToString()}/{createdFile.FileId.ToString("N")}.{createdFile.FileType}"
                };

                return Ok(result);
            }
            catch(Exception)
            {
                if (!string.IsNullOrEmpty(targetFilePath))
                {
                    System.IO.File.Delete(targetFilePath);
                }

                throw;
            }
        }

        private static Encoding GetEncoding(MultipartSection section)
        {
            MediaTypeHeaderValue mediaType;
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out mediaType);
            // UTF-7 is insecure and should not be honored. UTF-8 will succeed in 
            // most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }
            return mediaType.Encoding;
        }

        private string GetFilePath(FileRecord file)
        {
            //var path = Path.Join(_options.ContentRoot, $"{file.UploadedDate.Year}/{file.UploadedDate.Month}/{file.UploadedDate.Day}/{file.FileId}");
            var path = Path.Join(_options.ContentRoot, $"{file.FileId}");
            return Path.GetFullPath(path);
        }
    }
}

using FM.Core.Models;
using FM.Cqrs.Commands;
using FM.Cqrs.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace FileManagerMockup.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public FilesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("upload-stream")]
        public async Task<IActionResult> UploadStream([FromForm] BillDto record)
        {
            if (!Request.HasFormContentType ||
                !MediaTypeHeaderValue.TryParse(Request.ContentType, out var mediaTypeHeader) ||
                string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
            {
                return BadRequest("Invalid request");
            }

            var boundary = HeaderUtilities.RemoveQuotes(mediaTypeHeader.Boundary).Value;
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);

            MultipartSection section;
            while ((section = await reader.ReadNextSectionAsync()) != null)
            {
                if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition))
                {
                    if (contentDisposition.IsFileDisposition())
                    {
                        var fileName = Path.GetRandomFileName();
                        var filePath = Path.Combine(Path.GetTempPath(), fileName);

                        using (var targetStream = System.IO.File.Create(filePath))
                        {
                            await section.Body.CopyToAsync(targetStream);
                        }

                        var command = new RegisterFileUploadCommand
                        {
                            FilePath = filePath,
                            FileSize = new FileInfo(filePath).Length
                        };

                        var result = await _mediator.Send(command);

                        if (result)
                        {
                            return Ok(new { FilePath = filePath });
                        }
                        else
                        {
                            return StatusCode(500, "Failed to register file upload");
                        }
                    }
                }
            }

            return BadRequest("No files data in the request");
        }

        [HttpGet("getTree")]
        public async Task<IActionResult> GetTree()
        {
            return Ok();
        }
    }
}

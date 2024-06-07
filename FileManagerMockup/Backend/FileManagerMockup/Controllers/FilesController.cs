using FM.Cqrs.Queries;
using System.Text;
using FM.Cqrs.Queries.Files;
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
        private readonly IWebHostEnvironment _env;

        public FilesController(IMediator mediator, IWebHostEnvironment env)
        {
            _mediator = mediator;
            _env = env;
        }

        [HttpPost("upload-stream")]
        public async Task<IActionResult> UploadStream()
        {
            if (!Request.HasFormContentType ||
                !MediaTypeHeaderValue.TryParse(Request.ContentType, out var mediaTypeHeader) ||
                string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
            {
                return BadRequest("Solicitud no válida");
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
                        var fileName = contentDisposition.FileName.Value.Trim('"');
                        var filePath = Path.Combine(_env.ContentRootPath, "Uploads", fileName);

                        using (var targetStream = System.IO.File.Create(filePath))
                        {
                            await section.Body.CopyToAsync(targetStream);
                        }

                        var command = new RegisterFileUploadQuery
                        {
                            FilePath = filePath,
                            FileSize = new FileInfo(filePath).Length,
                            Name = fileName
                        };

                        var result = await _mediator.Send(command);

                        if (result)
                        {
                            return Ok(new { FilePath = filePath });
                        }
                        else
                        {
                            return StatusCode(500, "Error al registrar la subida del archivo");
                        }
                    }
                }
            }

            return BadRequest("No se encontraron datos de archivos en la solicitud");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFile([FromBody] UpdateFileQuery command)
        {
            var result = await _mediator.Send(command);
            if (result)
            {
                return NoContent();
            }
            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var command = new DeleteFileQuery { Id = id };
            var result = await _mediator.Send(command);

            if (result)
            {
                return NoContent();
            }

            return NotFound();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFileById(int id)
        {
            var query = new GetFileByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFiles()
        {
            var query = new GetAllFilesQuery();
            var result = await _mediator.Send(query);

            if (result != null)
            {
                return Ok(result);
            }

            return NoContent();
        }
    }
}

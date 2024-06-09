using FM.Cqrs.Queries;
using System.Text;
using FM.Cqrs.Queries.Files;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using FM.Core.Models;

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
        public async Task<IActionResult> UploadStream([FromForm] FileDto fileDto, IFormFile? file)
        {
            var command = new RegisterFileUploadQuery
            {
                FilePath = null,
                FileSize = 0,
                Name = fileDto.Name,
                ParentId = fileDto.ParentId,
                IsDirectory = true
            };
            bool result = false;
            if (fileDto.IsDirectory)
            {

                if (fileDto.Id > 0)
                {
                    var updateCommand = new UpdateFileQuery
                    {
                        Id = fileDto.Id,
                        FilePath = null,
                        FileSize = 0,
                        Name = fileDto.Name,
                        ParentId = fileDto.ParentId,
                        IsDirectory = true
                    };

                    var updateResult = await _mediator.Send(updateCommand);

                    if (updateResult)
                    {
                        return Ok(updateResult);
                    }
                    return StatusCode(500, "Error al actualizar el directorio");
                }

                result = await _mediator.Send(command);
                if (result)
                {
                    return Ok(result);
                }
                return StatusCode(500, "Error al registrar el directorio");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("Archivo no proporcionado");
            }

            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(_env.ContentRootPath, "Uploads", fileName);
            var directoryPath = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (var targetStream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(targetStream);
            }

            command = new RegisterFileUploadQuery
            {
                FilePath = filePath,
                FileSize = file.Length,
                Name = fileName,
                ParentId = fileDto.ParentId,
                IsDirectory = false
            };

            if (fileDto.Id > 0)
            {
                var updateCommand = new UpdateFileQuery
                {
                    Id = fileDto.Id,
                    FilePath = filePath,
                    FileSize = file.Length,
                    Name = fileName,
                    ParentId = fileDto.ParentId,
                    IsDirectory = false
                };

                var updateResult = await _mediator.Send(updateCommand);

                if (updateResult)
                {
                    return Ok(updateResult);
                }
                return StatusCode(500, "Error al actualizar el archivo");
            }

            result = await _mediator.Send(command);

            if (result)
            {
                return Ok(new { FilePath = filePath });
            }
            else
            {
                return StatusCode(500, "Error al registrar la subida del archivo");
            }
        }


        //[HttpPost("upload-stream")]
        //public async Task<IActionResult> UploadStream([FromForm] FileDto fileDto)
        //{
        //    if (fileDto.IsDirectory)
        //    {
        //        var command = new RegisterFileUploadQuery
        //        {
        //            FilePath = null,
        //            FileSize = 0,
        //            Name = fileDto.Name,
        //            ParentId = fileDto.ParentId,
        //            IsDirectory = fileDto.IsDirectory
        //        };
        //        if (fileDto.Id > 0)
        //        {
        //            var updateCommand = new UpdateFileQuery
        //            {
        //                Id = fileDto.Id,
        //                FilePath = null,
        //                FileSize = 0,
        //                Name = fileDto.Name,
        //                ParentId = fileDto.ParentId,
        //                IsDirectory = fileDto.IsDirectory
        //            };

        //            var updateResult = await _mediator.Send(updateCommand);

        //            if (updateResult)
        //            {
        //                return Ok(updateResult);
        //            }
        //            return StatusCode(500, "Error al actualizar el directorio");
        //        }

        //        var result = await _mediator.Send(command);
        //        if (result)
        //        {
        //            return Ok(result);
        //        }
        //        return StatusCode(500, "Error al registrar el directorio");
        //    }

        //    //if (file == null || file.Length == 0)
        //    //{
        //    //    return BadRequest("Archivo no proporcionado");
        //    //}

        //    if (!Request.HasFormContentType ||
        //        !MediaTypeHeaderValue.TryParse(Request.ContentType, out var mediaTypeHeader) ||
        //        string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
        //    {
        //        return BadRequest("Solicitud no válida");
        //    }

        //    var boundary = HeaderUtilities.RemoveQuotes(mediaTypeHeader.Boundary).Value;
        //    var reader = new MultipartReader(boundary, HttpContext.Request.Body);


        //    MultipartSection section;
        //    while ((section = await reader.ReadNextSectionAsync()) != null)
        //    {
        //        if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition))
        //        {
        //            if (contentDisposition.IsFileDisposition())
        //            {
        //                var fileName = contentDisposition.FileName.Value.Trim('"');
        //                var filePath = Path.Combine(_env.ContentRootPath, "Uploads", fileName);
        //                var directoryPath = Path.GetDirectoryName(filePath);
        //                if(!Directory.Exists(directoryPath))
        //                {
        //                    Directory.CreateDirectory(directoryPath);
        //                }

        //                using (var targetStream = System.IO.File.Create(filePath))
        //                {
        //                    await section.Body.CopyToAsync(targetStream);
        //                }

        //                var command = new RegisterFileUploadQuery
        //                {
        //                    FilePath = filePath,
        //                    FileSize = new FileInfo(filePath).Length,
        //                    Name = fileName
        //                };

        //                var result = await _mediator.Send(command);

        //                if (result)
        //                {
        //                    return Ok(new { FilePath = filePath });
        //                }
        //                else
        //                {
        //                    return StatusCode(500, "Error al registrar la subida del archivo");
        //                }
        //            }
        //        }
        //    }

        //    return BadRequest("No se encontraron datos de archivos en la solicitud");
        //}

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

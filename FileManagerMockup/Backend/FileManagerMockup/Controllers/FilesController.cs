using FM.Cqrs.Queries;
using System.Text;
using FM.Cqrs.Queries.Files;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using FM.Core.Models;
using System.Net;

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
            var response = new ResponseDto();

            try
            {
                if (fileDto.IsDirectory)
                {
                    var command = new RegisterFileUploadQuery
                    {
                        FilePath = null,
                        FileSize = 0,
                        Name = fileDto.Name,
                        ParentId = fileDto.ParentId,
                        IsDirectory = true
                    };

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
                        response.IsSuccess = updateResult;
                        response.Status = updateResult ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
                        response.Message = updateResult ? "Directorio actualizado exitosamente" : "Error al actualizar el directorio";

                        return StatusCode((int)response.Status, response);
                    }

                    var result = await _mediator.Send(command);
                    response.IsSuccess = result;
                    response.Status = result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
                    response.Message = result ? "Directorio registrado exitosamente" : "Error al registrar el directorio";

                    return StatusCode((int)response.Status, response);
                }

                if (file == null || file.Length == 0)
                {
                    response.IsSuccess = false;
                    response.Status = HttpStatusCode.BadRequest;
                    response.Message = "Archivo no proporcionado";

                    return StatusCode((int)response.Status, response);
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

                var registerCommand = new RegisterFileUploadQuery
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
                    response.IsSuccess = updateResult;
                    response.Status = updateResult ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
                    response.Message = updateResult ? "Archivo actualizado exitosamente" : "Error al actualizar el archivo";

                    return StatusCode((int)response.Status, response);
                }

                var registerResult = await _mediator.Send(registerCommand);
                response.IsSuccess = registerResult;
                response.Status = registerResult ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
                response.Message = registerResult ? "Archivo registrado exitosamente" : "Error al registrar la subida del archivo";
                response.Data = new { FilePath = filePath };

                return StatusCode((int)response.Status, response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = HttpStatusCode.InternalServerError;
                response.Message = "Error procesando la solicitud";
                response.Exception = ex;

                return StatusCode((int)response.Status, response);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var response = new ResponseDto();

            try
            {
                var command = new DeleteFileQuery { Id = id };
                var result = await _mediator.Send(command);
                response.IsSuccess = result;
                response.Status = result ? HttpStatusCode.OK : HttpStatusCode.NotFound;
                response.Message = result ? "Archivo eliminado exitosamente" : "Archivo no encontrado";

                return StatusCode((int)response.Status, response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = HttpStatusCode.InternalServerError;
                response.Message = "Error al eliminar el archivo";
                response.Exception = ex;

                return StatusCode((int)response.Status, response);
            }
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

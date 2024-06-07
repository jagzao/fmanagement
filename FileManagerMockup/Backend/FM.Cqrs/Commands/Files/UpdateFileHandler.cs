using Dapper;
using FM.Cqrs.Queries.Files;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace FM.Cqrs.Commands.Files
{
    public class UpdateFileHandler : IRequestHandler<UpdateFileQuery, bool>
    {
        private readonly ILogger<UpdateFileHandler> _logger;
        private readonly string _connectionString;

        public UpdateFileHandler(ILogger<UpdateFileHandler> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> Handle(UpdateFileQuery request, CancellationToken cancellationToken)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = "UPDATE Files SET FilePath = @FilePath, FileSize = @FileSize, Name = @Name, ParentId = @ParentId, IsDirectory = @IsDirectory WHERE Id = @Id";
                    var parameters = new { request.FilePath, request.FileSize, request.Name, request.ParentId, request.IsDirectory, request.Id };

                    await connection.ExecuteAsync(sql, parameters);
                }

                _logger.LogInformation("Archivo actualizado exitosamente: {Id}, {FilePath}, {FileSize}, {Name}, {ParentId}, {IsDirectory}", request.Id, request.FilePath, request.FileSize, request.Name, request.ParentId, request.IsDirectory);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el archivo: {Id}, {FilePath}, {FileSize}, {Name}, {ParentId}, {IsDirectory}", request.Id, request.FilePath, request.FileSize, request.Name, request.ParentId, request.IsDirectory);
                return false;
            }
        }
    }
}

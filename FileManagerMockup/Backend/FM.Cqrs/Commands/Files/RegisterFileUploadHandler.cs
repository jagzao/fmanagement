using Dapper;
using FM.Cqrs.Queries.Files;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace FM.Cqrs.Commands.Files
{
    public class CreateFileHandler : IRequestHandler<RegisterFileUploadQuery, bool>
    {
        private readonly ILogger<CreateFileHandler> _logger;
        private readonly string _connectionString;

        public CreateFileHandler(ILogger<CreateFileHandler> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> Handle(RegisterFileUploadQuery request, CancellationToken cancellationToken)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = "INSERT INTO Files (FilePath, FileSize, Name, ParentId, IsDirectory) VALUES (@FilePath, @FileSize, @Name, @ParentId, @IsDirectory)";
                    var parameters = new { request.FilePath, request.FileSize, request.Name, request.ParentId, request.IsDirectory };

                    await connection.ExecuteAsync(sql, parameters);
                }

                _logger.LogInformation("Archivo registrado exitosamente: {FilePath}, {FileSize}, {Name}, {ParentId}, {IsDirectory}", request.FilePath, request.FileSize, request.Name, request.ParentId, request.IsDirectory);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar el archivo: {FilePath}, {FileSize}, {Name}, {ParentId}, {IsDirectory}", request.FilePath, request.FileSize, request.Name, request.ParentId, request.IsDirectory);
                return false;
            }
        }
    }
}

using Dapper;
using FM.Core.Models;
using FM.Cqrs.Queries.Files;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace FM.Cqrs.Commands.Files
{
    public class GetFileByIdHandler : IRequestHandler<GetFileByIdQuery, FileDto>
    {
        private readonly ILogger<GetFileByIdHandler> _logger;
        private readonly string _connectionString;

        public GetFileByIdHandler(ILogger<GetFileByIdHandler> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<FileDto> Handle(GetFileByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = "SELECT Id, FilePath, FileSize, Name, ParentId, IsDirectory FROM Files WHERE Id = @Id";
                    var parameters = new { request.Id };

                    var file = await connection.QueryFirstOrDefaultAsync<FileDto>(sql, parameters);

                    if (file == null)
                    {
                        _logger.LogWarning("Archivo no encontrado: {Id}", request.Id);
                        return null;
                    }

                    return file;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el archivo: {Id}", request.Id);
                return null;
            }
        }
    }
}

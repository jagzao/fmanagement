using Dapper;
using FM.Core.Models;
using FM.Cqrs.Queries.Files;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace FM.Cqrs.Commands.Files
{
    public class GetAllFilesHandler : IRequestHandler<GetAllFilesQuery, List<FileDto>>
    {
        private readonly ILogger<GetAllFilesHandler> _logger;
        private readonly string _connectionString;

        public GetAllFilesHandler(ILogger<GetAllFilesHandler> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<FileDto>> Handle(GetAllFilesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = "SELECT Id, FilePath, FileSize, Name, ParentId, IsDirectory FROM Files";
                    var files = await connection.QueryAsync<FileDto>(sql);

                    return files.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los archivos");
                return new List<FileDto>();
            }
        }
    }
}

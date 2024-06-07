using Dapper;
using FM.Cqrs.Queries;
using FM.Cqrs.Queries.Files;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace FM.Cqrs.Commands.Files
{
    public class DeleteFileHandler : IRequestHandler<DeleteFileQuery, bool>
    {
        private readonly ILogger<DeleteFileHandler> _logger;
        private readonly string _connectionString;

        public DeleteFileHandler(ILogger<DeleteFileHandler> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> Handle(DeleteFileQuery request, CancellationToken cancellationToken)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = "DELETE FROM Files WHERE Id = @Id";
                    var parameters = new { request.Id };

                    await connection.ExecuteAsync(sql, parameters);
                }

                _logger.LogInformation("Archivo eliminado exitosamente: {Id}", request.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el archivo: {Id}", request.Id);
                return false;
            }
        }
    }
}

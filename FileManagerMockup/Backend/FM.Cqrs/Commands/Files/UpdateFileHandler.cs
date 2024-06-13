using Dapper;
using FM.Cqrs.Queries.Files;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

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
                var sql = "CALL public.update_file(@Id, @FilePath, @FileSize, @Name, @ParentId, @IsDirectory)";
                var parameters = new { Id = request.Id, FilePath = request.FilePath, FileSize = request.FileSize, Name = request.Name, ParentId = request.ParentId, IsDirectory = request.IsDirectory };

                await connection.ExecuteAsync(sql, parameters);
            }

            _logger.LogInformation("Archivo actualizado exitosamente: {Id}", request.Id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el archivo: {Id}", request.Id);
            return false;
        }
    }
}
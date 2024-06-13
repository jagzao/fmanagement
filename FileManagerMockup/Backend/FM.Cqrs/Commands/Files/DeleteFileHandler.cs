using Dapper;
using FM.Cqrs.Queries.Files;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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
                // Obtener las rutas de los archivos a eliminar
                var sqlGetPaths = "SELECT FilePath FROM get_all_file_children(@FileId)";
                var parameters = new { FileId = request.Id };

                var filesToDelete = await connection.QueryAsync<string>(sqlGetPaths, parameters);

                // Eliminar los archivos físicos
                foreach (var filePath in filesToDelete)
                {
                    if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }

                // Llamar al procedimiento para eliminar registros de la base de datos
                var sqlDelete = "CALL delete_file_with_children(@FileId)";
                await connection.ExecuteAsync(sqlDelete, parameters);
            }

            _logger.LogInformation("Archivo y sus hijos eliminados correctamente: {FileId}", request.Id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el archivo y sus hijos: {FileId}", request.Id);
            return false;
        }
    }
}
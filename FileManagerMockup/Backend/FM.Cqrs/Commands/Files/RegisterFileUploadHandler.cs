using Dapper;
using FM.Cqrs.Queries.Files;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

public class RegisterFileUploadHandler : IRequestHandler<RegisterFileUploadQuery, bool>
{
    private readonly ILogger<RegisterFileUploadHandler> _logger;
    private readonly string _connectionString;

    public RegisterFileUploadHandler(ILogger<RegisterFileUploadHandler> logger, IConfiguration configuration)
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
                var sql = "CALL public.insert_file(@FilePath, @FileSize, @Name, @ParentId, @IsDirectory)";
                var parameters = new { request.FilePath, request.FileSize, request.Name, request.ParentId, request.IsDirectory };

                await connection.ExecuteAsync(sql, parameters);
            }

            _logger.LogInformation("Subida de archivo registrada exitosamente: {FilePath}, {FileSize}, {Name}", request.FilePath, request.FileSize, request.Name);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar la subida del archivo: {FilePath}, {FileSize}, {Name}", request.FilePath, request.FileSize, request.Name);
            return false;
        }
    }
}
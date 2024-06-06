using Dapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace FM.Cqrs.Queries
{
    public class RegisterFileUploadHandler : IRequestHandler<RegisterFileUploadCommand, bool>
    {
        private readonly ILogger<RegisterFileUploadHandler> _logger;
        private readonly string _connectionString;

        public RegisterFileUploadHandler(ILogger<RegisterFileUploadHandler> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> Handle(RegisterFileUploadCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = "INSERT INTO FileUploads (FilePath, FileSize, Name) VALUES (@FilePath, @FileSize, @Name)";
                    var parameters = new { request.FilePath, request.FileSize, request.Name };

                    await connection.ExecuteAsync(sql, parameters);
                }

                _logger.LogInformation("File upload registered successfully: {FilePath}, {FileSize}, {Name}", request.FilePath, request.FileSize, request.Name);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering file upload: {FilePath}, {FileSize}, {Name}", request.FilePath, request.FileSize, request.Name);
                return false;
            }
        }
    }
}

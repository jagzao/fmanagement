using Dapper;
using FM.Core.Models;
using FM.Cqrs.Queries.Files;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

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
                var sql = "SELECT * FROM get_file_by_id(@Id)";
                var parameters = new { Id = request.Id };

                var result = await connection.QueryFirstOrDefaultAsync<FileDto>(sql, parameters);
                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving file by id: {Id}", request.Id);
            return null;
        }
    }
}
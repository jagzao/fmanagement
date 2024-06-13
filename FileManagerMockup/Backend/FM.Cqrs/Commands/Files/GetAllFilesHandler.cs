using Dapper;
using FM.Core.Models;
using FM.Cqrs.Queries.Files;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

public class GetAllFilesHandler : IRequestHandler<GetAllFilesQuery, List<FileDto>>
{
    private readonly ILogger<GetAllFilesHandler> _logger;
    private readonly string _connectionString;

    public class GetAllFiles
    {
        public int file_id { get; set; }
        public string file_name { get; set; }
        public string file_parent_id { get; set; }
        public bool file_is_directory { get; set; }
        public long file_size { get; set; }
        public string file_path { get; set; }
    }

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
                var query = await connection.QueryAsync<GetAllFiles>("select * from public.get_all_files()", 
                    commandType: System.Data.CommandType.Text);
                var result = query.Select(s => new FileDto
                {
                    Id = s.file_id,
                    Name = s.file_name,
                    ParentId = int.TryParse(s.file_parent_id, out int parent) ? parent : 0,
                    IsDirectory = s.file_is_directory,
                    FileSize = s.file_size,
                    FilePath = s.file_path
                }).AsList();
                _logger.LogInformation("GetAllFilesQuery", result.Count);
                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving files");
            throw ex;
        }
    }
}
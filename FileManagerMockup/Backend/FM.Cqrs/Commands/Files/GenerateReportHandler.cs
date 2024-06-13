using Dapper;
using FM.Core.Models;
using FM.Cqrs.Queries.Files;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

public class GenerateReportHandler : IRequestHandler<GenerateReportQuery, IEnumerable<ReportDto>>
{
    private readonly ILogger<GenerateReportHandler> _logger;
    private readonly string _connectionString;

    public GenerateReportHandler(ILogger<GenerateReportHandler> logger, IConfiguration configuration)
    {
        _logger = logger;
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<IEnumerable<ReportDto>> Handle(GenerateReportQuery request, CancellationToken cancellationToken)
    {
        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var sql = "SELECT id, name, company, date FROM generate_report(@StartDate, @EndDate)";
                var parameters = new { StartDate = request.StartDate, EndDate = request.EndDate };

                var result = await connection.QueryAsync<ReportDto>(sql, parameters);
                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating report for dates: {StartDate} - {EndDate}", request.StartDate, request.EndDate);
            return Enumerable.Empty<ReportDto>();
        }
    }
}
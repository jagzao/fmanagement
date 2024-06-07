using Dapper;
using FM.Core.Models;
using FM.Cqrs.Queries.Files;
using MediatR;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace FM.Cqrs.Commands.Files
{
    public class GenerateReportHandler : IRequestHandler<GenerateReportQuery, IEnumerable<ReportDto>>
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public GenerateReportHandler(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<ReportDto>> Handle(GenerateReportQuery request, CancellationToken cancellationToken)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"
                SELECT Id, Name, Company, Date
                FROM Bills
                WHERE Date BETWEEN @StartDate AND @EndDate";

                var parameters = new { request.StartDate, request.EndDate };

                var result = await connection.QueryAsync<ReportDto>(query, parameters);
                return result;
            }
        }
    }
}

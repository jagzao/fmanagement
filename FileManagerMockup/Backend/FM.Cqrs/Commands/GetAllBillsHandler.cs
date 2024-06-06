using Dapper;
using FM.Core.Models;
using FM.Cqrs.Queries;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace FM.Cqrs.Commands
{
    public class GetAllBillsHandler : IRequestHandler<GetAllBillsQuery, ResponseDto>
    {
        private readonly ILogger<GetAllBillsHandler> _logger;
        private readonly string _connectionString;

        public GetAllBillsHandler(ILogger<GetAllBillsHandler> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<ResponseDto> Handle(GetAllBillsQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = "SELECT * FROM Bills";
                    var bills = await connection.QueryAsync(sql);

                    response.IsSuccess = true;
                    response.Message = "Bills retrieved successfully.";
                    response.Data = bills;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bills.");
                response.IsSuccess = false;
                response.Message = "An error occurred while retrieving the bills.";
                response.exception = ex;
            }

            return response;
        }
    }
}

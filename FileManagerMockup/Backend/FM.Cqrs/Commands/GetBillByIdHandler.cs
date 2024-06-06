using Dapper;
using FM.Core.Models;
using FM.Cqrs.Queries;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace FM.Cqrs.Commands
{
    public class GetBillByIdHandler : IRequestHandler<GetBillByIdQuery, ResponseDto>
    {
        private readonly ILogger<GetBillByIdHandler> _logger;
        private readonly string _connectionString;

        public GetBillByIdHandler(ILogger<GetBillByIdHandler> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<ResponseDto> Handle(GetBillByIdQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = "SELECT * FROM Bills WHERE Id = @BillId";
                    var bill = await connection.QueryFirstOrDefaultAsync(sql, new { BillId = request.BillId });

                    if (bill != null)
                    {
                        response.IsSuccess = true;
                        response.Message = "Bill retrieved successfully.";
                        response.Data = bill;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = "Bill not found.";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bill with ID {BillId}", request.BillId);
                response.IsSuccess = false;
                response.Message = "An error occurred while retrieving the bill.";
                response.exception = ex;
            }

            return response;
        }
    }
}

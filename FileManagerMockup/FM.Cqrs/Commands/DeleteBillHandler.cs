using Dapper;
using FM.Core.Models;
using FM.Cqrs.Queries;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace FM.Cqrs.Commands
{
    public class DeleteBillHandler : IRequestHandler<DeleteBillCommand, ResponseDto>
    {
        private readonly ILogger<DeleteBillHandler> _logger;
        private readonly string _connectionString;

        public DeleteBillHandler(ILogger<DeleteBillHandler> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<ResponseDto> Handle(DeleteBillCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = "DELETE FROM Bills WHERE Id = @BillId";
                    var parameters = new { request.BillId };

                    await connection.ExecuteAsync(sql, parameters);
                }

                _logger.LogInformation("Bill with ID {BillId} deleted successfully", request.BillId);
                return new ResponseDto
                {
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting bill with ID {BillId}", request.BillId);
                return new ResponseDto
                {
                    IsSuccess = false,
                    exception = ex
                };
            }
        }
    }
}

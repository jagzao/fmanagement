using Dapper;
using FM.Core.Models;
using FM.Cqrs.Queries;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace FM.Cqrs.Commands
{
    public class UpdateBillHandler : IRequestHandler<UpdateBillCommand, ResponseDto>
    {
        private readonly ILogger<UpdateBillHandler> _logger;
        private readonly string _connectionString;

        public UpdateBillHandler(ILogger<UpdateBillHandler> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<ResponseDto> Handle(UpdateBillCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = "UPDATE Bills SET Name = @Name, Amount = @Amount, DueDate = @DueDate WHERE Id = @BillId";
                    var parameters = new
                    {
                        request.Name,
                        request.Amount,
                        request.DueDate,
                        request.BillId
                    };

                    var affectedRows = await connection.ExecuteAsync(sql, parameters);

                    if (affectedRows > 0)
                    {
                        response.IsSuccess = true;
                        response.Message = "Bill updated successfully.";
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = "No bill found with the given ID.";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating bill with ID {BillId}", request.BillId);
                response.IsSuccess = false;
                response.Message = "An error occurred while updating the bill.";
                response.exception = ex;
            }

            return response;
        }
    }
}

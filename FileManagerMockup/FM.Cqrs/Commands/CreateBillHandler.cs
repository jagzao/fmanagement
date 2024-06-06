using Dapper;
using FM.Core.Models;
using FM.Cqrs.Queries;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace FM.Cqrs.Commands
{
    public class CreateBillHandler : IRequestHandler<CreateBillCommand, ResponseDto>
    {
        private readonly ILogger<CreateBillHandler> _logger;
        private readonly string _connectionString;

        public CreateBillHandler(ILogger<CreateBillHandler> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<ResponseDto> Handle(CreateBillCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = "INSERT INTO Bills (Name, Amount, PdfPath) VALUES (@Name, @Amount, @PdfPath)";
                    var parameters = new { request.Name, request.Amount, request.PdfPath };

                    await connection.ExecuteAsync(sql, parameters);
                }

                _logger.LogInformation("Bill created successfully for customer: {Name}", request.Name);
                return new ResponseDto
                {
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bill for customer: {Name}", request.Name);
                return new ResponseDto
                {
                    IsSuccess = false,
                    exception = ex
                };
            }
        }
    }
}

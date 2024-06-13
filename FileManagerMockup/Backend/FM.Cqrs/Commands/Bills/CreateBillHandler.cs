using Dapper;
using FM.Core.Models;
using FM.Cqrs.Queries;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace FM.Cqrs.Commands.Bills
{
    public class CreateBillHandler : IRequestHandler<CreateBillQuery, ResponseDto>
    {
        private readonly ILogger<CreateBillHandler> _logger;
        private readonly string _connectionString;

        public CreateBillHandler(ILogger<CreateBillHandler> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<ResponseDto> Handle(CreateBillQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Name.Length > 255)
                {
                    return new ResponseDto
                    {
                        IsSuccess = false,
                        Message = "La longitud es maximo de 255 caracteres"
                    };
                }
                if (request.Company.Length > 255)
                {
                    return new ResponseDto
                    {
                        IsSuccess = false,
                        Message = "La longitud es maximo de 255 caracteres"
                    };
                }

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = "INSERT INTO Bills (Name, Company) VALUES (@Name, @Company)";
                    var parameters = new { request.Name, request.Company };

                    await connection.ExecuteAsync(sql, parameters);
                }

                _logger.LogInformation("Factura creada para: {Name}", request.Name);
                return new ResponseDto
                {
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando factura para: {Name}", request.Name);
                return new ResponseDto
                {
                    IsSuccess = false,
                    Exception = ex
                };
            }
        }
    }
}

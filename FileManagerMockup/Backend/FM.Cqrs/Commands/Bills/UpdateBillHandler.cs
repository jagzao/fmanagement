using Dapper;
using FM.Core.Models;
using FM.Cqrs.Queries;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace FM.Cqrs.Commands.Bills
{
    public class UpdateBillHandler : IRequestHandler<UpdateBillQuery, ResponseDto>
    {
        private readonly ILogger<UpdateBillHandler> _logger;
        private readonly string _connectionString;

        public UpdateBillHandler(ILogger<UpdateBillHandler> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<ResponseDto> Handle(UpdateBillQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = "UPDATE Bills SET Name = @Name, Company = @Company WHERE Id = @Id";
                    var parameters = new
                    {
                        request.Name,
                        request.Company,
                        request.Id
                    };

                    var affectedRows = await connection.ExecuteAsync(sql, parameters);

                    if (affectedRows > 0)
                    {
                        response.IsSuccess = true;
                        response.Message = "Factura actualizada.";
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = "No se encontro la factura.";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando con ID {Id}", request.Id);
                response.IsSuccess = false;
                response.Message = "Error actualizando.";
                response.Exception = ex;
            }

            return response;
        }
    }
}

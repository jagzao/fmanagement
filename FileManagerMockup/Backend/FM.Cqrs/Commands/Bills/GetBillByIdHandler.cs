using Dapper;
using FM.Core.Models;
using FM.Cqrs.Queries;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace FM.Cqrs.Commands.Bills
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
                    var bill = await connection.QueryFirstOrDefaultAsync(sql, new { request.BillId });

                    if (bill != null)
                    {
                        response.IsSuccess = true;
                        response.Data = bill;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = "Factura no encontrada.";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo factura {BillId}", request.BillId);
                response.IsSuccess = false;
                response.Message = "Error obteniendo factura.";
                response.exception = ex;
            }

            return response;
        }
    }
}

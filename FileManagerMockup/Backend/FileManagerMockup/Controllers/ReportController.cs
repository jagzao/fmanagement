using MediatR;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
using FM.Core.Models;
using FM.Cqrs.Queries.Files;

namespace FileManagerMockup.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("generate")]
        public async Task<IActionResult> GenerateReport(DateTime startDate, DateTime endDate)
        {
            var query = new GenerateReportQuery { StartDate = startDate, EndDate = endDate };
            var result = await _mediator.Send(query);

            if (result != null)
            {
                return Ok(result);
            }

            return NoContent();
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("download")]
        public async Task<IActionResult> DownloadReport(DateTime startDate, DateTime endDate)
        {
            var query = new GenerateReportQuery { StartDate = startDate, EndDate = endDate };
            var result = await _mediator.Send(query);

            if (result != null)
            {
                var csv = GenerateCsv(result);
                var bytes = Encoding.UTF8.GetBytes(csv);
                var output = new MemoryStream(bytes);

                return File(output, "text/csv", "reporte.csv");
            }

            return NoContent();
        }

        private string GenerateCsv(IEnumerable<ReportDto> reportData)
        {
            var sb = new StringBuilder();
            sb.AppendLine("ID,Nombre,Compania,Fecha");

            foreach (var item in reportData)
            {
                sb.AppendLine($"{item.Id},{item.Name},{item.Company},{item.Date}");
            }

            return sb.ToString();
        }
    }

}

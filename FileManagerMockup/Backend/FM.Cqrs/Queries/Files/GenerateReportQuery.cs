using FM.Core.Models;
using MediatR;

namespace FM.Cqrs.Queries.Files
{
    public class GenerateReportQuery : IRequest<IEnumerable<ReportDto>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

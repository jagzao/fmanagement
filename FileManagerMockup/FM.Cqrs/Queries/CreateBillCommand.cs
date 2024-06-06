using FM.Core.Models;
using MediatR;

namespace FM.Cqrs.Queries
{
    public class CreateBillCommand : IRequest<ResponseDto>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string PdfPath { get; set; }
    }
}

using FM.Core.Models;
using MediatR;

namespace FM.Cqrs.Queries
{
    public class GetBillByIdQuery : IRequest<ResponseDto>
    {
        public int BillId { get; set; }
    }
}

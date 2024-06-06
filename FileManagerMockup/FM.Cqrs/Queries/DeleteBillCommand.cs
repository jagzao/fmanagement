using FM.Core.Models;
using MediatR;

namespace FM.Cqrs.Queries
{
    public class DeleteBillCommand : IRequest<ResponseDto>
    {
        public int BillId { get; set; }
    }
}

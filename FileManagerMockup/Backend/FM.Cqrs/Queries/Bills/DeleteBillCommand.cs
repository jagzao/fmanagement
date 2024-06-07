using FM.Core.Models;
using MediatR;

namespace FM.Cqrs.Queries
{
    public class DeleteBillCommand : IRequest<ResponseDto>
    {
        public int Id { get; set; }
    }
}

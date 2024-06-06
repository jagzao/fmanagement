using FM.Core.Models;
using MediatR;

namespace FM.Cqrs.Queries
{
    public class GetAllBillsQuery : IRequest<ResponseDto>
    {
    }
}

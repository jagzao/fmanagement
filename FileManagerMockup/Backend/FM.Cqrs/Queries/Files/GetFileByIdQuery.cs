using FM.Core.Models;
using MediatR;

namespace FM.Cqrs.Queries.Files
{
    public class GetFileByIdQuery : IRequest<FileDto>
    {
        public int Id { get; set; }
    }
}

using FM.Core.Models;
using MediatR;

namespace FM.Cqrs.Queries.Files
{
    public class GetAllFilesQuery : IRequest<List<FileDto>>
    {
    }
}

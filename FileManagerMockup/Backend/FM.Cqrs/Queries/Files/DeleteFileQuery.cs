using MediatR;

namespace FM.Cqrs.Queries.Files
{
    public class DeleteFileQuery : IRequest<bool>
    {
        public int Id { get; set; }
    }
}

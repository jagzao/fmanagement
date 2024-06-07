using MediatR;

namespace FM.Cqrs.Queries.Files
{
    public class UpdateFileQuery : IRequest<bool>
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public bool IsDirectory { get; set; }
    }
}

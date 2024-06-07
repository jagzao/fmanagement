using MediatR;

namespace FM.Cqrs.Queries.Files
{
    public class RegisterFileUploadQuery : IRequest<bool>
    {
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public bool IsDirectory { get; set; }
    }
}

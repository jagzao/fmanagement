using MediatR;

namespace FM.Cqrs.Queries
{
    public class RegisterFileUploadCommand : IRequest<bool>
    {
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public string Name { get; set; }
    }
}

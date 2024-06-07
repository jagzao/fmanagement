namespace FM.Core.Models
{
    public class FileDto
    {
        public int Id { get; set; }
        public long FileSize { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
        public bool IsDirectory { get; set; }
        public List<FileDto> Children { get; set;}
    }
}

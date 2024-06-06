namespace FM.Core.Models
{
    public class TreeNodeDto
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public bool IsDirectory { get; set; }
        public List<TreeNodeDto> Children { get; set; }
    }
}

namespace ActiveOfficeLife.Common.Models
{
    public class TagModel
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; } = null!;
        public string? Slug { get; set; } = null!;
        public Guid? SeoMetadataId { get; set; }
        public SeoMetadataModel? SeoMetadata { get; set; }
    }
}

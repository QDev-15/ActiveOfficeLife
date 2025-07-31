namespace ActiveOfficeLife.Common.Models
{
    public class SeoMetadataModel
    {
        public Guid Id { get; set; }

        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

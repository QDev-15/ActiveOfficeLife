namespace ActiveOfficeLife.Common.Models
{
    public class CategoryModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Description { get; set; }

        // SEO
        public Guid? SeoMetadataId { get; set; }
        public SeoMetadataModel? SeoMetadata { get; set; }

        public Guid? ParentId { get; set; }                   // FK đến Category.Id (có thể null nếu là cha)
        public CategoryModel? Parent { get; set; }                 // Category cha
        public ICollection<CategoryModel> Children { get; set; } = new List<CategoryModel>(); // Các chuyên mục con

        // list of all children IDs (bao gồm con cháu)
        public List<Guid> AllChildrenIds
        {
            get
            {
                var allIds = new List<Guid>();
                GetAllChildrenIds(this, allIds);
                return allIds;
            }
            private set { } // Chỉ cho phép đọc, không cho phép ghi trực tiếp
        }

        // Fix for CS0103: Implement the missing GetAllChildrenIds method
        private void GetAllChildrenIds(CategoryModel category, List<Guid> allIds)
        {
            foreach (var child in category.Children)
            {
                allIds.Add(child.Id);
                GetAllChildrenIds(child, allIds); // Recursive call to get IDs of all descendants
            }
        }
    }
}

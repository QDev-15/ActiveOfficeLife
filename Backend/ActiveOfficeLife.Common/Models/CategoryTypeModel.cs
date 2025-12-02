namespace ActiveOfficeLife.Common.Models
{
    public class CategoryTypeModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        
        public bool IsActive { get; set; } = true; // Trạng thái hoạt động của chuyên mục
        public ICollection<CategoryModel> Categories { get; set; } = new List<CategoryModel>();
    }
}

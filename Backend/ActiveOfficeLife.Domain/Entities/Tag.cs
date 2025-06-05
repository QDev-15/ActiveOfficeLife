using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Entities
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public Guid? SeoMetadataId { get; set; }
        public SeoMetadata? SeoMetadata { get; set; }
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }

}

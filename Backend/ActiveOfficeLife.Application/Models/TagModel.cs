using ActiveOfficeLife.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Models
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

﻿using ActiveOfficeLife.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Entities
{
    public class Post
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? Summary { get; set; }

        public Guid AuthorId { get; set; }
        public User Author { get; set; } = null!;

        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public PostStatus Status { get; set; } = PostStatus.Draft;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();

        public Guid? SeoMetadataId { get; set; }
        public SeoMetadata? SeoMetadata { get; set; }
    }

}

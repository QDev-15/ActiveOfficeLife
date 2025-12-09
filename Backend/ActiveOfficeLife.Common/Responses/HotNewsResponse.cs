using ActiveOfficeLife.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Common.Responses
{
    public class HotNewsResponse
    {
        public List<PostModel> FeaturedHome { get; set; } = new List<PostModel>();
        public List<PostModel> CenterHighlight { get; set; } = new List<PostModel>();
        public List<PostModel> HotNews { get; set; } = new List<PostModel>();
    }
}

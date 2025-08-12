namespace ActiveOfficeLife.Admin.Models
{
    public class DialogConfig
    {
        public string Title { get; set; }
        public string Icon { get; set; } // HTML icon
        public string BodyId { get; set; }
        public string BodyHtml { get; set; }
        public bool ShowClose { get; set; } = true;
        public bool ShowSave { get; set; } = false;
        public string SaveText { get; set; } = "Save";
        public bool ShowUpdate { get; set; } = false;
        public string UpdateText { get; set; } = "Update";
    }
}

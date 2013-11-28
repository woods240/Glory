
namespace WebSite
{
    public class BatchImportViewModel
    {
        public string Title { get; set; }
        public string TemplatePath { get; set; }
        public string TemplateDownloadName { get; set; }
        public string SheetName { get; set; }

        public UploadSettingsViewModel UploadSettings { get; set; }
    }
}
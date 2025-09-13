namespace SupaBaseIntegration.Dtos.Vdocipher
{
    public class VideoUploadRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();
        public bool FolderId { get; set; } = false;
    }
}

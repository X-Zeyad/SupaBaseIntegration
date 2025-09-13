namespace SupaBaseIntegration.Dtos.Vdocipher
{
    public class VideoInfo
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public long Length { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ThumbnailUrl { get; set; }
        public Dictionary<string, string> Tags { get; set; }
    }
}

namespace SupaBaseIntegration.Dtos.Vdocipher
{
    public class VideoListResponse
    {
        public List<VideoInfo> Videos { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public bool HasNext { get; set; }
    }
}

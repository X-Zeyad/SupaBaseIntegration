using Supabase.Postgrest.Attributes;

namespace SupaBaseIntegration.Models
{
    public class Notfication
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Type { get; set; }
    }
}

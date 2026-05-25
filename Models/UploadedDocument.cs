namespace CapstoneProj.Models
{
    public class UploadedDocument
    {
        public string DocumentId { get; set; } = Guid.NewGuid().ToString();
        public string FileName { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public long Size { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
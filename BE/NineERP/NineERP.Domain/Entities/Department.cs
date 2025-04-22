using NineERP.Domain.Entities.Base;

namespace NineERP.Domain.Entities
{
    public class Department : AuditableBaseEntity<short>
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public string? BannerUrl { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? FaceBookUrl { get; set; }
        public string? WikipediaUrl { get; set; }
        public string? YoutubeUrl { get; set; }
    }
}

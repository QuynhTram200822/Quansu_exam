namespace NineERP.Application.Dtos.Department
{
    public class DepartmentDto
    {
        public short Id { get; set; }
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public string? BannerUrl { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? FaceBookUrl { get; set; }
        public string? WikipediaUrl { get; set; }
        public string? YoutubeUrl { get; set; }
    }
}

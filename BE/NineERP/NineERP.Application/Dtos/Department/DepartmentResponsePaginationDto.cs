namespace NineERP.Application.Dtos.Department
{
    public class DepartmentResponsePaginationDto
    {
        public short Id { get; set; }
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }
}

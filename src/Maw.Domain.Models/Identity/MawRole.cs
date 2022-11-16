namespace Maw.Domain.Models.Identity;

public class MawRole
{
    public short Id { get; set; }
    public required string? Name { get; set; }
    public required string? Description { get; set; }
}

namespace Maw.Domain.Models.Identity;

public class MawRole
{
    public short Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}

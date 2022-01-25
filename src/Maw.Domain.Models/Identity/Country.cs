namespace Maw.Domain.Models.Identity;

public class Country
{
    public short Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}

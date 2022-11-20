namespace Maw.Domain.Models;

public class CategoryAndRoles
{
    public short Id { get; set; }
    public string[] Roles { get; set; } = Array.Empty<string>();
}

namespace Maw.Domain.Models;

public class SecuredResource<T>
{
    public T Item { get; }
    public string[] Roles { get; }

    public SecuredResource(T item, string[] roles)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(roles);

        Item = item;
        Roles = roles;
    }
}

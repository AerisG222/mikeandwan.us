namespace Maw.Domain.Models;

public class SecuredResource<T>
{
    public T Item { get; }
    public string[] Roles { get; }

    public SecuredResource(T item, string[] roles)
    {
        Item = item ?? throw new ArgumentNullException(nameof(item));
        Roles = roles ?? throw new ArgumentNullException(nameof(roles));
    }
}

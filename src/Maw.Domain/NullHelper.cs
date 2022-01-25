using System.Diagnostics.CodeAnalysis;

namespace Maw.Domain;

public static class NullHelper
{
    public static void ThrowIfNull<T>([NotNull] T value)
    {
        _ = value ?? throw new ArgumentNullException(nameof(value));
    }
}

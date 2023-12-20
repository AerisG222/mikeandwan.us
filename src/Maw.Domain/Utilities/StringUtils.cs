using System.Text;

namespace Maw.Domain.Utilities;

public static class StringUtils
{
    public static string ToHexString(byte[] byteArray)
    {
        ArgumentNullException.ThrowIfNull(byteArray);

        StringBuilder builder = new(byteArray.Length);

        foreach (byte b in byteArray)
        {
            builder.Append(FormattableString.Invariant($"{b:X}"));
        }

        return builder.ToString();
    }
}

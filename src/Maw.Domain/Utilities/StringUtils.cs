using System.Text;

namespace Maw.Domain.Utilities;

public static class StringUtils
{
    public static string ToHexString(byte[] byteArray)
    {
        if (byteArray == null)
        {
            throw new ArgumentNullException(nameof(byteArray));
        }

        StringBuilder builder = new(byteArray.Length);

        foreach (byte b in byteArray)
        {
            builder.Append(FormattableString.Invariant($"{b:X}"));
        }

        return builder.ToString();
    }
}

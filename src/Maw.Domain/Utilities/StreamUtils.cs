using System.IO;
using System.Text;

namespace Maw.Domain.Utilities;

public static class StreamUtils
{
    public static Stream ConvertStringToStream(string input)
    {
        var ms = new MemoryStream();

        using (var writer = new StreamWriter(ms, Encoding.UTF8, 4096, true))
        {
            writer.Write(input);
            writer.Flush();

            ms.Seek(0, SeekOrigin.Begin);
        }

        return ms;
    }
}

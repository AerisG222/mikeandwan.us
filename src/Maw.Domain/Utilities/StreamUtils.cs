using System.IO;


namespace Maw.Domain.Utilities
{
    public static class StreamUtils
    {
        public static Stream ConvertStringToStream(string input)
        {
            MemoryStream ms = new MemoryStream();

            StreamWriter writer = new StreamWriter(ms);
            writer.Write(input);
            writer.Flush();

            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }
    }
}

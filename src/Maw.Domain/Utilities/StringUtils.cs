using System;
using System.Text;


namespace Maw.Domain.Utilities
{
    public static class StringUtils
    {
        public static string ToHexString(byte[] byteArray)
        {
            if(byteArray == null)
            {
                throw new ArgumentNullException(nameof(byteArray));
            }

            var size = byteArray.Length;

	        StringBuilder builder = new StringBuilder(byteArray.Length);

	        foreach(byte b in byteArray)
	        {
	            builder.Append($"{b:X}");
	        }

	        return builder.ToString();
        }
    }
}

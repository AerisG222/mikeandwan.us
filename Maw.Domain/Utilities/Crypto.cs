using System;
using System.Security.Cryptography;
using System.Text;


namespace Maw.Domain.Utilities
{
    public class Crypto
    {
        const string _passwordChars = "~!@#$%^&*{}[];<>/?\\abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        

        public string Hash(string value)
        {
            var sha256 = SHA256.Create();
            var input = Encoding.UTF8.GetBytes(value);
            var output = sha256.ComputeHash(input);

            return Convert.ToBase64String(output);
        }

        
        public string GenerateRandom(int size)
		{
			var randomBytes = new byte[size];
	        var rand = RandomNumberGenerator.Create();
	
	        rand.GetBytes(randomBytes);
	
	        StringBuilder builder = new StringBuilder(size);
	
	        foreach(byte b in randomBytes)
	        {
	            builder.AppendFormat("{0:X}", b);
	        }
	
	        return builder.ToString().Substring(0, size);
		}


        public string GeneratePassword(int length)
        {
            var sb = new StringBuilder();
            var rnd = new Random();
            
            while (0 < length--)
            {
                sb.Append(_passwordChars[rnd.Next(_passwordChars.Length)]);
            }
            
            return sb.ToString();
        }
    }
}

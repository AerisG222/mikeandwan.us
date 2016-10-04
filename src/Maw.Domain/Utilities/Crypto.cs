using System;
using System.Security.Cryptography;
using System.Text;


namespace Maw.Domain.Utilities
{
    public class Crypto
    {
        const string _passwordChars = "~!@#$%^&*{}[];<>/?\\abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        

        public byte[] Hash(string value)
        {
            var sha256 = SHA256.Create();
            var input = Encoding.UTF8.GetBytes(value);
            var output = sha256.ComputeHash(input);

            return output;
        }

        
        public byte[] GenerateRandom(int size)
		{
			var randomBytes = new byte[size];
	        
            using(var rand = RandomNumberGenerator.Create())
            {
                rand.GetBytes(randomBytes);
            }
	
            return randomBytes;
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

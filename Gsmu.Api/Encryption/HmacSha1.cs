using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace Gsmu.Api.Encryption
{
    public class HmacSha1
    {
        public static string Encode(string value, string key)
        {
            byte[] keyByte = Encoding.ASCII.GetBytes(key);
            HMACSHA1 myhmacsha1 = new HMACSHA1(keyByte);
            byte[] byteArray = Encoding.ASCII.GetBytes(value);
            MemoryStream stream = new MemoryStream(byteArray);

            string hash = myhmacsha1.ComputeHash(stream).Aggregate("", (s, e) => s + String.Format("{0:x2}", e), s => s);
            return hash;
        }
    }
}

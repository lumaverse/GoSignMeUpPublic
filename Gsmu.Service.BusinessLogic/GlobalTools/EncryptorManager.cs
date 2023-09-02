using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.BusinessLogic.GlobalTools
{
    public static class EncryptorManager
    {
        public static string SHA1Encryption(string value)
        {
            if (value != null)
            {
                byte[] hashBytes = Convert.FromBase64String(value);
                var sha1 = new SHA1CryptoServiceProvider();
                var encryptedValue = sha1.ComputeHash(hashBytes);
                return Convert.ToBase64String(encryptedValue);
            }
            return value;
        }

        public static string Encryptsha256(string value)
        {
            if (value != null)
            {

                System.Security.Cryptography.SHA256Managed crypt = new System.Security.Cryptography.SHA256Managed();

                System.Text.StringBuilder hash = new System.Text.StringBuilder();

                byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(value), 0, Encoding.UTF8.GetByteCount(value));

                foreach (byte theByte in crypto)
                {

                    hash.Append(theByte.ToString("x2"));

                }

                return Convert.ToBase64String(crypto);
            }
            return value;

        }
    }

public class StringEncryption
{
    private readonly Random random;
    private readonly byte[] key;
    private readonly RijndaelManaged rm;
    private readonly UTF8Encoding encoder;

    public StringEncryption()
    {
        this.random = new Random();
        this.rm = new RijndaelManaged();
        this.encoder = new UTF8Encoding();
        this.key = Convert.FromBase64String("gosignmeup");
    }

    public string Encrypt(string unencrypted)
    {
        var vector = new byte[16];
        this.random.NextBytes(vector);
        var cryptogram = vector.Concat(this.Encrypt(this.encoder.GetBytes(unencrypted), vector));
        return Convert.ToBase64String(cryptogram.ToArray());
    }

    public string Decrypt(string encrypted)
    {
        var cryptogram = Convert.FromBase64String(encrypted);
        if (cryptogram.Length < 17)
        {
            throw new ArgumentException("Not a valid encrypted string", "encrypted");
        }

        var vector = cryptogram.Take(16).ToArray();
        var buffer = cryptogram.Skip(16).ToArray();
        return this.encoder.GetString(this.Decrypt(buffer, vector));
    }

    private byte[] Encrypt(byte[] buffer, byte[] vector)
    {
        var encryptor = this.rm.CreateEncryptor(this.key, vector);
        return this.Transform(buffer, encryptor);
    }

    private byte[] Decrypt(byte[] buffer, byte[] vector)
    {
        var decryptor = this.rm.CreateDecryptor(this.key, vector);
        return this.Transform(buffer, decryptor);
    }

    private byte[] Transform(byte[] buffer, ICryptoTransform transform)
    {
        var stream = new MemoryStream();
        using (var cs = new CryptoStream(stream, transform, CryptoStreamMode.Write))
        {
            cs.Write(buffer, 0, buffer.Length);
        }

        return stream.ToArray();
    }
}
}

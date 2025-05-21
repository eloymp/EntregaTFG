using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinalEMP.Cryptography
{
    class CryptographyPassword
    {
        private static readonly string clave = "j8gOGa2ociSRPnCP6Yca";
        private static readonly byte[] salt = Encoding.UTF8.GetBytes("SalFijaParaAES");

        public static string Cifrar(string textoPlano)
        {
            using var aes = Aes.Create();
            var key = new Rfc2898DeriveBytes(clave, salt, 10000);
            aes.Key = key.GetBytes(32); // 256 bits
            aes.IV = key.GetBytes(16);  // 128 bits

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(textoPlano);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public static string Descifrar(string textoCifrado)
        {
            using var aes = Aes.Create();
            var key = new Rfc2898DeriveBytes(clave, salt, 10000);
            aes.Key = key.GetBytes(32);
            aes.IV = key.GetBytes(16);

            var bytes = Convert.FromBase64String(textoCifrado);

            using var ms = new MemoryStream(bytes);
            using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
}

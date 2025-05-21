using System.Security.Cryptography;
using System.Text;

namespace ProyectoFinalEMP.Cryptography
{
    public class HashAdministrator
    {
        public string GenerarHash(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password); // Convertir string a bytes
                byte[] hash = sha256.ComputeHash(bytes); // Obtener el hash
                return Convert.ToHexString(hash).ToLower(); // Convertir a string 
            }
        }
    }
}

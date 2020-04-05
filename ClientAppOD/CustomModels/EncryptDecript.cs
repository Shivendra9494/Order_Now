using System;
using System.Security.Cryptography;
using System.Text;

namespace ClientAppOD.CustomModels
{
    public static class EncryptDecrypt
    {
        static string PublicKey { get { return @"<RSAKeyValue><Modulus>vITIzVzmYKjQJ53XabFZsTnKPFVNKm1hv+lJIF87VKkOwnFOSLwI/VrKQBqz8LSs5oHQ8rIvAE7s+yquIT6c3ziZjk4/nRqmcK4RfnBWfSMo9515+YIg6bU8V7b4vGrjfJVDlxN9qXi6JSijOUDqNq1NzCIyflL11hDkqMvFqHc=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>"; } }
        static string PrivateKey { get { return @"<RSAKeyValue><Modulus>vITIzVzmYKjQJ53XabFZsTnKPFVNKm1hv+lJIF87VKkOwnFOSLwI/VrKQBqz8LSs5oHQ8rIvAE7s+yquIT6c3ziZjk4/nRqmcK4RfnBWfSMo9515+YIg6bU8V7b4vGrjfJVDlxN9qXi6JSijOUDqNq1NzCIyflL11hDkqMvFqHc=</Modulus><Exponent>AQAB</Exponent><P>0voEbQPLrA0tH8a4CE1lR0nycgCkL/P34quYlYhelMBWSkyFb3SkqT23xTLbk4HU25QRYUKCll1NAISb4ZKfjw==</P><Q>5L/azltIqWr8uSuvDwL8NCRB2FvD72LeyBgdgQAIUnnkjyZ7qxMamChBOyTGeKJW8ZElTsvVPLg8H4upi/D0mQ==</Q><DP>lkQI3vRzHkoMN7O74/3sAsiCa/xU9OqZRdLeTLLiWqRbUXQLHgVAOmKA/21nwzoXt4VQk6thg2NUsufAdvuNtQ==</DP><DQ>g9lt9km5bSWku9rJEZ8H3coURfBG1KGpha4Yu3VYVqm5qyVkXOwrBQ8W4k9FMt1nvd+KoItuwovy47/tnyLo6Q==</DQ><InverseQ>EbQQmmdaDOjrlnugMLNgvHzxWx9FxQ7YAZrYGSQQXXPfuacSc8tC1vKrlVE4OUNQ5cyLVTz99LF2BvSZIbLldw==</InverseQ><D>AcLLGLamROnew+KiuU8u83ZYOsG146W7lNjU1jNoMPgWoLSr6AcFnc7kIRQpcR/8QMzz9Yhk4v+vEVY1OS9GCir4K+ulDZyrPCh6eV6GAup0AVC8mnfjQ6lEi4dISqwyVQsFku5ikapW+XCeQCPTORUd8UNsg1riGVCN6Khf5/k=</D></RSAKeyValue>"; } }


        public static string Encrypt(this string text, string publicKey = "")
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "";
            }
            if (publicKey == "")
            {
                publicKey = PublicKey;
            }
            // Convert the text to an array of bytes   
            // UnicodeEncoding byteConverter = new UnicodeEncoding();
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(text);// byteConverter.GetBytes(text);

            // Create a byte array to store the encrypted data in it   
            byte[] encryptedData;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                // Set the rsa pulic key   
                rsa.FromXmlString(publicKey);

                // Encrypt the data and store it in the encyptedData Array   
                encryptedData = rsa.Encrypt(dataToEncrypt, true);
                return Convert.ToBase64String(encryptedData);

                //  return byteConverter.GetString(encryptedData);
            }
            // Save the encypted data array into a file   
            // File.WriteAllBytes(fileName, encryptedData);

            // Console.WriteLine("Data has been encrypted");
        }

        // Method to decrypt the data withing a specific file using a RSA algorithm private key   
        public static string Decrypt(this string encryptedText, string privateKey = "")
        {
            if (string.IsNullOrWhiteSpace(encryptedText))
            {
                return "";
            }
            if (privateKey == "")
            {
                privateKey = PrivateKey;
            }
            // UnicodeEncoding byteConverter = new UnicodeEncoding();

            byte[] dataToDecrypt = Convert.FromBase64String(encryptedText);// byteConverter.GetBytes(encryptedText);// read the encrypted bytes from the file   
            //byte[] dataToDecrypt = File.ReadAllBytes(fileName);

            // Create an array to store the decrypted data in it   
            byte[] decryptedData;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                // Set the private key of the algorithm   
                rsa.FromXmlString(privateKey);
                decryptedData = rsa.Decrypt(dataToDecrypt, true);
            }

            // Get the string value from the decryptedData byte array   
            // UnicodeEncoding byteConverter = new UnicodeEncoding();
            return Encoding.UTF8.GetString(decryptedData);
            // return byteConverter.GetString(decryptedData);
        }
    }
}

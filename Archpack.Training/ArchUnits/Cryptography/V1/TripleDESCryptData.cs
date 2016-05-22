using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;
using System.IO;
using Archpack.Training.ArchUnits.Contracts.V1;

namespace Archpack.Training.ArchUnits.Cryptography.V1
{
    /// <summary>
    /// コンテンツを暗号化・複合化
    /// </summary>
    public static class TripleDESCryptData
    {
        /// <summary>
        /// コンテンツを暗号化します
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string Encrypt(string content)
        {
            Contract.NotEmpty(content, "content");
            string result = string.Empty;

            using (TripleDESCryptoServiceProvider algorithm = new TripleDESCryptoServiceProvider())
            {
                byte[] byteContent = Encoding.UTF8.GetBytes(content);
                byte[] key = Encoding.UTF8.GetBytes(Properties.Settings.Default.TripleDESCryptDataKey);
                Array.Resize(ref key, 24); // 3DES need 192bit key.

                algorithm.Key = key;
                algorithm.Mode = CipherMode.ECB;
                algorithm.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform transfrom = algorithm.CreateEncryptor())
                {
                    byte[] encryptedData = transfrom.TransformFinalBlock(byteContent, 0, byteContent.Length);
                    result = Convert.ToBase64String(encryptedData);
                }

            }
            
            return result;
        }

        /// <summary>
        /// コンテンツを複合化します
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string Decrypt(string content)
        {
            Contract.NotEmpty(content, "content");

            string result = string.Empty;
            try
            {
                // create algorithm 3DES.
                using (TripleDESCryptoServiceProvider algorithm = new TripleDESCryptoServiceProvider())
                {
                    byte[] byteContent = Convert.FromBase64String(content);
                    byte[] key = Encoding.UTF8.GetBytes(Properties.Settings.Default.TripleDESCryptDataKey);
                    Array.Resize(ref key, 24); // 3DES need 192bit key.

                    algorithm.Key = key;
                    algorithm.Mode = CipherMode.ECB;
                    algorithm.Padding = PaddingMode.PKCS7;

                    using (ICryptoTransform transfrom = algorithm.CreateDecryptor())
                    {
                        using (MemoryStream ms = new MemoryStream(byteContent, 0, byteContent.Length))
                        {
                            byte[] decryptedData = transfrom.TransformFinalBlock(byteContent, 0, byteContent.Length);
                            result = Encoding.UTF8.GetString(decryptedData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CryptException(ex);
            }            

            return result;

        }

    }
}

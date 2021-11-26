using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LitFramework.Crypto
{
    public class CryptoSHA1:ICrypto
    {
        public string GetHash( string content )
        {
            string hashKey = "As#@$%$^_kLLHW10278+{";
            using ( SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider() )
            {
                string hashCode = BitConverter.ToString( sha1.ComputeHash( Encoding.UTF8.GetBytes( hashKey ) ) ).Replace( "-", "" ) + BitConverter.ToString( sha1.ComputeHash( Encoding.UTF8.GetBytes( content ) ) ).Replace( "-", "" );
                return BitConverter.ToString( sha1.ComputeHash( Encoding.UTF8.GetBytes( hashCode ) ) ).Replace( "-", "" );
            }
        }

        public bool VerifyHash( string content, string hash )
        {
            string hashOfInput = GetHash( content );
            StringComparer stringCompare = StringComparer.OrdinalIgnoreCase;
            return stringCompare.Compare( hashOfInput, hash ) == 0 ? true : false;
        }

        public string GetFileHash( string filePath )
        {
            using ( SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider() )
            using ( FileStream fs = new FileStream( filePath, FileMode.Open, FileAccess.Read, FileShare.Read ) )
            {
                return BitConverter.ToString( sha1.ComputeHash( fs ) ).Replace( "-", "" );
            }
        }

    }
}

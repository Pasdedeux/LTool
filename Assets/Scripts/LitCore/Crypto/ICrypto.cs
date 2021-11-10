using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LitFramework.Crypto
{
    interface ICrypto
    {
        string GetHash( string content );


        bool VerifyHash( string content, string hash );


        string GetFileHash( string filePath );
    }
}

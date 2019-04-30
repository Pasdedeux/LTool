/*======================================
* 项目名称 ：LitFramework.Crypto
* 项目描述 ：
* 类 名 称 ：CryptoMD5
* 类 描 述 ：
* 命名空间 ：LitFramework.Crypto
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2019/4/30 13:57:35
* 更新时间 ：2019/4/30 13:57:35
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Inplayable 2019. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2019/4/30 13:57:35
*修改人： Derek Liu
*版本号： V1.0.0.0
*描述：
*
======================================*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LitFramework.Crypto
{
    internal class CryptoMD5
    {
        internal string GetMD5Hash( string content )
        {
            string hashKey = "As#@$%$^_kL+{";
            using ( MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider() )
            {
                string hashCode = BitConverter.ToString( md5.ComputeHash( UTF8Encoding.Default.GetBytes( hashKey ) ) ).Replace( "-", "" ) + BitConverter.ToString( md5.ComputeHash( UTF8Encoding.Default.GetBytes( content ) ) ).Replace( "-", "" );
                return BitConverter.ToString( md5.ComputeHash( UTF8Encoding.Default.GetBytes( hashCode ) ) ).Replace( "-", "" );
            }
        }


        internal bool VerifyMd5Hash( string content, string hash )
        {
            string hashOfInput = GetMD5Hash( content );
            StringComparer stringCompare = StringComparer.OrdinalIgnoreCase;
            return stringCompare.Compare( hashOfInput, hash ) == 0 ? true : false;
        }


        internal string GetFileHash(string filePath)
        {
            using ( MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider() )
            using ( FileStream fs = new FileStream( filePath, FileMode.Open, FileAccess.Read, FileShare.Read ) )
            {
                return BitConverter.ToString( md5.ComputeHash( fs ) ).Replace( "-", "" );
            }
        }
    }
}

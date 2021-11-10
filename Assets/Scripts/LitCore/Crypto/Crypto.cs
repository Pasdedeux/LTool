/*======================================
* 项目名称 ：LitFramework.Crypto
* 项目描述 ：
* 类 名 称 ：Crypto
* 类 描 述 ：
* 命名空间 ：LitFramework.Crypto
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2019/4/30 13:57:09
* 更新时间 ：2019/4/30 13:57:09
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2019. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2019/4/30 13:57:09
*修改人： Derek Liu
*版本号： V1.0.0.0
*描述：
*
======================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LitFramework.Crypto
{
    public class Crypto
    {
        public readonly static CryptoMD5 md5 = new CryptoMD5();
        public readonly static CryptoSHA1 sha1 = new CryptoSHA1();
    }
}

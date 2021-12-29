﻿#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：LitFramework.UI.Base
* 项目描述 ：
* 类 名 称 ：IUIManager
* 类 描 述 ：
* 所在的域 ：DEREK-HOMEPC
* 命名空间 ：LitFramework.UI.Base
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2018/8/26 16:24:38
* 更新时间 ：2018/8/26 16:24:38
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2018. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LitFramework.UI.Base
{
    public interface IUIManager
    {
        void ShowFade( Action callBack = null, float time = 0.4f );
        void HideFade( Action callBack = null , float time = 0.4f );
        void Close( string uiName, bool isDestroy = false, bool useAnim = true );
        IBaseUI Show( string uiName , params object[] args);
    }

    public interface IBaseUI { }
    
}
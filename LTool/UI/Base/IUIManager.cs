#region << 版 本 注 释 >>
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
* Copyright @ Liu Hanwen 2018. All rights reserved.
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
        bool UseFading { get; set; }
        void ShowFade( float time, Action callBack = null );
        void HideFade( float time, Action callBack = null );
        void Close( string uiName, bool isDestroy = false );
        IBaseUI Show( string uiName );
    }

    public interface IBaseUI { }
    
}

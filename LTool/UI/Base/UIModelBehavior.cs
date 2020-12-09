#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：LitFramework.UI.Base
* 项目描述 ：
* 类 名 称 ：UIModelBehavior
* 类 描 述 ：
* 所在的域 ：DEREK-HOMEPC
* 命名空间 ：LitFramework.UI.Base
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2019/12/4 17:18:39
* 更新时间 ：2019/12/4 17:18:39
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Liu Hanwen 2019. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LitFramework.UI.Base
{
    /// <summary>
    /// 提供一个表进行动态修改UI状态
    /// </summary>
    public class UIModelBehavior:Singleton<UIModelBehavior>
    {
        private Dictionary<string, UIType> _monoUIBehaviorDict = new Dictionary<string, UIType>();
       
        /// <summary>
        /// 动态修改UICurrentUIType属性
        /// </summary>
        /// <param name="uiName"></param>
        /// <param name="currentUIType"></param>
        public void SetUIBehavior( string uiName, UIType currentUIType )
        {
            if ( !_monoUIBehaviorDict.ContainsKey( uiName ) )
                _monoUIBehaviorDict.Add( uiName, currentUIType );
            else
                _monoUIBehaviorDict[ uiName ] = currentUIType;
        }
        /// <summary>
        /// 移除临时修改得UI CurrentUIType属性
        /// </summary>
        /// <param name="uiName"></param>
        public void RemoveUIBehavior( string uiName )
        {
            if ( _monoUIBehaviorDict.ContainsKey( uiName ) ) _monoUIBehaviorDict.Remove( uiName );
        }
        /// <summary>
        /// 获取UI CurrentUIType属性，如果并未对特定UI设定属性，则为空
        /// </summary>
        /// <param name="uiName"></param>
        /// <returns></returns>
        public UIType GetBehavior( string uiName )
        {
            return _monoUIBehaviorDict.ContainsKey( uiName ) ? _monoUIBehaviorDict[ uiName ] : null;
        }
    }
}

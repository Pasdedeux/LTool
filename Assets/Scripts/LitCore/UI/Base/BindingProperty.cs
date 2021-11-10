/*======================================
* 项目名称 ：LitFramework.UI.Base
* 项目描述 ：
* 类 名 称 ：BindingProperty
* 类 描 述 ：
* 命名空间 ：LitFramework.UI.Base
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2019/7/8 18:24:39
* 更新时间 ：2019/7/8 18:24:39
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2019. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2019/7/8 18:24:39
*修改人： Derek Liu
*版本号： V1.0.0.0
*描述：
*
======================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LitFramework.UI.Base
{
    public class BindingProperty<T>
    {
        public delegate void ValueChangeEventHandler( T oriValue, T newValue );
        public ValueChangeEventHandler OnValueChange;

        private T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                if ( !_value.Equals( value ) ) 
                {
                    T oriValue = _value;
                    _value = value;

                    ChangeValue( oriValue, _value );
                }
            }
        }

        private void ChangeValue( T oriValue, T newValue )
        {
            OnValueChange?.Invoke( oriValue, newValue );
        }

        public override string ToString()
        {
            return Value != null ? Value.ToString() : "Binding Value Null";
        }
    }
}

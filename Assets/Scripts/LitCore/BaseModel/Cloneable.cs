/*======================================
* 项目名称 ：LitFramework.Base
* 项目描述 ：
* 类 名 称 ：Cloneable
* 类 描 述 ：
* 命名空间 ：LitFramework.Base
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2020/10/21 15:41:50
* 更新时间 ：2020/10/21 15:41:50
* 版 本 号 ：v1.0.0.0
*******************************************************************
======================================*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace LitFramework
{
    [Serializable]
    /// <summary>
    /// ICloneable基类，扩展与该接口
    /// </summary>
    public abstract class Cloneable<T> where T : class, ICloneable
    {
        /// <summary>
        /// 基础覆写方法
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        public T DeepClone()
        {
            using ( Stream objectStream = new MemoryStream() )
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize( objectStream, this );
                objectStream.Seek( 0, SeekOrigin.Begin );
                return formatter.Deserialize( objectStream ) as T;
            }
        }

        public T ShallowClone()
        {
            return Clone() as T;
        }
    }
}
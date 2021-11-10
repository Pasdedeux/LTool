/**************************************************************** 
 * 作    者：Derek Liu 
 * CLR 版本：4.0.30319.42000 
 * 创建时间：2018/1/31 15:49:36 
 * 当前版本：1.0.0.1 
 *  
 * 描述说明： 
 * 
 * 修改历史： 
 * 
***************************************************************** 
 * Copyright @ Derek Liu 2018 All rights reserved 
*****************************************************************/

using UnityEngine;

namespace LitFramework
{
    /// <summary>
    /// 组件单例基类
    /// </summary>
    /// <typeparam name="T">继承子类类名</typeparam>
    /// 
    public class SingletonMono<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                //是否已存在单例挂载对象
                if ( _instance == null )
                {
                    _instance = FindObjectOfType( typeof( T ) ) as T;

                    //若不存在则创建一个【隐匿对象】将（继承类）以组件方式挂载
                    if ( _instance == null )
                    {
                        GameObject gObj = new GameObject();

                        gObj.name = "_Singleton(" + typeof( T ).Name + ")";
                        gObj.hideFlags = HideFlags.HideAndDontSave; //可见性，以及不可消除性

                        //这里会先触发继承类Awake()方法
                        _instance = gObj.AddComponent( typeof( T ) ) as T;

                        DontDestroyOnLoad( gObj );
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// 移除
        /// </summary>
        public virtual void DoDestroy()
        {
            if ( _instance != null )
            {
                Destroy( _instance );
                _instance = null;
            }
        }
        
    }
}


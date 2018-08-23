/*======================================
* 项目名称 ：LitFramework
* 项目描述 ：
* 类 名 称 ：LoadingTaskModel
* 类 描 述 ：
*                   
* 命名空间 ：LitFramework.GameFlow
* 机器名称 ：SKY-20170413SEJ 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2018/8/23 20:50:24
* 更新时间 ：2018/8/23 20:50:24
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ DerekLiu 2018. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2018//23 21:50:24
*修改人： LHW
*版本号： V1.0.0.0
*描述：
*
======================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LitFramework.GameFlow
{
    /// <summary>
    /// Loading 加载任务数据类，用于承接Loading进度条的中间加载任务
    /// 执行时机由UI界面驱动
    /// </summary>
    public class LoadingTaskModel : Singleton<LoadingTaskModel>
    {
        private Func<bool> _defaultFuncTrue = null;
        private Dictionary<int, Func<bool>> _taskDic;
        public LoadingTaskModel()
        {
            _defaultFuncTrue = () => { return true; };
            _taskDic = new Dictionary<int, Func<bool>>();
        }

        /// <summary>
        ///  在指定加载进度位置执行代码
        /// </summary>
        /// <param name="framePercent">指定的百分比</param>
        /// <param name="funcCallBack">需要执行的回调函数</param>
        public void AddTask( int framePercent, Func<bool> funcCallBack )
        {
            if ( _taskDic.ContainsKey( framePercent ) )
                _taskDic[ framePercent ] = funcCallBack;
            else
                _taskDic.Add( framePercent, funcCallBack );
        }


        /// <summary>
        /// 尝试查询并取出任务
        /// </summary>
        /// <param name="framePercent"></param>
        /// <returns></returns>
        public Func<bool> TryGetTask( int framePercent )
        {
            _taskDic.TryGetValue( framePercent, out Func<bool> result );
            return result == null ? _defaultFuncTrue : result;
        }


        public override void DoDestroy()
        {
            _taskDic.Clear();
            base.DoDestroy();
        }
    }
}

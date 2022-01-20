/*======================================
* 项目名称 ：Assets.Scripts
* 项目描述 ：
* 类 名 称 ：UITestElement
* 类 描 述 ：
* 命名空间 ：Assets.Scripts
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2022/1/20 11:56:05
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2022. All rights reserved.
*******************************************************************
======================================*/

using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class UITestElement : BaseScrollElement
    {
        private Text _btnText;
        private LayoutElement _le;

        public override void Dispose()
        {
            
        }

        public override void SetElement()
        {
            _le = linkedTrans.GetComponent<LayoutElement>();
            _btnText = UnityHelper.GetTheChildNodeComponetScripts<Text>(linkedTrans, "Text");
        }

        public override void UpdateInfo(MsgArgs args)
        {
            _btnText.text = args.Get<int>(1).ToString();
            _le.preferredHeight = UnityEngine.Random.Range(150, 200);
            LDebug.Log(">>UITestElement " + index + " 获得更新 "+ linkedTrans.name);
        }
    }
}

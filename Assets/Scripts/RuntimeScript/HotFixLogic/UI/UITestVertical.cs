using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitFramework;
using LitFramework.HotFix;
namespace Assets.Scripts.UI
{
	/// <summary>
	///
	/// </summary>
	public partial class UITestVertical : BaseUI
	{
		LoopVerticalScrollRect _vertiacal;
		public override void OnAwake()
		{
			bttn_ReturnBtn.onClick.AddListener(OnClickExit);
		}

        public override void OnStart()
        {
			_vertiacal = UnityHelper.GetTheChildNodeComponetScripts<LoopVerticalScrollRect>(RootTrans, "Loop Vertical Scroll Rect");
			_vertiacal.prefabSource.ExecuteTypeScript = () => { return new UITestElement(); };
		}

        public override void OnShow(params object[] args)
		{
			_vertiacal.totalCount = 50;
			_vertiacal.RefillCells();
		}

		public override void OnClose()
		{
			_vertiacal.ClearCells();
		}

		#region 点击回调事件
		private void OnClickExit()
		{
			UIManager.Instance.Close(AssetsName);
		}
		#endregion
	}
}
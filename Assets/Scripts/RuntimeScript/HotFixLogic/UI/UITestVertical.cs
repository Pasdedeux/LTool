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
		public override void OnAwake()
		{
			bttn_ReturnBtn.onClick.AddListener(OnClickExit);
		}
		public override void OnShow(params object[] args)
		{
			var vertical = UnityHelper.GetTheChildNodeComponetScripts<LoopVerticalScrollRect>(RootTrans, "Loop Vertical Scroll Rect");
			vertical.prefabSource.ExecuteTypeScript = () => { return new UITestElement(); };
			vertical.totalCount = 50;
			vertical.RefillCells();
		}
		public override void OnClose()
		{
			
		}
		#region 点击回调事件
		private void OnClickExit()
		{
			UIManager.Instance.Close(AssetsName);
		}
		#endregion
	}
}
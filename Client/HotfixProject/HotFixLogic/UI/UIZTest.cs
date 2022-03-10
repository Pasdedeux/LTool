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
	public partial class UIZTest : BaseUI
	{
		public override void OnAwake()
		{
			bttn_ReturnBtn.onClick.AddListener(OnClickExit);
		}
		public override void OnShow(params object[] args)
		{
			
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
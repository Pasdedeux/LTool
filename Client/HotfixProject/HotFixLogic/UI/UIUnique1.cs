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
	public partial class UIUnique1 : BaseUI
	{
		public override void OnAwake()
		{
			bttn_ReturnBtn.onClick.AddListener(OnClickExit);
		}
		public override void OnShow(params object[] args)
		{
			LDebug.Log(">>>Unique 1 Show");
		}
		public override void OnClose()
		{
			LDebug.Log(">>>Unique 1 Close", LogColor.yellow);
		}
		#region 点击回调事件
		private void OnClickExit()
		{
			UIManager.Instance.Close(AssetsName);
		}
		#endregion
	}
}
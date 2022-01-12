using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitFramework.HotFix;
namespace Assets.Scripts.UI
{
	/// <summary>
	///
	/// </summary>
	public partial class UIUnique2 : BaseUI
	{
		public override void OnAwake()
		{
			bttn_ReturnBtn.onClick.AddListener(OnClickExit);
		}
		public override void OnEnabled(bool replay)
		{
			base.OnEnabled(replay);
			LDebug.Log(">>>Unique 2OnEnabled", LogColor.orange);
		}

		public override void OnDisabled(bool freeze)
		{
			base.OnDisabled(freeze);
			LDebug.Log(">>>Unique 2 OnDisabled", LogColor.red);
		}
		
		public override void OnShow(params object[] args)
		{
			LDebug.Log(">>>Unique 2 Show");
		}
		public override void OnClose()
		{
			LDebug.Log(">>>Unique 2 Close", LogColor.yellow);
		}
		#region 点击回调事件
		private void OnClickExit()
		{
			UIManager.Instance.Close(AssetsName);
		}
		#endregion
	}
}
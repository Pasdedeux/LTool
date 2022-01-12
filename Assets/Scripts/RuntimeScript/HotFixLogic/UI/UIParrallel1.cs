﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitFramework.HotFix;
namespace Assets.Scripts.UI
{
	/// <summary>
	///
	/// </summary>
	public partial class UIParrallel1 : BaseUI
	{
		public override void OnAwake()
		{
			bttn_ReturnBtn.onClick.AddListener(OnClickExit);
		}

		public override void OnEnabled(bool replay)
		{
			base.OnEnabled(replay);
			LDebug.Log(">>>Parr 1OnEnabled", LogColor.orange);
		}

		public override void OnDisabled(bool freeze)
		{
			base.OnDisabled(freeze);
			LDebug.Log(">>>Parr 1 OnDisabled", LogColor.red);
		}

		public override void OnShow(params object[] args)
		{
			LDebug.Log(">>>Parr 1 Show");
		}
		public override void OnClose()
		{
			LDebug.Log(">>>Parr 1 Close", LogColor.yellow);
		}
		#region 点击回调事件
		private void OnClickExit()
		{
			UIManager.Instance.Close(AssetsName);
		}
		#endregion
	}
}
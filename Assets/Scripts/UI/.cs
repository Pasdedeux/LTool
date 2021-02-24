#region << 版 本 注 释 >>
///*----------------------------------------------------------------
// Author : Derek Liu
// 创建时间:2021/2/24 12:53:51
// 备注：由模板工具自动生成
///----------------------------------------------------------------*/
#endregion
using UnityEngine;
using System;
using LitFramework;
using LitFramework.Mono;
using LitFramework.LitTool;
using System.Collections.Generic;
using DG.Tweening;
namespace Assets.Scripts.UI
{
	public class 
	{
		public Button btnExit;
		private bool _isFreeze;
		private Transform _root;
		private DOTweenAnimation[] _anims;
		
		 public override void OnAwake()
		{
			CurrentUIType.uiNodeType = PopUp;
		}
	}
}

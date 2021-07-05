#region << 版 本 注 释 >>
///*----------------------------------------------------------------
// Author : Derek Liu
// 创建时间:2021/4/25 14:08:28
// 该类由模板工具自动生成
///----------------------------------------------------------------*/
#endregion
using UnityEngine;
using UnityEngine.UI;
using System;
using LitFramework;
using LitFramework.Mono;
using LitFramework.LitTool;
using System.Collections.Generic;
using DG.Tweening;
namespace Assets.Scripts.UI
{
	/// <summary>
	/// 
	/// </summary>
	public class UILoading : BaseUI
	{
		private bool _isFreeze;
		private Transform _root;
		private DOTweenAnimation[] _anims;
		
		public override void OnAwake()
		{
			CurrentUIType.uiNodeType = UINodeTypeEnum.Normal;
			CurrentUIType.uiShowMode = UIShowModeEnum.Stack;
			CurrentUIType.uiTransparent = UITransparentEnum.NoPenetratingMiddle;
			
			Init();
		}
		
		/// <summary>
		/// 用于初始化各类UI信息
		/// </summary>
		private void Init()
		{
			_root = transform;
			
			//TODO 初始化该UI信息
			//..
			
			 _anims = AnimationManager.GetAllAnim( _root );
		}
		
		public override void OnShow()
		{
			_anims.Restart( "10001");
			
		}
		
		public override void OnEnabled( bool freeze )
		{
			//TODO 注册事件
			//..
			
			_isFreeze = freeze;
		}
		
		public override void OnDisabled( bool freeze )
		{
			//TODO 取消注册事件
			//..
			
		}
		
		
		
		
		
		
		//==========点击回调事件==========//
		
		private void OnClickExit()
		{
			_anims.Restart( "10002", () => 
			{
				UIManager.Instance.Close( AssetsName );
				//..
			});
		}
	}
}

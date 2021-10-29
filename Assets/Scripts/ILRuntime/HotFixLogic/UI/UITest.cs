#region << 版 本 注 释 >>
///*----------------------------------------------------------------
// Author : Derek Liu
// 创建时间:2021/10/29 12:01:14
// 该类由模板工具自动生成
///----------------------------------------------------------------*/
#endregion
using UnityEngine;
using UnityEngine.UI;
using System;
using LitFramework;
using LitFramework.HotFix;
using LitFramework.LitTool;
using System.Collections.Generic;
using DG.Tweening;
namespace Assets.Scripts.UI
{
	/// <summary>
	/// 主测试界面
	/// </summary>
	public class UITest : BaseUI
	{
		private bool _isFreeze;
		private Button _btnExit;
		private Transform _root;
		private DOTweenAnimation[] _anims;
		private LoopHorizontalScrollRect _loopVertical;
		
		public static int RegistSystem( string className )
		{
			UIManager.Instance.RegistFunctionCallFun( ResPath.UI.UITEST, className );
			return 1;
		}
		
		public override void OnAwake()
		{
			CurrentUIType.uiNodeType = UINodeTypeEnum.PopUp;
			CurrentUIType.uiShowMode = UIShowModeEnum.Stack;
			CurrentUIType.uiTransparent = UITransparentEnum.NoPenetratingMiddle;
			
			Init();
		}
		
		/// <summary>
		/// 用于初始化各类UI信息
		/// </summary>
		private void Init()
		{
			_root = this.GameObjectInstance.transform;
			_btnExit = UnityHelper.GetTheChildNodeComponetScripts<Button>( _root, "Btn_Exit" );
			_loopVertical = UnityHelper.GetTheChildNodeComponetScripts<LoopHorizontalScrollRect>(_root, "GameObject");
			_loopVertical.totalCount = 100;
			_loopVertical.RefillCells();

			 //TODO 初始化该UI信息
			 //..

			 _anims = AnimationManager.GetAllAnim( _root );
		}
		
		public override void OnShow()
		{
			_anims.Restart( "10001");
			LDebug.Log(">>>Animation");
		}
		
		public override void OnEnabled( bool freeze )
		{
			_btnExit.onClick.AddListener( OnClickExit );
			//TODO 注册事件
			//..
			
			_isFreeze = freeze;
		}
		
		public override void OnDisabled( bool freeze )
		{
			_btnExit.onClick.RemoveAllListeners();
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

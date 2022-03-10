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
	public partial class UIGerwerwe : BaseUI
	{
		private void SetUIType()
		{
			CurrentUIType.uiNodeType = UINodeTypeEnum.PopUp;
			CurrentUIType.uiShowMode = UIShowModeEnum.Stack;
			CurrentUIType.uiTransparent = UITransparentEnum.NoPenetratingMiddle;
			Flag = UIFlag.Normal;
			UseLowFrame = false;
		}
		public static int RegistSystem(string className)
		{
			UIManager.Instance.RegistFunctionCallFun(ResPath.UI.UIGERWERWE, className);
			return 1;
		}
		private Transform trans_ReturnBtn;
		private Button bttn_ReturnBtn;
		public override void FindMember()
		{
			SetUIType();
			trans_ReturnBtn = RootAniTrans.Find("_ReturnBtn");
			bttn_ReturnBtn = trans_ReturnBtn.GetComponent<Button>();
		}
	}
}
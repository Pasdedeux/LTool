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
	public partial class UIUnique3 : BaseUI
	{
		private void SetUIType()
		{
			CurrentUIType.uiNodeType = UINodeTypeEnum.PopUp;
			CurrentUIType.uiShowMode = UIShowModeEnum.Unique;
			CurrentUIType.uiTransparent = UITransparentEnum.NoPenetratingMiddle;
			Flag = UIFlag.Fix;
			UseLowFrame = true;
		}
		public static int RegistSystem(string className)
		{
			UIManager.Instance.RegistFunctionCallFun(ResPath.UI.UIUNIQUE3, className);
			return 1;
		}
		private Transform trans_ReturnBtn;
		private Button bttn_ReturnBtn;
		protected override void FindMember()
		{
			SetUIType();
			trans_ReturnBtn = m_AniTrans.Find("_ReturnBtn");
			bttn_ReturnBtn = trans_ReturnBtn.GetComponent<Button>();
		}
	}
}
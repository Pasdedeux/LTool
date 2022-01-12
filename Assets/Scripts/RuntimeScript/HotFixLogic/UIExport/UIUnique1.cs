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
	public partial class UIUnique1 : BaseUI
	{
		private void SetUIType()
		{
			CurrentUIType.uiNodeType = UINodeTypeEnum.PopUp;
			CurrentUIType.uiShowMode = UIShowModeEnum.Unique;
			CurrentUIType.uiTransparent = UITransparentEnum.NoPenetratingMiddle;
			Flag = UIFlag.Fix;
			UseLowFrame = true;
		}
		public override void OnEnabled(bool replay)
		{
			base.OnEnabled(replay);
			LDebug.Log(">>>Unique 1OnEnabled", LogColor.orange);
		}

		public override void OnDisabled(bool freeze)
		{
			base.OnDisabled(freeze);
			LDebug.Log(">>>Unique 1 OnDisabled", LogColor.red);
		}

		public static int RegistSystem(string className)
		{
			UIManager.Instance.RegistFunctionCallFun(ResPath.UI.UIUNIQUE1, className);
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
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor; 
using UnityEngine;
/// 代码自动创建、更新
/// 更新时间:2022/2/21   17:31:57
/// </summary>
public class ConfigToolWindow : OdinEditorWindow
{
	[MenuItem("本地数据 / 本地配置数据")]
	public static void SetCommon()
	{
		CommonData LocalData = LocalDataHandle.LoadConfig<CommonData>();
		var window = OdinEditorWindow.InspectObject(LocalData);
		window.position = GUIHelper.GetEditorWindowRect().AlignCenter(600, 800);
		window.OnClose +=()=>{ LocalDataHandle.SaveConfig(LocalData);};
	}
}
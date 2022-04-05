using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor; 
using UnityEngine;
using LitFramework.Persistent;
/// 代码自动创建、更新
/// 更新时间:2022/4/5   12:29:50
/// </summary>
public class LocalDataToolWindow : OdinEditorWindow
{
	[MenuItem("本地数据 / 修改本地数据")]
	 public static LocalDataToolWindow OpenWindow()
	{
		 LocalDataToolWindow window = OdinEditorWindow.GetWindow<LocalDataToolWindow>();
		window.position = GUIHelper.GetEditorWindowRect().AlignCenter(600, 800);
		 return window;
	}
	[Button("FuncRecordLocalData", buttonSize: ButtonSizes.Large), GUIColor(0.1f, 0.4f,0.8f)]
	public void SetFuncRecordLocalData()
	{
		FuncRecordLocalData LocalData = LocalDataHandle.LoadData<FuncRecordLocalData>();
		var window = OdinEditorWindow.InspectObject(LocalData);
		window.OnClose += LocalData.SaveFlag;
		window.OnClose += LocalData.SaveImmit;
	}
	[Button("AccountLocalData", buttonSize: ButtonSizes.Large),GUIColor(0f, 0.8f,0.6f)]
	public void SetAccountLocalData()
	{
		AccountLocalData LocalData = LocalDataHandle.LoadData<AccountLocalData>();
		var window = OdinEditorWindow.InspectObject(LocalData);
		window.OnClose += LocalData.SaveFlag;
		window.OnClose += LocalData.SaveImmit;
	}
}
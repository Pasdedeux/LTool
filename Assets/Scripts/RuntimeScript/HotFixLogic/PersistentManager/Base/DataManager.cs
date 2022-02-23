﻿using LitFramework;
using LitFramework.Base;
using LitFramework.LitTool;
/// 代码自动创建、更新
/// 更新时间:2022/2/21   17:31:57
/// </summary>
public class DataManager : Singleton<DataManager>,IManager
{
	public System.Action DataInstallEnd;
	public void Install()
	{
		LoadData();
		SetPlayerData();
		CheckFristLogin();
		InstallManagers();
		GameDriver.Instance.UpdateEventHandler += SaveData;
		DataInstallEnd?.Invoke();
	}
	public AccountLocalData AccountLocal;
	public FuncRecordLocalData FuncRecordLocal;
	private void LoadData()
	{
		AccountLocal = LocalDataHandle.LoadData<AccountLocalData>();
		FuncRecordLocal = LocalDataHandle.LoadData<FuncRecordLocalData>();
		
		CommonData.Instance= LocalDataHandle.LoadConfig<CommonData>();
		
	}
	private void SetPlayerData()
	{
		AccountManager.Instance.LocalData = AccountLocal;
		FuncRecordManager.Instance.LocalData = FuncRecordLocal;
	}
	private void InstallManagers()
	{
		AccountManager.Instance.Install();
		FuncRecordManager.Instance.Install();
	}
	private void CheckFristLogin()
	{
		if (0L !=AccountLocal.CreateAccountTime)
		{
			return;
		}
		AccountManager.Instance.FirstIniteData();
		FuncRecordManager.Instance.FirstIniteData();
		AccountLocal.CreateAccountTime = LitTool.GetTimeStamp();
		SaveAllFlag();
		SaveData();
	}
	public void SaveAllFlag()
	{
		AccountLocal.SaveFlag();
		FuncRecordLocal.SaveFlag();
	}
	public void SaveData()
	{
		AccountLocal.SavaImmit();
		FuncRecordLocal.SavaImmit();
	}
	public System.Action DestroyPayerData;
	public void Uninstall()
	{
		DestroyPayerData?.Invoke();
		DestroyPayerData = null;
		SaveAllFlag();
		SaveData();
		AccountLocal = null;
		FuncRecordLocal= null;
	}
}
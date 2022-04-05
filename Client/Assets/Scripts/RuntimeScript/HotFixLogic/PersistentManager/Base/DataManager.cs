using LitFramework;
using LitFramework.Base;
using LitFramework.LitTool;
using LitFramework.Persistent;
using ILRBaseModel.Singleton;
/// 代码自动创建、更新
/// 更新时间:2022/4/5   12:29:50
/// </summary>
public class DataManager : Singleton<DataManager>,IManager
{
	public System.Action DataInstallEnd;
	public void Install()
	{
		LoadData();
		CheckFristLogin();
		InstallManagers();
		GameDriver.Instance.UpdateEventHandler += SaveData;
		DataInstallEnd?.Invoke();
	}
	private ILocalDataManager[] m_LocalDataManagerArry = new ILocalDataManager[2];
	private void LoadData()
	{
		
		CommonData.Instance= LocalDataHandle.LoadConfig<CommonData>();
		
		m_LocalDataManagerArry[0] = AccountManager.Instance;
		m_LocalDataManagerArry[1] = FuncRecordManager.Instance;
		
		
		foreach (ILocalDataManager manager in m_LocalDataManagerArry)
		{
			manager.LoadLocalData();
		}
	}
	private void InstallManagers()
	{
		foreach (ILocalDataManager manager in m_LocalDataManagerArry)
		{
			manager.Install();
		}
	}
	private void CheckFristLogin()
	{
		if (0L !=AccountManager.Instance.LocalData.CreateAccountTime)
		{
			return;
		}
		foreach (ILocalDataManager manager in m_LocalDataManagerArry)
		{
			manager.FirstIniteData();
		}
		AccountManager.Instance.LocalData.CreateAccountTime = LitTool.GetTimeStamp();
		SaveAllFlag();
		SaveData();
	}
	public void SaveAllFlag()
	{
		foreach (ILocalDataManager manager in m_LocalDataManagerArry)
		{
			manager.LocalData.SaveFlag();
		}
	}
	public void SaveData()
	{
		foreach (ILocalDataManager manager in m_LocalDataManagerArry)
		{
			manager.LocalData.SaveImmit();
		}
	}
	public System.Action DestroyPayerData;
	public void Uninstall()
	{
		DestroyPayerData?.Invoke();
		DestroyPayerData = null;
		SaveAllFlag();
		SaveData();
		foreach (ILocalDataManager manager in m_LocalDataManagerArry)
		{
			manager.Uninstall();
		}
	}
}
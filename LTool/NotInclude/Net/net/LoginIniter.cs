using UnityEngine;
using System.Runtime.InteropServices;
using common.net.cookie;

public class LoginIniter
{
    AbstractLogReport logReport = LoggerFactory.getInst().getLogger();
    private static LoginIniter inst = new LoginIniter();
    public LoginIniter()
    {
    }
    public static LoginIniter getInst()
    {
        return inst;
    }

    [DllImport("__Internal")]
    private static extern string _GetUUID();
    AppCfg appCfg;
    public AppCfg AppCfg
    {
        set { appCfg = value; }
    }
    private int sdkCode;
    public int SdkCode { get { return sdkCode; } }
    private string openid;
    public string Openid { get { return openid; } }
    private string ext1;
    public string Ext1 { get { return ext1; } }
    public void Init()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            sdkCode = SDKCode.DEVICE_IOS;
            //1、keychain中查找imei.2、找到返回imei.3、没找到生成imei，存入keychain，返回imei
            openid = _GetUUID();
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            sdkCode = SDKCode.DEVICE_ANDROID;
            openid = SystemInfo.deviceUniqueIdentifier;
        }
        else if(Application.platform == RuntimePlatform.WindowsEditor||Application.platform==RuntimePlatform.WindowsPlayer)
        {
            sdkCode = SDKCode.DEVICE_PC;
            openid = (appCfg.TestPlayerIMEI==null|| "".Equals(appCfg.TestPlayerIMEI))? SystemInfo.deviceUniqueIdentifier: appCfg.TestPlayerIMEI;
            Cookie cookie = CookieData.GetInstance().Load();
            if (cookie != null)
            {
                if (cookie.SdkCode == SDKCode.DEVICE_PC)
                {
                    string IMEINew = appCfg.TestPlayerIMEI == null ? "" : appCfg.TestPlayerIMEI;
                    string IMEIOld = PlayerPrefs.GetString(CodeMap.Filed.Filed_IMEI.ToString(), "") == null ? "" : PlayerPrefs.GetString(CodeMap.Filed.Filed_IMEI.ToString(), "");
                    if (!IMEINew.Equals(IMEIOld))
                    {
                        CookieData.GetInstance().Clear();
                        PlayerPrefs.SetString(CodeMap.Filed.Filed_IMEI.ToString(), IMEINew);
                        PlayerPrefs.Save();
                    }
                }
            }
        }
    }
    public void OnSdkLoginReturn(int sdkCode,string openid,string ext1)
    {
        this.sdkCode = sdkCode;
        this.openid = openid;
        this.ext1 = ext1;
        Cookie cookie = CookieData.GetInstance().Load();
        if(cookie!=null)
        {
            if (cookie.SdkCode != sdkCode || cookie.Openid != openid)
                CookieData.GetInstance().Clear();
        }
    }
}
using common.net.socket;
using System.Collections.Generic;
using common.net.http;
using common.core;
using common.net.socket.codec;
using System;
using common.core.codec;
using common.net.socket.session;
using common.net.socket.acceptor;
using common.net.cookie;
public delegate void OnClosed();
public delegate void BeginReconnect();
public delegate void ConnectSuccess();
public delegate void OnTimeOut(Msg msg);
public class ConnectionProcessorProxy:ConnectedProcessor
{
    private ConnectedProcessor connectedProcessor;
    private int clientCode;
    private Client client;
    private Queue<KeyValuePair<int, Client>> removeClients;
    public ConnectionProcessorProxy(ConnectedProcessor connectedProcessor, int clientCode,Client client, Queue<KeyValuePair<int, Client>> removeClients) 
    {
        this.connectedProcessor = connectedProcessor;
        this.clientCode = clientCode;
        this.client = client;
        this.removeClients = removeClients;
    }
    public void OnConnected(Boolean isConnected,Client client) 
    {
        connectedProcessor.OnConnected(isConnected,client);
        if (!isConnected)
            removeClients.Enqueue(new KeyValuePair<int, Client>(this.clientCode, this.client));
    }
    public void OnOffline()
    {
        connectedProcessor.OnOffline();
    }
}
public class GameSrvConnectionProcessor : ConnectedProcessor
{
    private AbstractLogReport logReport = LoggerFactory.getInst().getUnityLogger();
    private Client client;
    private BeginReconnect beginReconnect;
    private OnClosed onClosed;
    public GameSrvConnectionProcessor(Client client,BeginReconnect beginReconnect,OnClosed onClosed)
    {
        this.client = client;
        this.beginReconnect = beginReconnect;
        this.onClosed = onClosed;
    }
    public void OnConnected(bool isConnected, Client client)
    {
        if (!isConnected)
        {
            logReport.OnLogReport("GameSrv connect fail");
            onClosed();
        }
        else
            logReport.OnLogReport("GameSrv connect success");

    }
    public void OnOffline()
    {
        if(client.IsKickOff)
        {
            logReport.OnLogReport("GameSrv connect offline is kickOff not reConnect.");
            client.RestKickOffFlag();
            return;
        }
        logReport.OnLogReport("GameSrv connect offline and reConnect.");
        beginReconnect();
        client.DoConnect();
    }
}
public class BindReturnWraper : CmdCallbackWraper
{
    public BindReturnWraper(CmdCallback cmdCallback):base(cmdCallback)
    {
        CmdCallback = cmdCallback;
    }
    public override void DoInWraper(Msg msg)
    {
        int rsCode = (int)msg.GetParam(AccountSrvCodeMap.Param.RS_CODE);
        if(rsCode == AccountSrvCodeMap.RsCode.ERRO_ACCOUNT_OP_BIND_SUCCESS||rsCode == AccountSrvCodeMap.RsCode.SUCCESS)
        {
            Cookie cookie = CookieData.GetInstance().Load();
            cookie.IsBind = true;
            CookieData.GetInstance().Save(cookie);
        }
        CmdCallback(msg);
    }

    public override bool IsCallback()
    {
        return true;
    }
}
public class NetService{
    private AbstractLogReport logReport = LoggerFactory.getInst().getLogger();
    private const int DEFAULT_TEST_LOGIN_TIME = 3;
    private static int testLoginTimes = DEFAULT_TEST_LOGIN_TIME;
    /// <summary>
    /// 验证码类型_注册
    /// </summary>
    public const int VERIFY_CODE_TYPE_REGIST = 1;
    /// <summary>
    /// 验证码类型_密码找回
    /// </summary>
    public const int VERIFY_CODE_TYPE_FIND_PWD = 2;
    private NetService() { }
    private ISocketDecoder decoder;
    private ISocketEncoder encoder;
    private IDecoder httpDecoder;
    private IEncoder httpEncoder;
    private Msg heartBeatPackage;
    private string uid;
    private string token;
    private int srvid;
    private string srvIp;
    private int srvPort;
    private long tokenTimestamp;
    private bool isBind;
    private string playerid;
    public string Playerid{get { return playerid; }}
    private string openid;
    private int sdkCode;
    private AppCfg appCfg;
    public AppCfg AppCfg{set { appCfg = value; }}
    Queue<KeyValuePair<int, Client>> addClients = new Queue<KeyValuePair<int, Client>>();
    Queue<KeyValuePair<int, Client>> removeClients = new Queue<KeyValuePair<int, Client>>();
	public delegate void ConnectInternetResultHandler(bool rs,Msg msg);
    private ConnectInternetResultHandler ConnectSocketComplete;
    private ConnectInternetResultHandler accountSrvComplete;
    private OnMsgPush onMsgPush;
    private System.Collections.Generic.Dictionary<int, Client> otherClients = new Dictionary<int, Client>();
    private static Client gameSrvClient;
    public BeginReconnect GameSrvBeginReconnect { get; set; }
    public ConnectSuccess GameSrvConnectSuccess { get; set; }
    public Client GameSrvClient { get { return gameSrvClient; } }
    private Acceptor acceptor;
    private Dictionary<string, HttpClient> httpClients = new Dictionary<string, HttpClient>();
    private bool isLogined = false;
    public bool IsLogined { get { return isLogined; } }
    public OnMsgPush OnMsgPush
	{
		set{ onMsgPush = value;gameSrvClient.OnMsgPush = onMsgPush;}
	}
	private OnClosed onClosed;
	public OnClosed OnClosed
	{
		set{ onClosed = value;}
	}
    private OnTimeOut onTimeOut;
    public OnTimeOut OnTimeOut
    {
        set { onTimeOut = value; }
    }
    private OnTimeOut onAccountSrvTimeOut;
    public OnTimeOut OnAccountLoginTimeOut
    {
        set { onAccountSrvTimeOut = value; }
    }
    public static bool IsBind()
    {
        Cookie cookie = CookieData.GetInstance().Load();
        if (cookie == null)
            return false;
        int sdkCode = cookie.SdkCode;
        if (sdkCode != SDKCode.DEVICE_ANDROID && sdkCode != SDKCode.DEVICE_IOS && sdkCode != SDKCode.DEVICE_PC)
            return true;
        return cookie.IsBind;
    }
    public static void SetBindFlagIsTrue()
    {
        Cookie cookie = CookieData.GetInstance().Load();
        cookie.IsBind = true;
        CookieData.GetInstance().Save(cookie);
    }
    private static NetService inst = new NetService();
    public static NetService getInstance()
    {
        return inst;
    }
    public void Start()
    {
        acceptor = new Acceptor();
        acceptor.Start();
        decoder = new DefaultSocketDecoder();
		encoder = new DefaultSocketEncoder();
        httpDecoder = new DefaultDecoder();
        httpEncoder = new DefaultEncoder();
        heartBeatPackage = new Msg(BaseCodeMap.BaseCmd.CMD_HEART_BEAT);
        SessionConfig sessionConfig = new SessionConfig(60, 1000000);
        gameSrvClient = getClient(null, onMsgPush, sessionConfig);
        gameSrvClient.ProcTimeOut = GameSrvProcTimeOut;
    }
    private void GameSrvProcTimeOut(Msg msg)
    {
        Close();
        if (onTimeOut!=null)
            onTimeOut(msg);
    }
    private void AccountSrvTimeOut(Msg msg)
    {
        if (onAccountSrvTimeOut != null)
            onAccountSrvTimeOut(msg);
    }
    public void OnDestroy()
    {
        acceptor.Stop();
    }
    public void Pause()
    {
        if(acceptor != null)
            acceptor.Pause();
    }
    public void Active()
    {
        if (acceptor != null)
            acceptor.Active();
    }
	public void Update () {
        DateTime now = DateTime.Now;
        if (gameSrvClient == null)
            return;
        gameSrvClient.Update(now);
        while(addClients.Count!=0)
        {
            KeyValuePair<int,Client> o = addClients.Dequeue();
            int key = o.Key;
            Client v = o.Value;
            otherClients[key] = v;
        }
        while(removeClients.Count!=0) 
        {
            KeyValuePair<int, Client> o = removeClients.Dequeue();
            int key = o.Key;
            Client v = o.Value;
            if (otherClients.ContainsKey(key))
                otherClients.Remove(key);
        }
        foreach (var v in otherClients)
        {
            Client client = v.Value;
            client.Update(now);
        }
		foreach (var v in httpClients) 
		{
			HttpClient httpClient = v.Value;
			httpClient.Update();
		}
    }
    public void LogOut()
    { 
        Close();
    }
    public void LogOutAndClearCookie()
    {
        CookieData.GetInstance().Clear();
        Close();
    }
    internal void Close()
    {
        isLogined = false;
        gameSrvClient.Close();
        foreach (var v in otherClients)
        {
            Client client = v.Value;
            client.Close();
        }
    }
	public void close(int clientCode)
	{
        if (!otherClients.ContainsKey(clientCode))
            return;
        Client client = otherClients [clientCode];
		client.Close ();
        removeClients.Enqueue(new KeyValuePair<int, Client>(clientCode,client));
    }
    public Client getClient(int clientCode)
    {
        if (otherClients.ContainsKey(clientCode))
            return otherClients[clientCode];
        else
            return null;
    }
	public Client getClient(Msg loginPackage,OnMsgPush onMsgPush,SessionConfig sessionCfg)
	{
        Client client = ClientFactory.getInstance ().get (sessionCfg,encoder,decoder, heartBeatPackage);
		client.OnMsgPush = onMsgPush;
        client.OnLoginReturn = OnSocketLoginReturn;
        client.Acceptor = acceptor;
		return client;
	}
    public void Connect(int clientCode, String ip, int port, Client client,ConnectedProcessor connectedProcessor) 
    {
        if (otherClients.ContainsKey(clientCode))
        {
            Client oldClient = otherClients[clientCode];
            oldClient.Close();
        }
        AddClient(clientCode, client);
        client.Connect(ip, port, new ConnectionProcessorProxy(connectedProcessor, clientCode, client, this.removeClients));
    }
    public void BgConnect(string ip, int port, Client client, ConnectedProcessor connectedProcessor)
    {
        client.Connect(ip, port, connectedProcessor);
    }
    public void AddClient(int clientCode, Client client)
    {
        addClients.Enqueue(new KeyValuePair<int,Client>(clientCode,client));
    }
    public bool Check()
    {
        return gameSrvClient.IsOpen();
    }
    public bool OtherClientCheck(int clientKey)
    {
        Client client = otherClients[clientKey];
        return client.IsOpen();
    }
    public bool sendMessage(Msg msg, CmdCallback callback)
    {
		bool rs = gameSrvClient.Send(msg, callback);
        return rs;
    }
    public bool sendMessage(Msg msg) 
    {
        bool rs = sendMessage(msg, null);
        return rs;
    }
    public bool sendMessage(int clientKey,Msg msg, CmdCallback callback)
    {
        if (!otherClients.ContainsKey(clientKey))
        {
            logReport.OnWarningReport("send:" + msg.ToString() + " fail,invalide clientKey:" + clientKey);
            return false;
        }
        bool rs = otherClients[clientKey].Send(msg, callback);
        logReport.OnLogReport("send:" + msg.ToString() + ",time:" + gameSrvClient.Runtime);
        return rs;
    }
    public bool sendMessage(int clientKey,Msg msg)
    {
        bool rs = sendMessage(clientKey, msg,null);
        return rs;
    }
    public void login(ConnectInternetResultHandler SocketComplete, ConnectInternetResultHandler accountSrvComplete)
    {
        ConnectSocketComplete = SocketComplete;
        this.accountSrvComplete = accountSrvComplete;
        Msg msg = new Msg(AccountSrvCodeMap.Cmd.CMD_ACCOUNT_LOGIN);
        testLoginTimes = DEFAULT_TEST_LOGIN_TIME;
        LoginWithCookie(msg);
    }
	public void accountAuthLogin(String userName,String pwd,ConnectInternetResultHandler SocketComplete, ConnectInternetResultHandler accountSrvComplete)
	{
		CookieData.GetInstance().Clear();
        LoginIniter.getInst().OnSdkLoginReturn(SDKCode.ACCOUNT_AUTH, userName, pwd);
		login (SocketComplete, accountSrvComplete);
	}
    public void accountAuthLogin(int sdkCode,String userName, String pwd, ConnectInternetResultHandler SocketComplete, ConnectInternetResultHandler accountSrvComplete)
    {
        CookieData.GetInstance().Clear();
        LoginIniter.getInst().OnSdkLoginReturn(sdkCode, userName, pwd);
        login(SocketComplete, accountSrvComplete);
    }
    public void VistorLogin(ConnectInternetResultHandler SocketComplete, ConnectInternetResultHandler accountSrvComplete)
    {
        CookieData.GetInstance().Clear();
        LoginIniter.getInst().Init();
        login(SocketComplete, accountSrvComplete);
    }
    public void SDKLogin(int sdkCode,string openid,string ext1,ConnectInternetResultHandler SocketComplete, ConnectInternetResultHandler accountSrvComplete)
    {
        LoginIniter.getInst().OnSdkLoginReturn(sdkCode, openid, ext1);
        login(SocketComplete, accountSrvComplete);
    }
    public void registAuthAccount(String userName,String pwd,CmdCallback onRegistReturn)
	{
		Msg msg = new Msg (AccountSrvCodeMap.Cmd.CMD_ACCOUNT_REGIST);
		msg.AddParam (AccountSrvCodeMap.Param.ACCOUNT_OPEN_ID,userName);
		msg.AddParam (AccountSrvCodeMap.Param.ACCOUNT_PWD, pwd);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_SDK_CODE, SDKCode.ACCOUNT_AUTH);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_SRV_VERSION, appCfg.PkgVersion);
        sendHttpMessage( appCfg.LoginUrl , msg , onRegistReturn, AccountSrvTimeOut);
	}
    public void RegistAuthAccount(string userName, string pwd,int sdkCode,string verificationCode, CmdCallback onRegistReturn)
    {
        Msg msg = new Msg(AccountSrvCodeMap.Cmd.CMD_ACCOUNT_REGIST);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_OPEN_ID, userName);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_PWD, pwd);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_SDK_CODE,sdkCode);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_VERIFICATION_CODE, verificationCode);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_SRV_VERSION, appCfg.PkgVersion);
        sendHttpMessage(appCfg.LoginUrl, msg, onRegistReturn,AccountSrvTimeOut);
    }
    public void BindAuthAccount(int sdkCode,String openId,string verifyCode,string pwd,CmdCallback onBindReturn)
    {
        Msg msg = new Msg(AccountSrvCodeMap.Cmd.CMD_ACCOUNT_RELACCOUNT_BIND);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_SDK_CODE, sdkCode);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_OPEN_ID, openId);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_UID, uid);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_VERIFICATION_CODE, verifyCode);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_PWD, pwd);
        sendHttpMessage2(appCfg.LoginUrl, msg, new BindReturnWraper(onBindReturn),AccountSrvTimeOut);
    }
    private void LoginWithCookie(Msg loginMsg)
    {
        Cookie cookie = CookieData.GetInstance().Load();
        if (cookie == null)
        {
            logReport.OnDebugReport("cookie is null");
			openid = LoginIniter.getInst().Openid;
            sdkCode = LoginIniter.getInst().SdkCode;
            string pwd = LoginIniter.getInst().Ext1;
			if ( openid == null||"".Equals( openid )|| 0 == sdkCode )
			{
				logReport.OnWarningReport("openid and sdkid error!");
				return;
			}
			loginMsg.AddParam( AccountSrvCodeMap.Param.ACCOUNT_OPEN_ID, openid );//设备号
			loginMsg.AddParam( AccountSrvCodeMap.Param.ACCOUNT_SDK_CODE, sdkCode );
			if (!"".Equals (pwd))
				loginMsg.AddParam (AccountSrvCodeMap.Param.ACCOUNT_PWD, pwd);
            loginMsg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_SRV_VERSION, appCfg.PkgVersion);
            sendHttpMessage (appCfg.LoginUrl, loginMsg, OnhttpLoginReturn,AccountSrvTimeOut);
        }
        else
        {
            uid = cookie.Uid;
			token = cookie.Token;
			srvid = cookie.Srvid;
            tokenTimestamp = cookie.TokenTimestamp;
            isBind = cookie.IsBind;
            openid = cookie.Openid;
            sdkCode = cookie.SdkCode;
            string loginfo = "save cookie info(accountid:" + this.uid + ","
				+ "accountToken:" + token + ","
				+ "srvid:" + srvid + ","
				+ "tokenTimestamp:" + tokenTimestamp + ")";
            logReport.OnLogReport( loginfo );
			Msg msg = new Msg( AccountSrvCodeMap.Cmd.CMD_ACCOUNT_GET_SERVER_INFO);
            msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_SRV_VERSION, appCfg.Version);
            msg.AddParam( AccountSrvCodeMap.Param.ACCOUNT_SRV_ID , srvid);
            msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_SRV_VERSION, appCfg.PkgVersion);
            sendHttpMessage( appCfg.LoginUrl , msg , srvInfoReturn, AccountSrvTimeOut);
        }
    }
	public void srvInfoReturn( Msg msg )
	{
		int rscode = ( int ) msg.GetParam( AccountSrvCodeMap.Param.RS_CODE );
		if ( rscode == AccountSrvCodeMap.RsCode.SUCCESS )
		{
			srvIp = ( string ) msg.GetParam( AccountSrvCodeMap.Param.ACCOUNT_SRV_IP );
			srvPort = ( int ) msg.GetParam( AccountSrvCodeMap.Param.ACCOUNT_SRV_PORT );
            Open();
		}
		else
			OnSocketLoginReturn( msg );
	}
    public void ReqVerificationCode(string mobilePhone,int verifyType,CmdCallback callback)
    {
        Msg msg = new Msg(AccountSrvCodeMap.Cmd.CMD_ACCOUNT_REQ_VERIFICATION_CODE);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_OPEN_ID, mobilePhone);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_VERIFY_CODE_TYPE, verifyType);
        sendHttpMessage(appCfg.LoginUrl, msg, callback);
    }
    public void VerifiyCode(string mobilePhone, string verificationCode, CmdCallback callback)
    {
        Msg msg = new Msg(AccountSrvCodeMap.Cmd.CMD_ACCOUNT_VERIFY_CODE);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_OPEN_ID, mobilePhone);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_VERIFICATION_CODE, verificationCode);
        sendHttpMessage(appCfg.LoginUrl, msg, callback);
    }
    public void UpdatePwd(string mobilePhone, string verificationCode,string pwd, CmdCallback callback)
    {
        Msg msg = new Msg(AccountSrvCodeMap.Cmd.CMD_ACCOUNT_UPDATE_PWD);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_OPEN_ID, mobilePhone);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_VERIFICATION_CODE, verificationCode);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_PWD, pwd);
        sendHttpMessage(appCfg.LoginUrl, msg, callback);
    }
    public void sendHttpMessage(string url, Msg msg, CmdCallback callback)
    {
        sendHttpMessage(url, msg, callback, null);
    }
    public void sendHttpMessage2(string url, Msg msg, Object callback, common.net.http.OnTimeOut onTimeOut)
    {
        HttpClient httpClient = null;
        if (httpClients.ContainsKey(url))
            httpClient = httpClients[url];
        if (httpClient == null)
        {
            httpClient = new HttpClient(url);
            httpClient.Decoder = httpDecoder;
            httpClient.Encoder = httpEncoder;
            httpClients.Add(url, httpClient);
        }
        if (onTimeOut != null)
            httpClient.TimeOutProc = onTimeOut;
        logReport.OnLogReport("sendHttp msg:" + msg.ToString() + ",url:" + url);
        httpClient.Send(msg, callback);
    }
    public void sendHttpMessage(string url, Msg msg, CmdCallback callback, common.net.http.OnTimeOut onTimeOut)
    {
        sendHttpMessage2(url, msg, callback, onTimeOut);
    }
    public void OnhttpLoginReturn(Msg msg)
    {
        int rsCode = (int)msg.GetParam(BaseCodeMap.BaseParam.RS_CODE);
        switch (rsCode)
        {
            case BaseCodeMap.BaseRsCode.SUCCESS:
				srvIp = ( string ) msg.GetParam( AccountSrvCodeMap.Param.ACCOUNT_SRV_IP );
			    srvPort = ( int ) msg.GetParam( AccountSrvCodeMap.Param.ACCOUNT_SRV_PORT );
				logReport.OnLogReport("loginHttpReqSucess->ip:"+srvIp+",port:"+srvPort);
                if(accountSrvComplete != null)
                    accountSrvComplete(true, msg);
                OpenSoketConnect(msg);
                break;
            default:
                logReport.OnWarningReport("loginHttpReqFailAndReconnect->rscode:" + rsCode);
                if (accountSrvComplete != null)
                    accountSrvComplete(false, msg);
                OnSocketLoginReturn(msg);
                break;
        }
    }
    public void OpenSoketConnect(Msg msg)//openSoket
    {
		uid = (string)msg.GetParam(AccountSrvCodeMap.Param.ACCOUNT_UID);
        token = (string)msg.GetParam(AccountSrvCodeMap.Param.ACCOUNT_TOKEN);
        srvid = (int)msg.GetParam(AccountSrvCodeMap.Param.ACCOUNT_SRV_ID);
        tokenTimestamp = (long)msg.GetParam(AccountSrvCodeMap.Param.ACCOUNT_TOKEN_TIMESTAMP);
        isBind = (bool)msg.GetParam (AccountSrvCodeMap.Param.ACCOUNT_IS_BIND);
        Open();
    }
    public void Open()
    {
        Msg loginPackage = new Msg(GameSrvCodeMap.Cmd.CMD_USER_LOG_IN);
        loginPackage.AddParam(GameSrvCodeMap.Param.UID, uid);
        loginPackage.AddParam(GameSrvCodeMap.Param.TOKEN, token);
        loginPackage.AddParam(GameSrvCodeMap.Param.SRVID, srvid);
        loginPackage.AddParam(GameSrvCodeMap.Param.TOKEN_TIMESTAMP, tokenTimestamp);
        loginPackage.AddParam(GameSrvCodeMap.Param.VERSION, appCfg.Version);
        gameSrvClient.LoginPackage = loginPackage;
        gameSrvClient.OnLoginReturn = OnSocketLoginReturn;
        gameSrvClient.Connect(srvIp, srvPort,new GameSrvConnectionProcessor(gameSrvClient, GameSrvBeginReconnect, onClosed));
    }
    public void KickOff()
    {
        gameSrvClient.KickOff();
    }
    private void OnSocketLoginReturn(Msg msg)
    {
        int rsCode = (int)msg.GetParam(BaseCodeMap.BaseParam.RS_CODE);
        switch (rsCode)
        {
			case BaseCodeMap.BaseRsCode.SUCCESS:
			    playerid = (string)msg.GetParam (GameSrvCodeMap.Param.PLAYER_ID);
			    testLoginTimes = DEFAULT_TEST_LOGIN_TIME;
                isLogined = true;
                Cookie cookie = new Cookie(uid, token, srvid, tokenTimestamp, isBind, openid, sdkCode);
                CookieData.GetInstance().Save(cookie);
                GameSrvConnectSuccess();
				logReport.OnLogReport ("loginSuccess->pid:"+playerid+",uid:"+msg.GetParam(GameSrvCodeMap.Param.UID)+",srvid:"+msg.GetParam(GameSrvCodeMap.Param.SRVID));
                break;
			case GameSrvCodeMap.RsCode.ERRO_CODE_FORCE_UPDATE_VERSION:
				logReport.OnWarningReport ("versionNeedUpdate");
                onClosed();
                Close();
				break;
            case GameSrvCodeMap.RsCode.ERRO_CODE_TOKEN_EXPIRED:
                logReport.OnWarningReport("loginTokenExpiredAndRelogin.");
                testLoginTimes--;
                CookieData.GetInstance().Clear();
                if (testLoginTimes > 0)
                {
                    Close();
                    login(ConnectSocketComplete, this.accountSrvComplete);
                } 
                else
                    onClosed();
                break;
            case GameSrvCodeMap.RsCode.ERRO_CODE_INVALIDE_TOKEN:
                logReport.OnWarningReport("login token invalid,relogin.");
                CookieData.GetInstance().Clear();
                onClosed();
                Close();
                break;
            case GameSrvCodeMap.RsCode.ERR_CODE_SRV_ERRO:
                logReport.OnWarningReport("login fail.srv erro");
                onClosed();
                Close();
                break;
            case BaseCodeMap.BaseRsCode.TIME_OUT:
                logReport.OnWarningReport("login fail.time out");
                onClosed();
                Close();
                break;
			default:
			    logReport.OnWarningReport( "login fail,code:" + rsCode );
                CookieData.GetInstance().Clear();
                Close();
				break;
        };
        if (ConnectSocketComplete != null)
        {
			ConnectSocketComplete(Check(),msg);
            ConnectSocketComplete = null;
        }
    }
}
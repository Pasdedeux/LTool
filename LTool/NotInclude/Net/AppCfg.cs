using System.Collections;

public class AppCfg{

    private string testPlayerIMEI = "baggio10";

    public string TestPlayerIMEI
    {
        get
        {
            return testPlayerIMEI;
        }

        set
        {
            testPlayerIMEI = value;
        }
    }
	string loginUrl = "http://211.149.245.65:8080/accountSrv/procServlet";//211.149.245.65
    public string LoginUrl
    {
        get
        {
            return loginUrl;
        }

        set
        {
            loginUrl = value;
        }
    }

    string version = "4.0.0";
    public string Version
    {
        get
        {
            return version;
        }

        set
        {
            version = value;
        }
    }
    string pkgVersion;
    public string PkgVersion { get { return pkgVersion; } set { pkgVersion = value; } }
}

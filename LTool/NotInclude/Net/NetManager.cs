using System;
using common.gameData.payment;
using UnityEngine;
public class NetManager : MonoBehaviour {

    public static NetManager inst;
    
    void Awake()
    {
        inst = this;
    }
    void Start()
    {
        NetService.getInstance().Start();
        PaymentService.getInstance().Start();
    }
	// Update is called once per frame
	void Update () 
    {
        DateTime runtime = DateTime.Now;
        NetService.getInstance().Update();
        PaymentService.getInstance().Update(runtime);
    }
    void OnDestroy()
    {
        NetService.getInstance().OnDestroy();
        LoggerFactory.getInst().OnDestroy();
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
            NetService.getInstance().Pause();
        else
            NetService.getInstance().Active();
    }
}

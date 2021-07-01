using LitFramework;
using LitFramework.GameFlow;
using LitFramework.InputSystem;
using LitFramework.LitTool;
using LitFramework.Mono;
using UnityEngine;

public class LoadingStart : MonoBehaviour
{
    private AsyncOperation _asyncOperation;

    void Start()
    {
        LitFrameworkFacade.Instance.StartUp( () =>
        {
            //启用UIMANAGER 渐变功能
            UIManager.Instance.FadeImage.CrossFadeAlpha( 0, 0.4f, false );
            ColorUtility.TryParseHtmlString( "#0B477B", out Color color );
            UIMaskManager.Instance.SetMaskColor( color );

            //TODO 可以从这里开始，写入需要初始化的一切信息
            UIManager.Instance.Show( ResPath.UI.UILOADING );
            //其它初始化项..
            //..

            VibrateManager.Instance.Install();
        } );

        //TODO 项目当中需要切换场景时，可以使用这个方法，具体功能可以参考参数说明
        //GameFlowController.Instance.ChangeScene( 1, loadingUIPath: ResPath.UI.UILOADING, needFading: true );
    }

    
}

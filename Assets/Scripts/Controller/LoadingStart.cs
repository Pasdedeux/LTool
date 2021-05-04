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
            UIManager.Instance.UseFading = true;
            UIManager.Instance.FadeImage.CrossFadeAlpha( 0, 0.4f, false );
            ColorUtility.TryParseHtmlString( "#0B477B", out Color color );
            UIMaskManager.Instance.SetMaskColor( color );
        } );

        //GameFlowController.Instance.ChangeScene( 1, loadingUIPath: ResPath.UI.UILOADING, needFading: true );
    }

    
}

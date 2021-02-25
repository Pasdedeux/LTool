using LitFramework;
using LitFramework.GameFlow;
using LitFramework.LitTool;
using LitFramework.Mono;
using UnityEngine;

public class LoadingStart : MonoBehaviour
{
    private AsyncOperation _asyncOperation;

    void Start()
    {
        DontDestroyOnLoad( GameObject.Find( "Canvas_Root" ) );
        LDebug.Enable = true;

        ResourceManager.Instance.Install();
        //System install
        UIManager.Instance.UseFading = true;
        UIManager.Instance.LoadResourceFunc = ( e ) => { return Resources.Load( e ) as GameObject; };
        UIManager.Instance.FadeImage.CrossFadeAlpha( 0, 0.4f, false );
        UIManager.Instance.Install();
        
        //Audio System
        AudioManager.Instance.IsEnabled = true;
        AudioManager.Instance.LoadResourceFunc = ( e ) => { return Resources.Load( e ) as AudioClip; };
        AudioManager.Instance.Install();
        
        //LoadingTaskModel.Instance.AddTask( 6, () =>
        //{
        //    //DataModel.Instance.LoadData();
        //    return true;
        //} );
        //LoadingTaskModel.Instance.AddTask( 0, () =>
        //{
        //    _asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync( 1, UnityEngine.SceneManagement.LoadSceneMode.Single );
        //    _asyncOperation.allowSceneActivation = false;
        //    return true;
        //}, true );
        //LoadingTaskModel.Instance.AddTask( 1, () =>
        //{
        //    bool over = _asyncOperation.progress >= 0.9f;
        //    return over;
        //}, true );
        //LoadingTaskModel.Instance.AddTask( 99, () =>
        //{
        //    _asyncOperation.allowSceneActivation = true;
        //    return true;
        //}, true );
        //LoadingTaskModel.Instance.AddTask( 100, () =>
        //{
        //    LoadingTaskModel.Instance.ClearTask();
            
        //    //UIManager.Instance.Close( DataModel.UI.UI_LOADING, true );
        //    //UIManager.Instance.Show( DataModel.UI.UI_MAIN );
        //    UIManager.Instance.FadeImage.raycastTarget = false;
        //    return true;
        //}, true );

        //UIManager.Instance.Show( DataModel.UI.UI_LOADING );
    }
}

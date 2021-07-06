/*======================================
* 项目名称 ：Assets.Scripts.Controller
* 项目描述 ：
* 类 名 称 ：URPCamManager
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Controller
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/7/6 15:08:10
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using LitFrameworkEditor.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if URP
using UnityEngine.Rendering.Universal;
#endif

namespace Assets.Scripts.Controller
{
    [RequireComponent( typeof( Camera ) )]
    [DisallowMultipleComponent]
    public class URPCam : MonoBehaviour
    {
        #if URP
        public static UniversalAdditionalCameraData BaseCamera;

        [Tooltip( "整个游戏只能有1个Base，其他都是Overlay" )]
        public CameraRenderType cameraRenderType = CameraRenderType.Overlay;

        [Tooltip( "相机depth（赋值0，取老的Camera.depth）" )]
        public float cameraDepth;

        private Camera _camera;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            UniversalAdditionalCameraData universalAdditionalCameraData = _camera.GetUniversalAdditionalCameraData();
            universalAdditionalCameraData.renderType = cameraRenderType;
            if ( cameraRenderType == CameraRenderType.Base )
            {
                if ( BaseCamera != null )
                {
                    LDebug.LogError( string.Format( "URP Camera 配置错误，有多个Base Camera！ Old = {0}, New = {1}  ", BaseCamera.name, _camera.name ) );
                }
                else
                {
                    BaseCamera = universalAdditionalCameraData;
                }
            }

            if ( cameraDepth == 0 )
                cameraDepth = _camera.depth;
#endif
        }

#if URP
    private void Start()
        { 
            if ( cameraRenderType == CameraRenderType.Overlay )
            {
                if ( BaseCamera != null )
                {
                    if ( !BaseCamera.cameraStack.Contains( _camera ) )
                        BaseCamera.cameraStack.Add( _camera );

                    BaseCamera.cameraStack.Sort( ( A, B ) => { return A.gameObject.GetComponent<URPCam>().cameraDepth.CompareTo( B.gameObject.GetComponent<URPCam>().cameraDepth ); } );
                }
                else
                {
                    LDebug.LogError( "URP Camera 配置错误，没有Base Camera！" );
                }
            }
        }

        private void OnDestroy()
        {
            if ( cameraRenderType == CameraRenderType.Overlay )
            {
                if ( BaseCamera != null )
                {
                    BaseCamera.cameraStack.Remove( _camera );
                }
            }
        }
    }
#endif
}

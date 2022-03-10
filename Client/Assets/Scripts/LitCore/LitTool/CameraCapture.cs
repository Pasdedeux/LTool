/*======================================
* 项目名称 ：LitFramework
* 项目描述 ：
* 类 名 称 ：AssetPathManager
* 类 描 述 ：
* 命名空间 ：LitFramework.EditorTool
* 机器名称 ：SKY-20170413SEJ 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2018/5/9 14:22:01
* 更新时间 ：2018/5/9 14:22:01
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2018. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2018/5/9 14:22:01
*修改人： LHW
*版本号： V1.0.0.0
*描述：
*
======================================*/



namespace LitFramework.LitTool
{
    using UnityEngine;
    using System.IO;

    /// <summary>
    /// 截屏功能
    /// </summary>
    public class TextureCapture : MonoBehaviour
    {
        /// <summary>
        /// 截图尺寸
        /// </summary>
        public enum CaptureSize
        {
            CameraSize,
            ScreenResolution,
            FixedSize
        }

        
        /// <summary>
        /// 保存截图 ，默认存储于TempPath
        /// </summary>
        /// <param name="targetCamera">目标摄像机</param>
        /// <param name="captureSize">截图尺寸</param>
        /// <param name="outPutName">文件名称</param>
        /// <param name="outPut">存储路径 - 外部输入格式: AssetPathManager.Instance.GetTemporaryCachePath( outPutName , false )</param>
        /// <param name="fixedSizeX">指定截图尺寸下的纵横像素值X</param>
        /// <param name="fixedSizeY">指定截图尺寸下的纵横像素值Y</param>
        public static Texture2D SaveCapture( Camera targetCamera , CaptureSize captureSize = CaptureSize.ScreenResolution,  string outPutName = "cameraCapture.png", string outPut = "default" , float fixedSizeX = 0f, float fixedSizeY = 0f )
        {
            Vector2 size = new Vector2( fixedSizeX, fixedSizeY );
            if ( captureSize == CaptureSize.CameraSize )
            {
                size = new Vector2( targetCamera.pixelWidth, targetCamera.pixelHeight );
            }
            else if ( captureSize == CaptureSize.ScreenResolution )
            {
                size = new Vector2( Screen.currentResolution.width, Screen.currentResolution.height );
            }
            else if( captureSize == CaptureSize.FixedSize )
            {
               if( size == Vector2.zero )
                {
                    size = new Vector2( Screen.currentResolution.width, Screen.currentResolution.height );
                    LDebug.LogWarning( "[SaveCapture]当前截屏方式为CaptureSize.FixedSize，且未指定区域像素范围。采用当前屏幕分辨率横纵像素值!" );
                }
            }
            string path = outPut.Equals( "default" )  ?AssetPathManager.Instance.GetTemporaryCachePath( outPutName , false ) : outPut;
            Texture2D captureResult = Capture( targetCamera, ( int )size.x, ( int )size.y );
            SaveTexture( path, captureResult );

            return captureResult;
        }

        /// <summary> 相机截图 </summary>
        /// <param name="camera">目标相机,设备屏幕宽高</param>
        public static Texture2D Capture( Camera camera )
        {
            return Capture( camera, Screen.width, Screen.height );
        }

        /// <summary>
        /// 从RenderTexture处获取Texture2D
        /// </summary>
        /// <param name="renderT"></param>
        /// <returns></returns>
        public static Texture2D GetTexture2d( RenderTexture renderT )
        {
            if ( renderT == null )
                return null;

            int width = renderT.width;
            int height = renderT.height;
            Texture2D tex2d = new Texture2D( width, height, TextureFormat.ARGB32, false );
            RenderTexture.active = renderT;
            tex2d.ReadPixels( new Rect( 0, 0, width, height ), 0, 0 );
            tex2d.Apply();
            
            return tex2d;
        }

        /// <summary> 相机截图 </summary>
        /// <param name="camera">目标相机</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public static Texture2D Capture( Camera camera, int width, int height )
        {
            RenderTexture rt = new RenderTexture( width, height, 0 );
            rt.depth = 24;
            rt.antiAliasing = 8;
            camera.targetTexture = rt;
            camera.RenderDontRestore();
            RenderTexture.active = rt;
            Texture2D texture = new Texture2D( width, height, TextureFormat.ARGB32, false, true );
            Rect rect = new Rect( 0, 0, width, height );
            texture.ReadPixels( rect, 0, 0 );
            texture.filterMode = FilterMode.Point;
            texture.Apply();
            camera.targetTexture = null;
            RenderTexture.active = null;
            Destroy( rt );
            return texture;
        }

        /// <summary> 保存贴图 </summary>
        /// <param name="path">保存路径</param>
        /// <param name="texture">Texture2D</param>
        public static void SaveTexture( string path, Texture2D texture )
        {
            File.WriteAllBytes( path, texture.EncodeToPNG() );
            Debug.Log( "已保存截图到:" + path );
        }



        /// <summary>
        /// Texture2D 转换为 RenderTexture
        /// </summary>
        /// <param name="targetT">需要转换的Texture2D</param>
        /// <param name="imgWidth">目标RenderTexture像素宽度</param>
        /// <param name="imgHeiht">目标RenderTexture像素高度</param>
        /// <returns></returns>
        public static RenderTexture Texture2DToRendertexture( Texture2D targetT, int imgWidth, int imgHeiht )
        {
            //width height depth(深度缓冲区中的位数（0、16或24）。请注意只有24位深度具有模板缓冲区)
            RenderTexture computeTex = new RenderTexture( imgWidth, imgHeiht, 24 );
            //设置了enableRandomWrite标记,这使你的compute shader 有权写入贴图
            computeTex.enableRandomWrite = true;
            //不执行create(),Shader执行结束像素也不会被修改
            computeTex.Create();
            //利用Graphics.Blit方法给render texture 初始化赋值 Graphics.Blit(2dtextre,destination rendertexture)
            Graphics.Blit( targetT, computeTex);

            return computeTex;
        }


        /// <summary>
        /// Texture2D 转换为 Sprite
        /// </summary>
        /// <param name="targetT">需要转换的Texture2D</param>
        /// <param name="ImageWidth">目标RenderTexture像素宽度</param>
        /// <param name="imageHeight">目标RenderTexture像素高度</param>
        /// <returns></returns>
        public static Sprite Texture2DToSprite( Texture2D targetT, int ImageWidth, int imageHeight )
        {
            return Sprite.Create( targetT, new Rect( 0, 0, ImageWidth, imageHeight ), new Vector2( 0.5f, 0.5f ) );

        }
    }
}

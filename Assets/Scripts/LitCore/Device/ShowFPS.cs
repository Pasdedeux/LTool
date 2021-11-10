/**************************************************************** 
 * 作    者：Derek Liu 
 * CLR 版本：4.0.30319.42000 
 * 创建时间：2018/1/31 17:30:34 
 * 当前版本：1.0.0.1 
 *  
 * 描述说明： 
 * 
 * 修改历史： 
 * 
***************************************************************** 
 * Copyright @ Derek Liu 2018 All rights reserved 
*****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LitFramework.Device
{
    public class ShowFPS:MonoBehaviour
    {
        public float fpsMeasuringDelta = 2.0f;

        private float timePassed;
        private int m_FrameCount = 0;
        private float m_FPS = 0.0f;
        GUIStyle titlestyle;
        public static string customstring = "";
        void Awake()
        {
            titlestyle = new GUIStyle();
            titlestyle.fontSize = 50;
            titlestyle.normal.textColor = Color.white;

        }

        void Start()
        {
            timePassed = 0.0f;
        }

        void OnGUI()
        {
            GUI.Label( new Rect( 0 , 0 , 200 , 30 ) , "FPS: "+m_FPS.ToString( "f1" ) , titlestyle );
            GUI.Label( new Rect( 400 , 0 , 500 , 30 ) , "QualityLevel: "+ QualitySettings.GetQualityLevel().ToString() , titlestyle );
            GUI.Label( new Rect( 800 , 0 , 800 , 30 ) , "TargetFrameRate: "+Application.targetFrameRate.ToString() , titlestyle );
            GUI.Label( new Rect( 0 , 60 , 800 , 30 ) , customstring , titlestyle );
        }

        void Update()
        {
            m_FrameCount = m_FrameCount + 1;
            timePassed = timePassed + Time.deltaTime;

            if ( timePassed > fpsMeasuringDelta )
            {
                m_FPS = m_FrameCount / timePassed;

                timePassed = 0.0f;
                m_FrameCount = 0;
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Reflection;

namespace ET
{
    public class GUITest : MonoBehaviour
    {
        public static Scene zonScene;

        public static async void ChooseServer1()
        {
            zonScene.GetComponent<ServerInfoComponent>().CurrentServerId = 1;
            await ETTask.CompletedTask;
        }

        public static async void ChooseServer2()
        {
            zonScene.GetComponent<ServerInfoComponent>().CurrentServerId = 2;
            await ETTask.CompletedTask;
        }

        public static async void EnterServer()
        {
            //获取指定区服信息角色信息
            int errorCode = await LoginHelper.GetRoles(zonScene);

            if (errorCode != ErrorCode.ERR_Success)
            {
                Log.Error(errorCode.ToString());
                return;
            }

            Log.Debug("LoginHelper.GetRoles");
        }

        public static async void CreateRole()
        {
            int errorCode = await LoginHelper.CreateRole(zonScene, "创建角色");

            if (errorCode != ErrorCode.ERR_Success)
            {
                Log.Error(errorCode.ToString());
                return;
            }

            Log.Debug("LoginHelper.CreateRole");
        }


        public static async void DeleteRole()
        {
            int errorCode = await LoginHelper.DeleteRole(zonScene);

            if (errorCode != ErrorCode.ERR_Success)
            {
                Log.Error(errorCode.ToString());
                return;
            }

            Log.Debug("LoginHelper.DeleteRole");
        }


        private void OnGUI()
        {
            //第一行
            int index = 0, vindex = 0;
            if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "选择进入1服"))
            {
                ChooseServer1();
            }
            if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "选择进入2服"))
            {
                ChooseServer2();
            }
            if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "进入角色服"))
            {
                EnterServer();
            }
            index = 0; vindex++;
            if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "创建角色"))
            {
                CreateRole();
            }
            if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "删除角色"))
            {
                DeleteRole();
            }
            if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "进入游戏"))
            {
                
            }
        }

    }

}
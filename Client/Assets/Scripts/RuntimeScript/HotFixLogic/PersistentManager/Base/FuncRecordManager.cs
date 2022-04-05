using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitFramework;
using LitFramework.Base;

namespace LitFramework.Persistent
{
    /// <summary>
    /// 功能记录
    /// </summary>
    public class FuncRecordManager : BaseLocalDataManager<FuncRecordManager, FuncRecordLocalData>
    {

        /// <summary>
        /// 是否当天第一次打开功能
        /// </summary>
        /// <param name="aName">名字</param>
        public bool IsOneOpenFunc(string aName)
        {
            long oldDaySC;
            bool isHave = LocalData.FuncTime.TryGetValue(aName, out oldDaySC);
            if (!isHave)
            {
                return true;
            }

            long nowDay = LitTool.LitTool.GetTimeStamp() / ConstantTool.TIME_DAY_SC;
            long oldDay = oldDaySC / ConstantTool.TIME_DAY_SC;
            if (nowDay == oldDay)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 距离现在的天数
        /// </summary>
        public int OpenFuncDisNowDay(string aName)
        {
            long oldDaySC;
            bool isHave = LocalData.FuncTime.TryGetValue(aName, out oldDaySC);
            if (!isHave)
            {
                return -1;
            }
            long nowDay = LitTool.LitTool.GetTimeStamp() / ConstantTool.TIME_DAY_SC;
            long oldDay = oldDaySC / ConstantTool.TIME_DAY_SC;
            int day = (int)(nowDay - oldDay);
            if (nowDay == oldDay)
            {
                return 0;
            }
            return day;
        }
        /// <summary>
        /// 距离现在的秒数
        /// </summary>
        public long OpenFuncDisNowTimeS(string aName)
        {
            long oldDaySC;
            bool isHave = LocalData.FuncTime.TryGetValue(aName, out oldDaySC);
            if (!isHave)
            {
                return -1L;
            }

            return LitTool.LitTool.GetTimeStamp() - oldDaySC;
        }
        /// <summary>
        /// 获取功能刷新时间戳
        /// </summary>
        public long OpenFuncTime(string aName)
        {
            long oldDaySC;
            bool isHave = LocalData.FuncTime.TryGetValue(aName, out oldDaySC);
            if (!isHave)
            {
                return -1L;
            }

            return oldDaySC;
        }
        /// <summary>
        /// 保存当天第一次打开功能时间戳
        /// </summary>
        /// <param name="aName">名字</param>
        /// <param name="aTime">保存时间，-2为当前时间戳</param>
        public void SaveOneOpenFunc(string aName, long aTime = -2)
        {
            aTime = aTime == -2 ? LitTool.LitTool.GetTimeStamp() : aTime;
            LocalData.FuncTime[aName] = aTime;
            LocalData.SaveFlag();

        }
        /// <summary>
        /// 删除功能保存
        /// </summary>
        public void RemoveFuncSave(string aName)
        {
            if (LocalData.FuncTime.ContainsKey(aName))
            {
                LocalData.FuncTime.Remove(aName);
                LocalData.SaveFlag();
            }
        }
        /// <summary>
        /// 设置功能数量
        /// </summary>
        /// <param name="aName">名字</param>
        /// <param name="aNum">数量</param>
        public void SetFuncNum(string aName, int aNum)
        {
            LocalData.FuncNum[aName] = aNum;
            LocalData.SaveFlag();
        }
        /// <summary>
        /// 添加功能数量
        /// </summary>
        /// <param name="aName">名字</param>
        /// <param name="aNum">数量</param>
        public void AddFuncNum(string aName, int aNum = 1)
        {

            int old = 0;
            bool isHave = LocalData.FuncNum.TryGetValue(aName, out old);
            LocalData.FuncNum[aName] = aNum + old;
            LocalData.SaveFlag();
        }
        /// <summary>
        /// 获取功能数量
        /// </summary>
        /// <param name="aName">名字</param>
        /// <param name="aNum">数量</param>
        public int GetFuncNum(string aName)
        {
            int old = 0;
            bool isHave = LocalData.FuncNum.TryGetValue(aName, out old);
            return old;
        }
        /// <summary>
        /// 重置功能数量
        /// </summary>
        public void ResetFuncNum(string aName)
        {
            LocalData.FuncNum[aName] = 0;
            LocalData.SaveFlag();
        }

        public override void Install()
        {

        }
    }
}

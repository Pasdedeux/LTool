using System;
using System.Reflection;
using UnityEngine;
namespace common.gameData
{
    class BaseDao<T>
    {
        private UnityLogReport logReport = LoggerFactory.getInst().getUnityLogger();
        protected int tableField;
        public int TableField { get { return tableField; } }
        public BaseDao(int tableField)
        {
            this.tableField = tableField;
        }
        public void Save(Po po)
        {
            byte[] bytes = po.GetBinData();
            string info = Convert.ToBase64String(bytes);
            PlayerPrefs.SetString(tableField.ToString(), info);
            PlayerPrefs.Save();
            logReport.OnLogReport("save tableFiled:"+ tableField + " data to local,data size:"+bytes.Length+",saveInfo:"+info);
        }
        public void Clear()
        {
            PlayerPrefs.SetString(tableField.ToString(), null);
        }
        public T Load(Type type)
        {
            try
            {
                Type[] paramTypes = new Type[1];
                paramTypes[0] = typeof(byte[]);
                ConstructorInfo constructor = type.GetConstructor(paramTypes);
                string info = PlayerPrefs.GetString(tableField.ToString(), null);
                if (info == null||"".Equals(info))
                    return default(T);
                byte[] bytes = Convert.FromBase64String(info);
                return (T)constructor.Invoke(new object[] { bytes });
            }
            catch(Exception e)
            {
                logReport.OnWarningReport("load tableFiled:" + tableField + " fail:"+e.Message);
            }
            return default(T);
        }
    }
}
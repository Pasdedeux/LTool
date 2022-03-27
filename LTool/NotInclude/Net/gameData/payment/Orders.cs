using System;
using System.Collections.Generic;
namespace common.gameData.payment
{
    class Orders:Po
    {
        private UnityLogReport logReport = LoggerFactory.getInst().getUnityLogger();
        private const int FIELD_ORDERS = 0;
        private Dictionary<string, Order> orders;
        private Dictionary<object, object> ordersSaveData;
        public Orders() : base() { }
        public Orders(byte[] data):base(data) { }
        public override void Init()
        {
            orders = new Dictionary<string, Order>();
            ordersSaveData = new Dictionary<object, object>();
            saveData.Add(FIELD_ORDERS, ordersSaveData);
        }
        public override void ReadData(Dictionary<object, object> input)
        {
            logReport.OnLogReport("orders data read ...");
            if (!input.ContainsKey(FIELD_ORDERS))
                return;
            ordersSaveData = (Dictionary<object, object>)input[FIELD_ORDERS];
            saveData[FIELD_ORDERS] = ordersSaveData;
            foreach (var o in ordersSaveData)
            {
                string orderid = (string)o.Key;
                Dictionary<object,object> iput = (Dictionary<object, object>)o.Value;
                Order order = new Order(iput);
                orders.Add(orderid, order);
            }
        }
        public int Count()
        {
            return orders.Count;
        }
        public void AddOrder(Order order)
        {
            string id = order.Orderid;
            orders.Add(id, order);
            SynAddToSaveData(order);
        }
        private void SynAddToSaveData(Order order)
        {
            string id = order.Orderid;
            Dictionary<object, object> data = order.SaveData;
            ordersSaveData.Add(id, data);
            logReport.OnLogReport("SynAddToSaveData,order id:"+id+",count:"+ ordersSaveData.Count);
        }
        public bool RemoveOder(string id)
        {
            if (!orders.ContainsKey(id))
                return false;
            orders.Remove(id);
            SynRemoveSaveData(id);
            return true;
        }
        private void SynRemoveSaveData(string id)
        {
            if (ordersSaveData.ContainsKey(id))
                ordersSaveData.Remove(id);
        }
        public List<Order> GetTimeOutOrders(DateTime runtime, int timeOut)
        {
            List<Order> rs = null;
            if (orders.Count == 0)
                return rs;
            foreach (var o in orders)
            {
                Order v = o.Value;
                if (v.IsPaySuccessTimeOut(runtime, timeOut))
                {
                    if (rs == null)
                        rs = new List<Order>();
                    rs.Add(v);
                }
            }
            return rs;
        }
        public Order GetOrderById(string orderid)
        {
            if (!orders.ContainsKey(orderid))
                return null;
            return orders[orderid];
        }
    }
}
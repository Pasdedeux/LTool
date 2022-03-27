namespace common.gameData.Item
{
    public class ItemChangeObj
    {
        private int itemCid;
        public int ItemCid { get { return itemCid; } }
        private int itemAddNum;
        public int ItemAddNum { get { return itemAddNum; } }
        private int itemChangeNum;
        public int ItemChangeNum { get { return itemChangeNum; } }
        public ItemChangeObj(int itemCid,int itemAddNum,int itemChangeNum)
        {
            this.itemCid = itemCid;
            this.itemAddNum = itemAddNum;
            this.itemChangeNum = itemChangeNum;
        }
    }
}

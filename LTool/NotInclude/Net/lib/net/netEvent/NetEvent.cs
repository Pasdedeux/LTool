namespace common.net.netEvent
{
    /// <summary>
    /// 网络事件
    /// </summary>
    public abstract class NetEvent
    {
        public NetEvent()
        {
        }
        /// <summary>
        /// 触发
        /// </summary>
        public abstract void Fire();
    }
}

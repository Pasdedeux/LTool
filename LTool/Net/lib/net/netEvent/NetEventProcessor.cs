using common.lib.core.collections.concurrent;
namespace common.net.netEvent
{
    public class NetEventProcessor
    {
        protected LinkedBlockingQueue<NetEvent> events = new LinkedBlockingQueue<NetEvent>();
        public void Trigger(NetEvent netEvent)
        {
            events.Put(netEvent);
        }
        protected void Fire()
        {
            while (events.GetCount() != 0)
            {
                NetEvent netEvent = events.Poll();
                if (netEvent != null)
                    netEvent.Fire();
            }
        }
    }
}

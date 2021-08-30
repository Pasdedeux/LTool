namespace common.net.netEvent
{
    public class BlockingNetEventProcessor:NetEventProcessor
    {
        protected int timeout = 1000;
        public new void Fire()
        {
            while (events.GetCount() != 0)
            {
                NetEvent netEvent = events.Poll(timeout);
                if (netEvent != null)
                    netEvent.Fire();
            }
        }
    }
}

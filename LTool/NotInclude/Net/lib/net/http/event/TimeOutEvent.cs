using common.core;
using common.net.netEvent;
namespace common.net.http
{
    public class TimeOutEvent : NetEvent
    {
        private HttpClient client;
        private Msg msg;
        public TimeOutEvent(HttpClient client, Msg msg)
        {
            this.client = client;
            this.msg = msg;
        }
        public override void Fire()
        {
            client.OnTimeOut(msg);
        }
    }
}

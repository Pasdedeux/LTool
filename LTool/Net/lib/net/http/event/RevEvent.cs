using common.core;
using common.net.netEvent;
namespace common.net.http
{
    public class RevEvent : NetEvent
    {
        private HttpClient client;
        private Msg msg;
        public RevEvent(HttpClient client, Msg msg)
        {
            this.client = client;
            this.msg = msg;
        }
        public override void Fire()
        {
            client.RevMsg(msg);
        }
    }
}

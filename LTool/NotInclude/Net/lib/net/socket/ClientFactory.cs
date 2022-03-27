using common.core;
using common.net.socket.codec;
using common.net.socket.session;
namespace common.net.socket
{
	/// <summary>
	/// Client factory.
	/// </summary>
	public class ClientFactory{
		private static ClientFactory inst = new ClientFactory();
		private ClientFactory()
		{
        }
		public static ClientFactory getInstance()
		{
			return inst;
		}
		public Client get(SessionConfig sessionConfig,ISocketEncoder encoder,ISocketDecoder decoder, Msg heartBeatPackage)
		{
            Client client = new Client(sessionConfig, encoder, decoder, heartBeatPackage);
            return client;
        }
    }
}

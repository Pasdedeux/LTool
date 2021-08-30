using System;
using common.net.socket.session;

namespace common.net.socket.acceptor.filterchain
{
    public abstract class Filter
    {
        public Filter Next { set; get; }
        public void doChain(DateTime runtime,Session session){
            doFilter(runtime,session);
		    if(Next != null)
                Next.doChain(runtime,session);
	    }
        public abstract void doFilter(DateTime runtime,Session session);
    }
}

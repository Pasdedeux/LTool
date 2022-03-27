using System;
using System.Collections.Generic;
using common.net.socket.session;

namespace common.net.socket.acceptor.filterchain
{
    public class Filterchain
    {
        private Filter head;
        private List<Filter> filters;
        public void setFilters(List<Filter> filters)
        {
            this.filters = filters;
        }
        public void init()
        {
            Filter curFilter = null;
            foreach (var next in filters)
            {
                if (curFilter == null)
                    head = next;
                else
                    curFilter.Next = next;
                curFilter = next;
            }
        }

        public void doChain(DateTime runtime,Session session)
        {
            if(head!=null)
                head.doChain(runtime,session);
        }
    }
}

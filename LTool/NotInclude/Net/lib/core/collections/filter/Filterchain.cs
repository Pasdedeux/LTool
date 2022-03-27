using System;
namespace common.lib.core.collections.filter
{
    public class Filterchain
    {
        private Filter head;
        public Filter Head { get { return head; } }
        private Filter tail;
        public Filter Tail { get { return tail; } }
        public void DoChain(DateTime runtime)
        {
            if(head!=null)
                head.doChain(runtime);
        }
        public void Add(Filter filter)
        {
            Filter curFilter = tail;
            if (curFilter == null)
                head = filter;
            else
                curFilter.Next = filter;
            tail = filter;
        }
        public void AddFirst(Filter filter)
        {
            if (head == null)
                tail = filter;
            else
                filter.Next = head;
            head = filter;
        }
        public void RemoveHead()
        {
            if (head == null)
                return;
            head = head.Next;
        }
    }
}

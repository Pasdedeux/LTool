using System;
namespace common.lib.core.collections.filter
{
    public abstract class Filter
    {
        public Filter Next { set; get; }
        public void doChain(DateTime runtime)
        {
            doFilter(runtime);
		    if(Next != null)
                Next.doChain(runtime);
	    }
        public abstract void doFilter(DateTime runtime);
    }
}

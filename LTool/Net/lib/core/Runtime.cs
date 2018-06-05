using System.Diagnostics;
namespace common.core
{
    class Runtime
    {
        public static string GetStackTrace()
        {
            string info = null;
            //设置为true，这样才能捕获到文件路径名和当前行数，当前行数为GetFrames代码的函数，也可以设置其他参数  
            StackTrace st = new StackTrace(true);
            //得到当前的所以堆栈  
            StackFrame[] sf = st.GetFrames();
            for (int i = 0; i < sf.Length; ++i)
            {
                info = info + "\r\n" + " FileName=" + sf[i].GetFileName() + " fullname=" + sf[i].GetMethod().DeclaringType.FullName + " function=" + sf[i].GetMethod().Name + " FileLineNumber=" + sf[i].GetFileLineNumber();
            }
            return info;
        }
    }
}

using System;
namespace common.io
{
    class IoBufferException : Exception
    {
        internal const int lengthShort = -1;//长度不对
        internal const int typeUnknown = -2;//类型未知

        internal int errorCode;
        internal IoBufferException(int code)
        {
            errorCode = code;
        }
    }
}

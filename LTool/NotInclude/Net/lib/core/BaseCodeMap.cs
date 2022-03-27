namespace common.core
{
    public class BaseCodeMap
    {
        /// <summary>
        /// 基本命令码
        /// </summary>
        public class BaseCmd
        {
            /// <summary>
            /// 心跳
            /// </summary>
            public const int CMD_HEART_BEAT = 0;
        }
        /// <summary>
        /// 基本参数码
        /// </summary>
        public class BaseParam
        {
            /// <summary>
            /// 结果码返回参数
            /// </summary>
            public const int RS_CODE = 1;
        }
        /// <summary>
        /// 基本返回码
        /// </summary>
        public class BaseRsCode
        {
            public const int TIME_OUT = -1;
            /// <summary>
            /// 成功
            /// </summary>
            public const int SUCCESS = 0;
			/// <summary>
			/// The SR v error.
			/// </summary>
			public const int SRV_ERRO = 1;
        }  
    }
}
using common.core;

public class AccountSrvCodeMap
{
    public class Cmd:BaseCodeMap.BaseCmd
    {
        public const int CMD_ACCOUNT = 100;
        public const int CMD_ACCOUNT_GET_SERVER_INFO = CMD_ACCOUNT + 1;//获取服务器信息
        public const int CMD_ACCOUNT_LOGIN = CMD_ACCOUNT + 2;//登录
        public const int CMD_ACCOUNT_RELACCOUNT_BIND = CMD_ACCOUNT + 3;//三方账号绑定
		public const int CMD_ACCOUNT_REGIST = CMD_ACCOUNT + 4;//账号注册
        public const int CMD_ACCOUNT_REQ_VERIFICATION_CODE = CMD_ACCOUNT + 5;//请求验证码
        public const int CMD_ACCOUNT_UPDATE_PWD = CMD_ACCOUNT + 6;//修改密码
        public const int CMD_ACCOUNT_VERIFY_CODE = CMD_ACCOUNT + 7;//校验验证码
    }

	public class Param:BaseCodeMap.BaseParam
    {
        public const int ACCOUNT = 100;
        public const int ACCOUNT_SDK_CODE = ACCOUNT + 1;//三方code
        public const int ACCOUNT_OPEN_ID = ACCOUNT + 2;//三方账号id
        public const int ACCOUNT_UID = ACCOUNT + 3;//账号id
        public const int ACCOUNT_PWD = ACCOUNT + 4;//密码
        public const int ACCOUNT_SDK_INFO = ACCOUNT + 5;//三方验证sessioninfo
        public const int ACCOUNT_SRV_ID = ACCOUNT + 6;//服务器id
        public const int ACCOUNT_SRV_IP = ACCOUNT + 7;//服务器ip
        public const int ACCOUNT_SRV_PORT = ACCOUNT + 8;//服务器端口
        public const int ACCOUNT_TOKEN = ACCOUNT + 9;//token
        public const int ACCOUNT_TOKEN_TIMESTAMP = ACCOUNT + 10;//token时间戳
        public const int ACCOUNT_SRV_HTTP_PORT = ACCOUNT + 11;//http服务器端口
		public const int ACCOUNT_IS_BIND = ACCOUNT + 12;//账号是否被绑定
		public const int ACCOUNT_SRV_VERSION = ACCOUNT + 13;//版本号
        public const int ACCOUNT_VERIFICATION_CODE = ACCOUNT + 14;//验证码
        public const int ACCOUNT_VERIFY_CODE_TYPE = ACCOUNT + 15;//验证码类型 1 注册，2 找回密码

        public const int ACCOUNT_SDK_INFO_FIELED = 200;
        /**SDK内部uid*/
        public const int ACCOUNT_SDK_INFO_FIELED_SDK_UID = ACCOUNT_SDK_INFO_FIELED + 1;
        /**SDK指示的渠道id*/
        public const int ACCOUNT_SDK_INFO_FIELED_SDK_CHANNELID = ACCOUNT_SDK_INFO_FIELED + 2;
        /**渠道uid*/
        public const int ACCOUNT_SDK_INFO_UID = ACCOUNT_SDK_INFO_FIELED + 3;
        /**渠道username*/
        public const int ACCOUNT_SDK_INFO_USERNAME = ACCOUNT_SDK_INFO_FIELED + 4;
        /**渠道SDK登录完成后的Session ID。特别提醒：部分渠道此参数会包含特殊值如`+`，空格之类的，如直接使用URL参数传输到游戏服务器请求校验，请使用URLEncode编码。*/
        public const int ACCOUNT_SDK_INFO_TOKEN = ACCOUNT_SDK_INFO_FIELED + 5;
        /**SDK内部定义的appid*/
        public const int ACCOUNT_SDK_INFO_SDK_APPID = ACCOUNT_SDK_INFO_FIELED + 6;
    }

	public class RsCode:BaseCodeMap.BaseRsCode
	{
        public const int ERRO_ACCOUNT_OP = 100;
        public const int ERRO_ACCOUNT_OP_INVALID_REL_CODE = ERRO_ACCOUNT_OP + 1;//无效的第三方关联code
        public const int ERRO_ACCOUNT_OP_INVALID_SRV_ID = ERRO_ACCOUNT_OP + 2;//无效的服务器id
        public const int ERRO_ACCOUNT_OP_SRV_ID_IS_NULL = ERRO_ACCOUNT_OP + 3;//srvid为空
        public const int ERRO_ACCOUNT_OP_PWD = ERRO_ACCOUNT_OP + 4;//密码错误
        public const int ERRO_ACCOUNT_OP_NO_EXIST_ACCOUNT = ERRO_ACCOUNT_OP + 5;//账号不存在
        public const int ERRO_ACCOUNT_OP_RELCODE_ALREADY_BINDED = ERRO_ACCOUNT_OP + 6;//三方账号已绑定
        public const int ERRO_ACCOUNT_OP_THIS_UID_ALREAD_BINDED_AND_REL_CODE_NOT_REL_UID = ERRO_ACCOUNT_OP + 7;//当前账号已关联其他三方账号并且该三方账号未关联任何账号
        public const int ERRO_ACCOUNT_OP_BIND_SUCCESS = ERRO_ACCOUNT_OP + 8;//绑定成功
        public const int ERRO_ACCOUNT_OP_ALREADY_REGISTED = ERRO_ACCOUNT_OP + 9;//账号已注册
        public const int ERRO_ACCOUNT_OP_NOT_IN_WHITE_LIST = ERRO_ACCOUNT_OP + 10;//账号不在白名单中
        public const int ERRO_ACCOUNT_OP_VERIFICATION_CODE_SENDED = ERRO_ACCOUNT_OP + 11;//验证码已发送
        public const int ERRO_ACCOUNT_OP_VERIFICATION_CODE_INVALIDE = ERRO_ACCOUNT_OP + 12;//验证码失效，请重新获取验证码
        public const int ERRO_ACCOUNT_OP_VERIFICATION_CODE_WRONG = ERRO_ACCOUNT_OP + 13;//验证码错误
        public const int ERRO_ACCOUNT_OP_VERIFICATION_CODE_ALREADY_REGISTED = ERRO_ACCOUNT_OP + 14;//获取验证码时，账号已被注册
        public const int ERRO_ACCOUNT_OP_VERIFICATION_CODE_CANT_MODIFY_PWD = ERRO_ACCOUNT_OP + 15;//验证码密保功能，不支持修改密码
        public const int ERRO_ACCOUNT_OP_VERIFICATION_CODE_UN_REGIST_OPEN_ID_IN_MODIFY_PWD = ERRO_ACCOUNT_OP + 16;//验证码密保功能，账号未注册
    }
}
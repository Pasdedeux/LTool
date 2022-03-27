//using UnityEngine;
//using UnityEditor;
using common.core;

public class GameSrvCodeMap
{
    public class Cmd : BaseCodeMap.BaseCmd
    {
        // usr模块
        private const int CMD_USER = 1000;
        public const int CMD_USER_LOG_IN = CMD_USER + 1;// 登录
        public const int CMD_USER_LOG_OUT = CMD_USER + 2;// 登出
        public const int CMD_USER_GET_PLAYER_BASE_INFO = CMD_USER + 3;
        public const int CMD_USER_KICK_OFF = CMD_USER + 4;//当新玩家上线踢旧玩家下线

        public const int CMD_KICK_OFF = 19000;
        public const int CMD_KICK_OFF_ONLINE = CMD_KICK_OFF + 1;// 用户登录踢下线
 
        public const int CMD_PLAYER_LOGIN_TRIGGER = 2011;       //收到登陆消息后回发给服务器

        #region 多人匹配
        public const int CMD_PVP_INVITE_INVITEE = 32019;                       //邀请好友
        public const int CMD_PVP_INVITE_AGREE_OR_REFUSE = 32020;      //接收/拒绝邀请
        public const int CMD_PVP_INVITE_EXCHANGE_HOST = 32021;        //交换房主
        public const int CMD_PVP_INVITE_EXCHANGE_RACE = 32022;        //选择种族
        public const int CMD_PVP_QUIT_TEAM = 32027;                            //退出组队
        public const int CMD_PVP_TEAM_START_MATCH = 32014;             //开始匹配
        public const int CMD_PVP_TEAM_CANCEL_MATCH = 32017;          //取消匹配
        public const int CMD_PVP_TEAM_START_MATCH_RESPONSE = 32015;//匹配服开始匹配（返回）

        #endregion

        #region 背包
        private const int CMD_BAG = 2000;
        public const int CMD_BAG_SELL_ITEM = CMD_BAG + 21002;
        #endregion

        #region 邮件
        private const int CMD_MAIL = 20000;
        public const int CMD_MAIL_LIST = CMD_MAIL + 4005;
        public const int CMD_MAIL_READ = CMD_MAIL + 4002;
        public const int CMD_MAIL_AWARD = CMD_MAIL + 4004;
        public const int CMD_MAIL_DEL = CMD_MAIL + 4003;
        #endregion

        #region 玩家信息
        private const int CMD_USERINFO = 2000;
        public const int CMD_USERINFO_CHANGENAME = CMD_USERINFO + 7;
        public const int CMD_USERINFO_CHANGEICON = CMD_USERINFO + 8;
        #endregion

        #region 任务
        private const int CMD_TASK = 3000;
        public const int CMD_TSK_GET_REWARD = CMD_TASK + 1;//任务奖励领取请求

        public const int CMD_TSK_GET_ACTIVE_POINT_REWARD = CMD_TASK + 2;//活跃度宝箱领取奖励

        public const int CMD_TASK_UPDATE = CMD_TASK + 3;//任务更新

        #endregion
        #region 引导
        public const int CMD_GUIDE_NOTE = 26001;//引导记录
        public const int CMD_PVP_FINISH_COMPUTER = 32013;
        public const int CMD_PLAYER_DEFAULT_RACE = 2010;
        #endregion
        #region 活动
        public const int CMD_ACTIVITY_CHECK = 27001;//检查
        public const int CMD_ACTIVITY_AWARD = 27002;//结算
        public const int CMD_ACTIVITY_LEFT_TIME = 27003;//活动次数
        #endregion

        #region 英雄大挑战
        private const int CMD_HERO = 4000;
        public const int CMD_HERO_CHALLENGE_IN = CMD_HERO + 1;//进入战斗之前获取BOSS数据
        public const int CMD_HERO_CHALLENGE_START = CMD_HERO + 2;//进入战斗之前的检查
        public const int CMD_HERO_CHALLENGE_REWARD = CMD_HERO + 3;//战斗结算
        #endregion

        #region 守卫战
        private const int CMD_GUARD = 5000;
        public const int CMD_GUARD_BATTLE_START = CMD_GUARD + 1;//进入守卫战战斗之前的检查
        public const int CMD_GUARD_BATTLE_REWARD = CMD_GUARD + 2;//守卫战战斗结束奖励获得申请
        #endregion

        #region 单人征服
        private const int CMD_CONQUER = 29000;
        public const int CMD_CONQUER_CHECK = CMD_CONQUER + 1;//单人征服挑战检测
        public const int CMD_CONQUER_SUCCESS = CMD_CONQUER + 2;//单人征服挑战胜利
        public const int CMD_CONQUER_FAIL = CMD_CONQUER + 3;//单人征服挑战失败
        public const int CMD_CONQUER_FIRST_AWARD = CMD_CONQUER + 4;//领取单人首次通关奖励
        #endregion

        #region 英雄
        private const int CMD_HEROS_SHOW = 8000;
        /// <summary>
        /// 英雄召唤
        /// </summary>
        public const int CMD_HERO_SUMMON = CMD_HEROS_SHOW + 1;
        /// <summary>
        /// 英雄升级
        /// </summary>
        public const int CMD_HERO_LV_UP = CMD_HEROS_SHOW + 2;
        /// <summary>
        /// 【操作命令】英雄升级
        /// </summary>
        public const int SUB_CMD_HERO_LV_UP = 1;
        /// <summary>
        /// 【操作命令】技能升级
        /// </summary>
        public const int SUB_CMD_HERO_SKILL_UP = 2;
        /// <summary>
        /// 【操作命令】军衔升级
        /// </summary>
        public const int SUB_CMD_HERO_MILITARY_RANK = 3;
        /// <summary>
        /// 【操作命令】英雄强化
        /// </summary>
        public const int SUB_CMD_HERO_STRENGTH = 4;
        /// <summary>
        /// 激活槽位
        /// </summary>
        public const int CMD_HERO_EVOLUTION_ACTIVE_POSITION = CMD_HEROS_SHOW + 3;
        /// <summary>
        /// 英雄进化
        /// </summary>
        public const int CMD_HERO_EVOLUTION_LV_UP = CMD_HEROS_SHOW + 4;
        /// <summary>
        /// 英雄皮肤购买
        /// </summary>
        public const int CMD_HERO_BUY_SKIN = 8005;
        /// <summary>
        /// 英雄穿戴
        /// </summary>
        public const int CMD_HERO_EQUI_SKIN = 8006;
        /// <summary>
        /// 英雄一键升级
        /// </summary>
        public const int CMD_HERO_ONE_KEY_LV_UP = CMD_HEROS_SHOW + 7;
        /// <summary>
        /// 使用英雄体验卡
        /// </summary>
        public const int CMD_HERO_USE_EXPIRE = CMD_HEROS_SHOW + 8;
        /// <summary>
        /// 使用皮肤体验卡
        /// </summary>
        public const int CMD_HERO_SKIN_USE_EXPIRE = CMD_HEROS_SHOW + 9;
        #endregion

        #region 物品合成
        private const int CMD_SYNTHETICS = 30000;
        /// <summary>
        /// 材料合成
        /// </summary>
        public const int CMD_SYNTHETICS_MATERIAL = CMD_SYNTHETICS + 1;
        /// <summary>
        /// 碎片合成
        /// </summary>
        public const int CMD_SYNTHETICS_FRAGMENT = CMD_SYNTHETICS + 2;
        #endregion
        #region pve
        private const int CMD_DUP = 28000;
        public const int CMD_DUP_VERIFY = CMD_DUP + 2;//检查
        public const int CMD_DUP_REWARD = CMD_DUP + 1;//结算
        public const int CMD_DUP_SWEEP = CMD_DUP + 3;//扫荡
        public const int CMD_DUP_HIDDEN_RETRY = CMD_DUP + 5;//:(28005)//隐藏关重新挑战
        public const int CMD_DUP_HIDDEN_AWARD = CMD_DUP + 4;//:(28004)隐藏关结算
        #endregion

        #region 签到
        private const int CMD_SIGN = 20000;
        public const int CMD_SIGN_INFO = CMD_SIGN + 3;//获取签到列表
        public const int CMD_SIGN_IN = CMD_SIGN + 1;//签到
        public const int CMD_SIGN_CUMULATE = CMD_SIGN + 2;//结获取最终奖励
        #endregion

        #region 能量宝箱
        private const int CMD_ENERGY_BOX = 25000;
        /// <summary>
        /// 宝箱列表
        /// </summary>
        public const int CMD_ENERGY_INFO = CMD_ENERGY_BOX + 5;
        /// <summary>
        /// 宝箱兑换
        /// </summary>
        public const int CMD_ENERGY_BOX_CONVERT = CMD_ENERGY_BOX + 1;
        /// <summary>
        /// 宝箱交换位置
        /// </summary>
        public const int CMD_ENERGY_BOX_POSITION = CMD_ENERGY_BOX + 4;
        /// <summary>
        /// 免费开宝箱
        /// </summary>
        public const int CMD_ENERGY_BOX_OPEN = CMD_ENERGY_BOX + 2;
        /// <summary>
        /// 钻石开宝箱
        /// </summary>
        public const int CMD_ENERGY_DIAMOND_OPEN = CMD_ENERGY_BOX + 3;
        #endregion

        #region 抽奖
        private const int CMD_LOTTERY = 31000;
        /// <summary>
        /// 宝箱列表
        /// </summary>
        public const int CMD_LOTTERY_SINGLE_GOLD = CMD_LOTTERY + 1;
        /// <summary>
        /// 宝箱兑换
        /// </summary>
        public const int CMD_LOTTERY_TENS_GOLD = CMD_LOTTERY + 2;
        /// <summary>
        /// 宝箱交换位置
        /// </summary>
        public const int CMD_LOTTERY_SINGLE_DIAMOND = CMD_LOTTERY + 3;
        /// <summary>
        /// 免费开宝箱
        /// </summary>
        public const int CMD_LOTTERY_TENS_DIAMOND = CMD_LOTTERY + 4;
        /// <summary>
        /// 获取抽奖信息
        /// </summary>
        public const int CMD_LOTTERY_INFO = CMD_LOTTERY + 5;
        #endregion


        #region 竞技宝箱
        public const int CMD_PVP_GET_INTEGRAL_BOX_REWARD = 32004;
        #endregion

        #region 单兵升级
        private const int CMD_SINGLE_PAWN = 10000;
        /// <summary>
        /// 单兵升级
        /// </summary>
        public const int CMD_SINGLE_PAWN_LV_UP = CMD_SINGLE_PAWN + 2;
        /// <summary>
        /// 【操作命令】单兵升级
        /// </summary>
        public const int SUB_CMD_SINGLE_PAWN_LV_UP = 1;
        /// <summary>
        /// 【操作命令】单兵强化
        /// </summary>
        public const int SUB_CMD_SINGLE_PAWN_STRENGTH_LV_UP = 2;
        /// <summary>
        /// 单兵皮肤购买
        /// </summary>
        public const int CMD_SINGLE_PAWN_BUY_SKIN = CMD_SINGLE_PAWN + 3;
        /// <summary>
        /// 单兵皮肤穿戴
        /// </summary>
        public const int CMD_SINGLE_PAWN_EQUI_SKIN = CMD_SINGLE_PAWN + 4;
        #endregion

        /// <summary>
        /// 基因段合成克隆体
        /// </summary>
        public const int CMD_SYNTHETICS_CLONE = CMD_SYNTHETICS + 3;

        /// <summary>
        /// 基因段合成灵魂石
        /// </summary>
        public const int CMD_SYNTHETICS_SOULSTONE = CMD_SYNTHETICS + 4;

        /// <summary>
        /// 背包使用物品
        /// </summary>
        private const int CMD_ITEM = 7000;

        /// <summary>
        /// 背包开礼包
        /// </summary>
        public const int CMD_ITEM_USE = CMD_ITEM + 3;

        #region PVP相关
        public const int CMD_PVP_GET_PVP_SRV = 32005;

        #endregion

        #region 商城
        public const int CMD_SHOP_SHOW_ITEM = 6002;
        public const int CMD_SHOP_REFRESH_ITEM = 6003;

        public const int CMD_SHOP_BUY = 6001;
        public const int CMD_SHOP_EXCHANGE_ITEM = 6004;
        #endregion

        /// <summary>
        /// 购买体力
        /// </summary>
        public const int CMD_STRENGTH_BUY_STRENGTH = 33002;

        /// <summary>
        /// 推送：体力
        /// </summary>
        public const int CMD_STRENGTH_INFO = 35001;
        #region battle服务器相关

        /*请求*/
        public const int REQ_TRANSFER_MOVE_OPT = 2019;
        public const int NEW_CREATE_UNIT = 304;
        public const int REQ_MATCH_OPPONET = 2001;//請求pvp服務器匹配信息
        public const int REQ_AUTH_AND_GET_FIGHT_STATE = 2001;//请求Battle服务器状态
        public const int REQ_TRANSFER_USE_GLOBAL_SKILL = 2061;//全图技能
        public const int REQ_TRANSFER_UNIT_POSITION = 2025;//单位位置
        public const int REQ_TRANSFER_UNIT_MOVE = 2033;//移动
        public const int REQ_TRANSFER_UNIT_HP_SHIELDVALUE = 2023;//血量与护盾
        public const int REQ_TRANSFER_UNIT_DEAD = 2021;//单位死亡
        public const int REQ_TRANSFER_STOP_UNIT_ALL_ACTIONS = 2045;//停止单位的动作
        public const int REQ_TRANSFER_STOP = 2027;
        public const int REQ_TRANSFER_SKILL = 2013;//技能
        public const int REQ_TRANSFER_SEARCH_TARGET = 2035;//SearchTarget
        public const int REQ_TRANSFER_FOUCUS_FIRE = 2015;//集火
        public const int REQ_TRANSFER_FLASH = 2037;//闪现
        public const int REQ_TRANSFER_CRYSTAL_INFO = 2005;//据点信息
        public const int REQ_TRANSFER_CREATE_UNIT = 2029;//创建单位
        public const int REQ_TRANSFER_BUILDING_HP = 2011;//建筑血量
        public const int REQ_TRANSFER_BUILDING_DESTROYED = 2031;//敌方建筑被摧毁的消息
        public const int REQ_TRANSFER_ADD_HP = 2039;//加血
        public const int REQ_RECONNECT = 2049;//断线重连
        public const int REQ_CURRENT_FIELD_DATA = 2051;//当前数据
        public const int REQ_SYNC_TIME = 2043;//同步当前时间
        public const int REQ_SYNC_RESOURCE_NUM = 2053;//同步资源数量
        public const int REQ_SET_CRYSTAL_VISUL = 2063;//设置据点视野
        public const int REQ_QUIT_GAME = 2057;//退出游戏
        public const int REQ_LOADING_END = 2041;//加载完毕
        public const int REQ_HEART_BEAT = 2047;//心跳
        public const int REQ_FORCE_STATE_ONGOING = 2055;//改变状态至开始
        public const int REQ_BUILD_ENGING_SUCESS = 2003;//建立主机信息
        /*相应*/
        public const int CMD_UNIT_UNIT_INFOS = 403;//单位信息
        public const int RES_MATCH_OPPONET = 2002;//请求匹配服信息的回应
        public const int RES_AUTH_AND_GET_FIGHT_STATE = 2002;//等于验证回复
        public const int RES_BUILD_ENGING_SUCESS = 2004;//建立主机成功
        public const int RES_HEART_BEAT = 2048;         //心跳机制
        public const int RES_SYNC_RESOURCE_NUM = 2054;  //同步资源数量
        public const int RES_SYNC_TIME = 2044;          //同步当前时间
        public const int RES_CURRENT_FIELD_DATA = 2052;//断线重连接受当前数据
        public const int RES_RECONNECT = 2050;//断线重连
        public const int RES_TRANSFER_ADD_HP = 2040;//加血
        public const int RES_TRANSFER_BUILDING_DESTROYED = 2032;//地方建筑被摧毁的消息
        public const int RES_TRANSFER_BUILDING_HP = 2012;//建筑血量
        public const int RES_TRANSFER_CREATE_UNIT = 1003;//创建单位
        public const int RES_TRANSFER_CRYSTAL_INFO = 2006;//据点信息
        public const int RES_TRANSFER_FLASH = 2038;//闪现
        public const int RES_TRANSFER_FOUCUS_FIRE = 2016;//集火
        public const int RES_TRANSFER_MOVE_OPT = 2020;//移动
        public const int RES_TRANSFER_SEARCH_TARGET = 2036;//寻找目标
        public const int RES_TRANSFER_SKILL = 2014;//技能
        public const int RES_TRANSFER_STOP = 2028;//停止
        public const int RES_TRANSFER_STOP_UNIT_ALL_ACTIONS = 2046;//停止单位的动作
        public const int RES_TRANSFER_UNIT_DEAD = 2022;//单位死亡
        public const int RES_TRANSFER_UNIT_HP_SHIELDVALUE = 2024;//血量与护盾
        public const int RES_TRANSFER_UNIT_MOVE = 2034;//移动
        public const int RES_TRANSFER_UNIT_POSITION = 2026;//单位位置
        public const int RES_TRANSFER_USE_GLOBAL_SKILL = 2062;//全图技能
        public const int CMD_ROOM_FIGHT_END = 321;//游戏结束的消息
        public const int CMD_ROOM_LOGIN_RES = 306;//成功登陆相应

        /*
         *  2017-09-25 新加
         */
        public const int MANY_FIND_PAT = 1002;  //群体移动

        #endregion

        #region 新的BattleServer
        public const int CMD_ROOM_LOGIN_REQ = 305;
        public const int CMD_ROOM_ROOM_INFO_RES = 308;
        public const int CMD_ROOM_ROOM_START = 309;
        public const int CMD_PLAYER_LOADING_PROGRESS_REQ = 201;//加载进度表示加载完毕
        public const int CMD_ROOM_START_OCCUPY_STRONG_HOLD_REQ = 310;//占领据点
        public const int CMD_ROOM_START_OCCUPY_STRONG_HOLD_RES = 311;//占领据点相应
        public const int CMD_ROOM_CANCEL_OCCUPY_STRONG_HOLD_RES = 312;//取消占领
        public const int CMD_ROOM_SUCCESS_OCCUPY_STRONG_HOLD_RES = 313;//占领成功
        public const int CMD_PLAYER_RESOURCE_DETAIL = 203;//同步资源
        public const int CMD_ROOM_ROOM_INFO_REQ = 307;//获取房间内数据
        public const int CMD_ROOM_ROOM_STATE = 326;
        public const int CMD_BATTLE_SERVER_RES_CONNECT = 603;//游戏服推送的消息
        public const int CMD_ROOM_ROOM_RESTART = 325;//通知某玩家房间重新开始
        public const int CMD_UNIT_UNIT_ATTACK_RES = 406;//攻击请求相应
        public const int CMD_UNIT_UNIT_ATTACK_REQ = 405;//攻击请求
        public const int CMD_UNIT_SHAN_XIAN_REQ = 409;//闪现请求
        public const int CMD_UNIT_BACK_HOME_RES = 411;//回城的结果相应
        public const int CMD_PVP_BATTLE_END_GAME_SERVER_RESULT = 32011;//向游戏服请求战斗服务结束的消息
        public const int CMD_PVP_MATCH_CANCLE = 32009;//取消匹配
        public const int CMD_PVP_UPDATE_DATA = 32001;//人机结算申请服务器记录

        public const int CMD_ROOM_FIGHT_GIVEUP = 322;//放弃战斗
        public const int CMD_MATCH_TIME_OUT = 34001;//匹配失败
        #endregion

        #region 账号相关
        /// <summary>
        /// 请求验证码
        /// </summary>
        public const int CMD_ACCOUNT_GET_VERIFY_CODE = 105;
        /// <summary>
        /// 检查验证码
        /// </summary>
        public const int CMD_ACCOUNT_CHECK_VERIFY_CODE = 107;
        /// <summary>
        /// 修改密码
        /// </summary>
        public const int CMD_ACCOUNT_CHANGE_PASSWORD = 106;
        #endregion
        #region 支付
        public const int CMD_PAY = 9000;
        public const int CMD_PAY_REQ = CMD_PAY + 1;// 支付请求
        public const int CMD_PAY_NOTICE_PAY_SUCCESS = CMD_PAY + 2;// 支付成功通知
        public const int CMD_PAY_INNER_NOTICE_PAY_SUCCESS = CMD_PAY + 3;// 支付成功通知
        public const int CMD_PAY_RESET_NEW_PLAYER_BUY = CMD_PAY + 4;// 重设新玩家购买
        #endregion

        #region 运营活动
        public const int CMD_ACTIVITY_OPEN = 27004;//活动开启消息
        public const int CMD_ACTIVITY_UPDATE = 27005;//活动变更消息
        public const int CMD_ACTIVITY_CLOSE = 27006;//活动关闭消息
        public const int CMD_ACTIVITY_EX_ACTIVITY_CLOSED = 27012;//活动关闭消息（新）


        #region 六一活动
        public const int CMD_ACTIVITY_EX_NOTIFY_CHANGED = 27008;            //推送变化
        public const int CMD_ACTIVITY_EX_INIT_CONSUME_STATUS = 27009;    //玩家登陆推送初始的活动数据状态信息

        public const int CMD_ACTIVITY_EX_PICK_REWARD = 27010;  //发送领取登陆奖励

        public const int CMD_ACTIVITY_FRAGMENT_SEND_TOTALREWARD = 27011;  //发送领取登陆奖励
        #endregion
        #endregion

        #region 好友
        /// <summary>
        /// 获取好友列表
        /// </summary>
        public const int CMD_FRIEND_FRIEND_LIST = 36005;
        /// <summary>
        /// 好友详情
        /// </summary>
        public const int CMD_FRIEND_GET_PLAYER_SIMPLE_INFO = 36011;
        /// <summary>
        /// 删除好友
        /// </summary>
        public const int CMD_FRIEND_DELETE_FRIEND = 36003;
        public const int CMD_FRIEND_ON_FRIEND_REMOVED = 36010;  //推送：删除好友的推送消息
        /// <summary>
        /// 添加好友
        /// </summary>
        public const int CMD_FRIEND_INVITE_FRIEND = 36001;
        public const int CMD_FRIEND_ON_RECEIVE_FRIEND_INVITE = 36008;//推送：玩家接受添加好友的申请
        /// <summary>
        /// 反馈玩家的好友申请
        /// </summary>
        public const int CMD_FRIEND_INVITE_FRIEND_CONFIRM = 36002;
        public const int CMD_FRIEND_ON_CONFIRM_FRIEND_INVITE = 36009;//推送：我申请添加对方好友后，对方给我的反馈
        /// <summary>
        /// 推送：自己在线或离线的状态
        /// </summary>
        public const int CMD_FRIEND_ONLINE_STATUS = 36007;

        /// <summary>
        /// 发送聊天消息
        /// </summary>
        public const int CMD_CHAT_SEND = 37001;

        /// <summary>
        /// 接受聊天消息
        /// </summary>
        public const int CMD_CHAT_RECEIVE = 37002;

        /// <summary>
        /// 接受聊天消息
        /// </summary>
        public const int CMD_PAY_EVENT_SUCCESS_TRIGGER = 9005;//;
        #endregion
    }
    public class HTTPCmd
    {
        #region 排行榜
        private const int CMD_RANK = 22000;
        public const int CMD_RANK_GET_TOP = CMD_RANK + 2;
        public const int CMD_RANK_GET_PERSONAL_DATA = CMD_RANK + 3;
        public const int CMD_PVP_IN = 32003;

        #endregion
    }
    public class HTTPParam : BaseCodeMap.BaseParam
    {
        public const int PID = 3;//玩家唯一id
        public const int REQUEST_TOP_RANK_PARAS = 13;//请求排行榜数据
        public const int REQUEST_TOP_RANK_TYPES = 15;//多个top榜单的请求参数
        public const int RESPONSE_TOP_RANKS = 14;//多个排行榜的返回
        public const int RESPONSE_TOP_RANKS_PERSON_DATA = 16;//多个排行榜数据的返回
        public const int PVP_HERO_INFO = 26007;//我的英雄信息
        public const int PVP_BATTLE_RECORD = 26008;//战斗记录
        public const int PVP_PVP_ACCUMULATE_DATA = 26009;//PVP积分数据
        public const int PVP_RACE = 26003;
        public const int RANK_TYPE_3LV_CONQUER = 20000;
        public const int RANK_PLAYER_NAME = 17001;
        public const int RANK_RACE = 17003;
        public const int CONQUER_TOP_TIER = 24009;
        public const int CONQUER_PASS_TIME = 24006;
        public const int PVP_ACCUMULATE_DATA = 26009;
        public const int RANK_TYPE_3LV_PVP = 10000;
        public const int RANK_TYPE_2LV_PVP_1V1 = 100;
        public const int PVP_STAR = 26019;//PVP星数
        public const int PVP_RECENT_BATTLE = 26026;//PVP最近对战记录
        public const int PVP_RECENT_HERO = 26027;//PVP最近常用英雄
    }

    public class Param : BaseCodeMap.BaseParam
    {
        // 预留公共字段
        public const int UID = 2;// 账号id
        public const int SRVID = 3;// 服务器id
        public const int TOKEN = 4;// token
        public const int TOKEN_TIMESTAMP = 5;// token时间搓
        public const int DEVICE_ID = 6;// 设备id
        public const int VERSION = 7;//版本号
        public const int SERVER_TIME = 8;//服务器时间
        public const int DAILY_REFRESH_LEFT_TIME = 9;//下次统一刷新倒计时
        // 玩家模块
        private const int PLAYER = 1000;
        public const int PLAYER_ID = PLAYER + 1;//玩家id
        public const int PLAYER_NOVICE_GUIDE_PROGRESS = PLAYER + 2;//新手引导进度
        public const int PLAYER_CUR_DUP_ID = PLAYER + 3;//玩家当前关卡id
        public const int PLAYER_HEAD_ID = PLAYER + 4;//玩家头像Id
        public const int RANK_NAME = PLAYER + 16001;//玩家游戏名称
        public const int PLAYER_LEVEL = PLAYER + 5;//玩家等级
        public const int PLAYER_EXP = PLAYER + 6;//玩家经验
        public const int PLAYER_TOTAL_LOGIN = PLAYER + 7;//玩家本日登录次数
        public const int MONTH_CARD_IS_MONTH_CARD_USER = PLAYER + 11005;//是否是月卡玩家
        public const int PLAYER_STRENGTH = PLAYER + 8;//体力值
        public const int PLAYER_BUY_STRENGTH = PLAYER + 9;//剩余购买体力的次数
        public const int PLAYER_STRENGTH_TIME = PLAYER + 10;//体力恢复倒计时
        public const int PLAYER_IS_NEW_CREATED = PLAYER + 13;// (1013) true/false
        public const int PLAYER_CREATED_AT = PLAYER + 14;// 玩家创建时间
        public const int RACE_TERRAN_EXP = PLAYER + 18005;//人族经验值
        public const int RACE_ZERG_EXP = PLAYER + 18006;//虫族经验值
        public const int RACE_PROTOSS_EXP = PLAYER + 18007;//神族经验值
        public const int RACE_TERRAN_LV = PLAYER + 18008;//人族等级
        public const int RACE_ZERG_LV = PLAYER + 18009;//虫族等级
        public const int RACE_PROTOSS_LV = PLAYER + 18010;//神族等级
        public const int DUP_TERRAN_INFO = PLAYER + 24012;//人族关卡信息
        public const int DUP_ZERG_INFO = PLAYER + 24013;//虫族关卡信息
        public const int DUP_PROTOSS_INFO = PLAYER + 24014;//神族关卡信息
        public const int DUP_REFRESH_INFO = PLAYER + 24015;
        public const int PVP_FIGHT_DATA = 26016;
        public const int DUP_NORMAL_STARS = PLAYER + 24010;//普通关数据
        public const int DUP_ELITE_RECORD = PLAYER + 24011;//精英关数据
        public const int PLAYER_DEFAULT_RACE = 1011;//选择种族
        public const int PLAYER_ACTIVITY_INFO = 1012;//运营活动信息
        public const int OPERATE_ACTIVITY_CID = 22001;//活动配置id
        public const int OPERATE_ACTIVITY_ID = 22002;//活动数字id
        public const int OPERATE_ACTIVITY_DESCRIPTION = 22013;//活动描述
        public const int OPERATE_ACTIVITY_LEFT_TIME_END = 22009;//活动剩余结束秒数
        public const int OPERATE_ACTIVITY_EXTRA_PARAM = 22003;//活动额外信息
        public const int CLIENT_PARAM_STATUS = 22014;//奖励状态
        public const int CLIENT_PARAM_AWARD = 22015;//奖励物品
        public const int CLIENT_PARAM_AWARD_DAY = 22012;//奖励领取天数
        public const int ACTIVITY_SINGLE_PAY_AWARD_STATUS = 22016;//首冲领取奖励状态

        //道具模块
        private const int ITEM = 3000;
        public const int ITEM_DATA_DIMOND = ITEM + 5;//钻石值
        public const int ITEM_DATA_GOLD = ITEM + 6;//金币值
        public const int ITEM_DATA_CLONE = ITEM + 7;//克隆体数量
        public const int ITEM_DATA_SOULSTONE = ITEM + 8;//抽奖券数量
        public const int ITEM_DATA_PROPS = ITEM + 9;//物品列表
        public const int ITEM_CID = ITEM + 2;//使用的物品Id
        public const int ITEM_NUM = ITEM + 1;//使用的物品数量
        public const int ITEM_USE_CONVERTED_ITEMS = ITEM + 15;//被转换的物品
        public const int ITME_DATA_LOTTERY_TICKET = ITEM + 16;//抽奖券
        public const int ITEM_DATA_COUPONS = ITEM + 18;//点券
        public const int ITEM_DATA_ACTIVITY = 3012;//任务活跃度
        public const int ITEM_REMAIN_EXCHANGE_COUNT = 3019;// (3019) 剩余兑换次数
        public const int ITEM_DATA_EXPIRE_COUNT = 3020;// (3019) 剩余兑换次数
        //任务模块
        private const int TASK = 5000;
        // public const int TASK_LIST = TASK + 2;//任务列表
        public const int TSK_ID = TASK + 1;//任务id
        public const int TASK_REWARD = TASK + 2;//任务奖励
        public const int TSK_ACTIVE_POINT = TASK + 3;//活跃点
        // public const int TSK_ACTIVE_POINT = TASK + 4;//活跃点
        public const int TASK_NEW_GET_TASK_LIST = TASK + 4;//新任务列表
        public const int TASK_MAIN_LINE_TASK_LIST = TASK + 5;//主线任务列表
        public const int TASK_DAILY_TASK_LIST = TASK + 6;//日常任务列表
        public const int TASK_ACHIEVEMENT_TASK_LIST = TASK + 7;//成就任务列表
        public const int TSK_ACTIVE_GET_TIME = TASK + 8;//已经领取活跃点次数
        public const int TASK_UPDATE_TASK = TASK + 9;//获得任务更新

        //背包
        public const int SHOP_ITEM_NUM = 8002;
        public const int SHOP_ITEM_CID = 8001;
        public const int ITEM_ADD_LIST = 3004;//金币增加数量
        public const int ITEM_UPDATES = 3017;// 物品更新
        //邮件
        private const int MAIL = 20000;
        public const int MAIL_ID = MAIL + 1;//邮件Id
        public const int MAIL_TITLE = MAIL + 2;//邮件标题
        public const int MAIL_CONTENT = MAIL + 3;//邮件内容
        public const int MAIL_AWARD = MAIL + 4;//附件信息
        public const int MAIL_CREATE_TIME = MAIL + 6;//创建时间
        public const int MAIL_LIST = MAIL + 7;//邮件列表
        public const int MAIL_READ_STATE = MAIL + 8;//读取状态
        public const int MAIL_AWARD_EXIST = MAIL + 9;//是否存在附件
        public const int MMAIL_AWARD_STATE = MAIL + 10;//附件领取状态 
        public const int MAIL_SENDER_NAME = MAIL + 11;//发送者名称
        //玩家信息
        public const int USERINFO_USERNAME = 17001;//玩家名称
        public const int USERINFO_USERICON = 1004;//玩家头像
        //引导
        public const int OPTION_SETTINGS = 21001;//设置列表
        public const int OPTION_NAME = 21002;//选项名称
        public const int OPTION_VALUE = 21003;//选项值

        //活动
        public const int ACTIVITY_ID = 22002;//活动id
        public const int ACTIVITY_CID = 22001;//活动cid
        public const int ACTIVITY_RACE = 22004;//种族
        public const int ACTIVITY_EXP_VALUE = 22006;//获取经验值
        public const int ACTIVITY_GOLD_VALUE = 22007;//获取金币值
        public const int ACTIVITY_DIFFICULT = 22005;//难度
        public const int ACTIVITY_MAIL_AWARD = 3004;//奖励物品组
        public const int ENERGY_BOX_VALUE_PVE = 23002;//pve宝箱能量增加值
        public const int ACTIVITY_ACT_COUNT = 22010;//活动参与次数
        public const int ACTIVITY_CUMULATION_AWARD_DAY = 22012;//领取奖励天数
        //英雄大挑战
        private const int HERO_CHALLENGE = 6000;
        public const int HERO_CHALLENGE_CHALLENGE_INFO = HERO_CHALLENGE+1;//获得的挑战信息 
        public const int HERO_CHALLENGE_CHALLENGE_IDX = HERO_CHALLENGE + 2;//挑战序号
        public const int HERO_CHALLENGE_REWARD = HERO_CHALLENGE + 3;//挑战的结算奖励
        public const int HERO_CHALLENGE_REWARD_ENERGY_POINT = HERO_CHALLENGE + 4;//能量点奖励
        public const int HERO_CHALLENGE_IS_PACKAGE_FULL = HERO_CHALLENGE + 5;//背包是否满

        //守卫战
        private const int HERO_GUARD_BATTLE = 7000;
        public const int HERO_GUARD_BATTLE_DEGREE = HERO_GUARD_BATTLE + 1;//挑战难度
        public const int HERO_GUARD_BATTLE_BOMb_TIMES = HERO_GUARD_BATTLE + 2;//核弹次数
        public const int GUARD_BATTLE_REWARD = HERO_GUARD_BATTLE + 3;//奖励
        public const int GUARD_BATTLE_REWARD_ENERGY_POINT = HERO_GUARD_BATTLE + 4;//能量点奖励
        public const int GUARD_BATTLE_IS_PACKAGE_FULL = HERO_GUARD_BATTLE + 5;//背包是否已满
        public const int GUARD_BATTLE_CHALLEGE_TIMES = HERO_GUARD_BATTLE + 6;//英雄守卫战已挑战次数
        public const int HERO_GUARD_BATTLE_WAVE = HERO_GUARD_BATTLE + 7;//波次

        //单人征服
        private const int CONQUER_SINGLE = 24000;
        public const int CONQUER_SINGLE_INFO = CONQUER_SINGLE + 11;
        public const int CONQUER_LAST_TIER = CONQUER_SINGLE + 8;//最近挑战的层数
        public const int CONQUER_LAST_RACE = CONQUER_SINGLE + 7;//最挑战的种族
        public const int CONQUER_TOP_TIER = CONQUER_SINGLE + 9;//最高通关的层数
        public const int CONQUER_FIRST_REWARDS_STATE = CONQUER_SINGLE + 10;//通关与首次通关领取状态
        public const int CONQUER_TIER = CONQUER_SINGLE + 5;//征服层数
        public const int RACE = 19000;//种族
        public const int CONQUER_PASS_TIME = CONQUER_SINGLE + 6;//征服调整用时，单位s
        public const int ITEM_DATA_BAG_FULL = 3013;//背包是否已满
        public const int ENERGY_BOX_VALUE_CONQUER = 23003;//征服宝箱能量点
        public const int CONQUER_NORMAL_REWARDS = CONQUER_SINGLE + 1;//普通奖励信息
        public const int CONQUER_LUCKY_REWARDS = CONQUER_SINGLE + 2;//幸运奖励信息
        public const int CONQUER_FIRST_REWARDS = CONQUER_SINGLE + 4;//征服通关宝箱：首次通关奖励

        #region 英雄
        private const int HEROS = 2000;
        /// <summary>
        /// 英雄ID
        /// </summary>
        public const int HERO_CID = HEROS + 1;
        /// <summary>
        /// 操作列表
        /// </summary>
        public const int HERO_LV_UP_OPTS = HEROS + 2;
        /// <summary>
        /// 使用道具
        /// </summary>
        public const int HERO_USE_ITEM = HEROS + 3;
        /// <summary>
        /// 升级技能数据
        /// </summary>
        public const int HERO_SKILL_UP_OPTS = HEROS + 4;
        /// <summary>
        /// 【操作返回参数】英雄等级
        /// </summary>
        public const int HERO_LV = HEROS + 5;
        /// <summary>
        /// 【操作返回参数】英雄经验值
        /// </summary>
        public const int HERO_EXP = HEROS + 6;
        /// <summary>
        /// 【操作返回参数】英雄技能更新数据
        /// </summary>
        public const int HERO_SKILL_UP_RS = HEROS + 7;
        /// <summary>
        /// 【操作返回参数】英雄军阶等级
        /// </summary>
        public const int HERO_MILITARY_RANK = HEROS + 8;
        /// <summary>
        /// 【操作返回参数】英雄强化等级
        /// </summary>
        public const int HERO_STRENGTH_LV = HEROS + 9;
        /// <summary>
        /// 操作返回列表
        /// </summary>
        public const int HERO_LV_UP_OPT_RS = HEROS + 10;
        /// <summary>
        /// 英雄信息列表
        /// </summary>
        public const int HERO_DATA = HEROS + 11;
        /// <summary>
        /// 英雄激活槽位
        /// </summary>
        public const int HERO_OPEN_POSTION_OF_EVOLUTION = HEROS + 12;
        /// <summary>
        /// 英雄进化等级操作列表
        /// </summary>
        public const int HERO_EVOLUTION_UPDATE_DATA = HEROS + 13;
        /// <summary>
        /// 英雄皮肤购买的操作列表
        /// </summary>
        public const int HERO_SKINS_DATA = HEROS + 14;
        /// <summary>
        /// 英雄皮肤Id
        /// </summary>
        public const int HERO_SKIN_ID = HEROS + 15;
        /// <summary>
        /// 过期时间
        /// </summary>
        public const int FIELD_EXPIRE_AT = 1000;
        /// <summary>
        /// 是否已经执行过期的操作（0：是，1：否），服务器使用，前段不用
        /// </summary>
        public const int FIELD_IS_EXPIRE_DONE = 1001;
        /// <summary>
        /// 体验是否接受（1:正在体验中，2:不在体验中）
        /// </summary>
        public const int FIELD_PHANTOM_VISIBLE = 2000;
        /// <summary>
        /// 所有皮肤的集合
        /// </summary>
        public const int ITEM_GAME_SKIN_DATA = 3022;
        /// <summary>
        /// 单兵信息
        /// </summary>
        private const int SINGLE_SPAWN = 9000;
        /// <summary>
        /// 单兵CId
        /// </summary>
        public const int SINGLE_SPAWN_CID = SINGLE_SPAWN + 1;
        /// <summary>
        /// 单兵操作列表
        /// </summary>
        public const int SINGLE_SPAWN_LV_UP_OPTS = SINGLE_SPAWN + 2;
        /// <summary>
        /// 单兵使用道具
        /// </summary>
        public const int SINGLE_SPAWN_USE_ITEM = SINGLE_SPAWN + 3;//使用道具
        /// <summary>
        /// 单兵等级
        /// </summary>
        public const int SINGLE_SPAWN_LV = SINGLE_SPAWN + 4;//HERO_LV
        /// <summary>
        /// 单兵强化
        /// </summary>
        public const int SINGLE_SPAWN_STRENGTH_LV = SINGLE_SPAWN + 5;//强化等级
        /// <summary>
        /// 单兵列表
        /// </summary>
        public const int SINGLE_SPAWN_LV_UP_OPT_RS = SINGLE_SPAWN + 6;//操作返回列表
        /// <summary>
        /// 单兵数据
        /// </summary>
        public const int SINGLE_SPAWN_DATA = SINGLE_SPAWN + 7;//数据
        /// <summary>
        /// 英雄皮肤Id
        /// </summary>
        public const int SINGLE_SPAWN_SKIN_ID = SINGLE_SPAWN + 9;
        #endregion

        #region 物品合成
        private const int SYNTHETICS = 26000;
        /// <summary>
        /// 合成的物品Id
        /// </summary>
        public const int SYNTHETICS_ITEM = SYNTHETICS + 1;
        #endregion
        //pve
        public const int PVE_PARAM = 25000;
        public const int PVE_DUP_ID = PVE_PARAM + 1;//关卡id
        public const int PVE_RACE = 19000;//种族类型
        public const int PVE_DUP_IS_HERO_DEAD = PVE_PARAM + 6;//是否有英雄死亡
        public const int PVE_DUP_ALL_POINT_COVER = PVE_PARAM + 7;//是否全部占领据点
        public const int DUP_STAR_LV = PVE_PARAM + 2;//通关星数
        public const int DUP_RACE_EXP = PVE_PARAM + 8;//种族经验
        public const int DUP_REWARD = PVE_PARAM + 3;//奖励信息
        public const int DUP_SWEEP_TIMES = PVE_PARAM + 9;//扫荡次数   
        public const int DUP_CONVERTED_ITEMS = PVE_PARAM + 16;//被转化的物品   
        public const int DUP_HIDDEN_POINT_SUCCESS = PVE_PARAM + 17;//是否通关 
        public const int DUP_HIDDEN_POINT_GOLD = PVE_PARAM + 18;//获取的金币 
        public const int DUP_HIDDEN_POINT_DIAMOND = PVE_PARAM + 19;//获取的钻石 
        public const int DUP_HIDDEN_TRIG_STATE = PVE_PARAM + 20;//<boolean>(25020):是否触发了隐藏关卡
        public const int DUP_HIDDEN_POINT_ID = PVE_PARAM + 21;//<string>(25021):隐藏关id
        //public const int ITEM_DATA_BAG_FULL = 3013;

        //sign
        public const int SIGN_PARAM = 15000;
        public const int SIGN_CUR_MONTH = SIGN_PARAM + 5;//当前月份
        public const int SIGN_CUR_DAY = SIGN_PARAM + 6;//当前天数
        public const int SIGN_CUMULATE_COUNT = SIGN_PARAM + 9;//累计签到天数
        public const int SIGN_CUMULATE_AWARD_STATE = SIGN_PARAM + 2;//累计签到奖励状态
        public const int SIGN_MONTH_MAX_NUM = SIGN_PARAM + 7;//当前月份最大天数
        public const int SIGN_CUR_SIGN_INFO = SIGN_PARAM + 8;//当前签到信息
        public const int SIGN_IN_DAY = SIGN_PARAM + 1;//当前签到天数
        public const int SIGN_IN_AWARD = SIGN_PARAM + 3;//签到奖励信息
        public const int SIGN_CUMULATE_AWARD = SIGN_PARAM + 4;//连续签到奖励信息
        public const int SIGN_CUMULATE_GET_REWARDS_IDX = SIGN_PARAM + 10;//续签到领取奖励的序号

        #region 能量宝箱
        private const int ENERGY_BOX = 23000;
        /// <summary>
        /// 时间道具状态
        /// </summary>
        public const int ENERGY_BOX_TIME_ITEM_STATE = 23015;
        /// <summary>
        /// 能量宝箱的全部数据
        /// </summary>
        public const int ENERGY_BOX_INFO = ENERGY_BOX + 12;
        /// <summary>
        /// 系统能量槽信息
        /// </summary>
        public const int ENERGY_BOX_VALUE_INFO = ENERGY_BOX + 7;
        /// <summary>
        /// 能量槽Id
        /// </summary>
        public const int ENERGY_BOX_ENERGY_ID = ENERGY_BOX + 9;
        /// <summary>
        /// 能量值
        /// </summary>
        public const int ENERGY_BOX_VALUE = ENERGY_BOX + 6;
        /// <summary>
        /// 宝箱列表信息
        /// </summary>
        public const int ENERGY_BOX_LIST_INFO = ENERGY_BOX + 8;
        /// <summary>
        /// 宝箱index
        /// </summary>
        public const int ENERGY_BOX_ID = ENERGY_BOX + 4;
        /// <summary>
        /// 宝箱Id，交换位置之后的index
        /// </summary>
        public const int ENERGY_BOX_ID_ANOTHER = ENERGY_BOX + 5;
        /// <summary>
        /// 宝箱剩余倒计时的时间
        /// </summary>
        public const int ENERGY_BOX_LEFT_TIME = ENERGY_BOX + 10;
        /// <summary>
        /// 宝箱物品
        /// </summary>
        public const int ENERGY_BOX_ITEMS = ENERGY_BOX + 11;
        /// <summary>
        /// 宝箱状态
        /// </summary>
        public const int ENERGY_BOX_STATE = ENERGY_BOX + 13;
        /// <summary>
        /// 宝箱的物品配置Id
        /// </summary>
        public const int ENERGY_BOX_ITEM_ID = ENERGY_BOX + 14;
        #endregion

        #region 抽奖
        private const int LOTTERY = 27000;
        /// <summary>
        /// 抽奖信息
        /// </summary>
        public const int LOTTERY_INFO = LOTTERY + 5;
        /// <summary>
        /// 金币单抽的免费时间倒计时
        /// </summary>
        public const int LOTTERY_GOLD_FREE_LEFT_TIME = LOTTERY + 2;
        /// <summary>
        /// 金币单抽的免费剩余次数
        /// </summary>
        public const int LOTTERY_GOLD_FREE_LEFT_COUNT = LOTTERY + 4;
        /// <summary>
        /// 钻石单抽的免费倒计时
        /// </summary>
        public const int LOTTERY_DIAMOND_FREE_LEFT_TIME = LOTTERY + 3;
        /// <summary>
        /// 获得奖励
        /// </summary>
        public const int LOTTERY_ITEMS = LOTTERY + 1;
        /// <summary>
        /// 被转换的物品（英雄）
        /// </summary>
        public const int LOTTERY_CONVERTED_ITEMS = LOTTERY + 6;
        /// <summary>
        /// 抽奖消耗的资源
        /// </summary>
        public const int LOTTERY_COST_ITEMS = LOTTERY + 7;
        #endregion

        /// <summary>
        /// 合成的数量
        /// </summary>
        public const int SYNTHETICS_NUM = SYNTHETICS + 2;

        /// <summary>
        /// （合成物品）指定的消耗道具
        /// </summary>
        public const int SYNTHETICS_APPOINTED_ITEM = SYNTHETICS + 3;

        #region PVP相关
        public const int PVP_PVP_SRV_IP = 26011;
        public const int PVP_PVP_SRV_PORT = 26012;
        public const int PVP_SERVER_TOKEN = 26013;
        public const int PVP_RACE = 26003;
        public const int PVP_TYPE = 26002;//  pvp类型
        public const int PVP_BATTLE_RS = 26005;//  战斗结果
        public const int PVP_HEROS = 26006;// List<Integer>
        #endregion

        #region 商城
        public const int SHOP_TAG = 8003;
        public const int EXCHANGE_TYPE = 11003;
        public const int SHOP_ITEM_LIST = 8005;
        public const int SHOP_REFRESH_TIME = 8009;//服务器时间戳
        public const int SHOP_SERVER_NEXT_REFRESH_TIME = 8010;//下次刷新时间

        public const int SHOP_ITEM_ID = 8001;
        public const int SHOP_ITEM_COST = 8006;//商品价格
        public const int SHOP_ITEM_COST_TYPE = 8007;//商品价格类型

        public const int SHOP_ENOUGH_BUY_COUNT = 8012;//限购次数

        public const int SHOP_REFRESH_NEED_GOLD = 8013;
        public const int SHOP_REFRESH_LIMIT_COUNT = 8014;
        public const int SHOP_REFRESH_LIMIT_CURRENT_COUNT = 8015;
        public const int SHOP_BLACK_SHOP_OPEN_STATUS_INFO = 8011;
        #endregion

        #region PVP积分宝箱
        public const int PVP_INTERGRALBOX_NEXT_VALUES = 26014;
        public const int PVP_INTERGRALBOX_NEXT_VALUE = 26015;
        public const int PVP_INTERGRAL_BOX_REWARD = 26010;
        #endregion

        #region battle服务器相关
        public const int BATTLE_PREPARATION_AUTH_RESULT = 301;
        public const int ROOM_STATE = 202;//房间状态
        public const int ROOM_PLAYER_POS = 203;//玩家位置
   
        public const int ROOM_NET_ONE_WAY_DELAY = 207;//房间单边时延
                                                      // public const int ROOM_BATTLE_OldTIME = 204;//已经过去的游戏时间；
        public const int ROOM_CLIENT_GAME_TIME = 206;//客户端游戏时间
        public const int BATTLE_SCENE_APM = 409;//apm
        public const int BATTLE_SCENE_CRYSTAL_POSITION = 410;//据点位置
        public const int BATTLE_SCENE_CRYSTAL_STATE = 411;//据点状态
        public const int BATTLE_PREPARATION_TIME_BEGIN_RESOURCE = 302;//开始计算资源的时间
        public const int BATTLE_SCENE_RESOURCE_NUM = 412;//资源量

        public const int BATTLE_PREPARATION_SELF_HERO_INFO = 311;//己方英雄
        public const int BATTLE_PREPARATION_GROUP_NUM = 310;//组数
        public const int BATTLE_PREPARATION_UNIT_CONFIG_ID = 303;//单位配置id
        public const int BATTLE_PREPARATION_UNIT_MODEL_ID = 304;//单位模型
        public const int BATTLE_PREPARATION_UNIT_HP = 305;//单位血量
        public const int BATTLE_PREPARATION_UNIT_SHIELD_VALUE = 306;//单位护盾值
        public const int BATTLE_PREPARATION_UNIT_POSITION = 307;//单位坐标
        public const int BATTLE_SCENE_POSITION_X = 401;//x坐标
        public const int BATTLE_SCENE_POSITION_Y = 402;//y坐标
        public const int BATTLE_SCENE_POSITION_Z = 403;//z坐标
        public const int BATTLE_PREPARATION_UNIT_ID = 308;//单位Id
        public const int BATTLE_PREPARATION_UNIT_LAST_SKILL_TIME = 309;//上次技能时间
        public const int BATTLE_PREPARATION_PEER_HERO_INFO = 312;//对方英雄
        public const int BATTLE_PREPARATION_SELF_SOLDIER_INFO = 313;//己方单兵
        public const int BATTLE_PREPARATION_PEER_SOLDIER_INFO = 314;//对方单兵
        public const int BATTLE_SCENE_SELF_BASE_HP = 404;//己方基地血量
        public const int BATTLE_SCENE_PEER_BASE_HP = 405;//对方基地血量
        public const int BATTLE_SCENE_SELF_RES_NUM = 406;//己方资源量
        public const int BATTLE_PREPARATION_GLOBAL_SKILL_USE_INFO = 315;//全图技能信息
        public const int BATTLE_PREPARATION_GLOBAL_SKILL_ID = 316;//全图技能Id
        public const int BATTLE_PREPARATION_GLOBAL_SKILL_USE_TIME = 317;//全图技能时间
        public const int BATTLE_SCENE_CRYSTAL_VISUL = 407;//据点视野
        public const int BATTLE_SCENE_CRYSTAL_OWNER = 408;//据点占领者
        public const int BATTLE_PREPARATION_ALL_FIGHTER_DATA = 318;//全部战斗单位数据列表
        public const int PLAYER_NICKNAME = 104;//玩家昵称
        public const int PLAYER_ID_New = 101;
        public const int PLAYER_HEAD_ID_New = 103;//玩家头像
        public const int BATTLE_PREPARATION_RACE = 329;//玩家种族
        public const int BATTLE_PREPARATION_RACE_LEVEL = 330;//玩家种族等级
        public const int BATTLE_PREPARATION_PVP_HIS_SCORE = 331;//玩家pvp历史高分
        public const int BATTLE_PREPARATION_PVP_NORMAL_LEVEL_SCORE = 332;//玩家pvp分数
        public const int BATTLE_PREPARATION_PVP_TOP_LEVEL_SCORE = 333;//玩家巅峰分数
        public const int BATTLE_PREPARATION_HERO_LIST = 334;//英雄
        public const int BATTLE_PREPARATION_UNIT_LEVEL = 319;//英雄等级
        public const int BATTLE_PREPARATION_UNIT_STAR = 320;//英雄星级
        public const int BATTLE_PREPARATION_UNIT_QUALITY = 321;//英雄品质
        public const int BATTLE_PREPARATION_ACTIVE_SKILL = 322;//主动技能
        public const int BATTLE_PREPARATION_PASSIVE_SKILL = 323;//被动技能
        public const int BATTLE_PREPARATION_NORMAL_ATTACK_SKILL = 324;//普通攻击技能
        public const int BATTLE_PREPARATION_EXP = 325;//英雄经验
        public const int BATTLE_PREPARATION_ATTACK_POWER_ADDITION = 326;//攻击强度加成
        public const int BATTLE_PREPARATION_ATTACK_SPEED_ADDITION = 327;//攻击速度加成
        public const int BATTLE_PREPARATION_PROMOTE_LEVEL = 328;//强化等级
        public const int BATTLE_PREPARATION_SIMPLE_UNIT_LIST = 335;//种族普通单位列表
        public const int BATTLE_PREPARATION_LAST_LARGE_SEASON_RANKING = 336;//上一大周期赛季的排名

        public const int ROOM_ROOM_ID = 201;

        public const int BATTLE_SCENE_HP_TO_ADD = 413;//加血量
        public const int BATTLE_SCENE_LEFT_HP = 414;//剩余血量
        public const int BATTLE_SCENE_UNIT_ID_LIST = 415;//单位Id列表
        public const int BATTLE_SCENE_GROUP_NO_AND_TYPE = 416;//组
        public const int BATTLE_SCENE_BORN_POSITION = 417;//出生坐标
        public const int BATTLE_SCENE_DEST_POSITION = 418;//目标坐标
        public const int BATTLE_SCENE_SEARCH_TARGET_SELF_ID = 420;//selfid
        public const int BATTLE_SCENE_SELF_POSITIO = 421;//selfposition
        public const int BATTLE_SCENE_SEARCH_TARGET_BULLET_SPEED = 422;//bulletspeed
        public const int BATTLE_SCENE_TARGET_ID = 423;//targetid
        public const int BATTLE_SCENE_TARGET_POSITION = 424;//targetPosition
        public const int BATTLE_SCENE_SEARCH_TARGET_SOLDIER_TYPE = 425;//soliderType
        public const int BATTLE_SCENE_SEARCH_TARGET_UNIT_TYPE = 426;//unitType
        public const int BATTLE_SCENE_IS_FRIEND = 427;//isFriend
        public const int BATTLE_SCENE_ATTACKER_UNIT_ID = 430;//施法者id
        public const int BATTLE_SCENE_SELF_POSITION = 421;
        public const int BATTLE_SCENE_SKILL_NAME = 428;//技能名字
        public const int BATTLE_SCENE_SKILL_TYPE = 429;//技能类型
        public const int BATTLE_SCENE_UNIT_SPEED = 431;//单位的速度
        public const int BATTLE_SCENE_SHIELD_VALUE_LIST = 432;//护盾列表
        public const int BATTLE_SCENE_HP_LIST = 433;//血量列表
        public const int BATTLE_SCENE_SPEED_LIST = 437;//速度列表
        public const int BATTLE_SCENE_POSITION_LIST = 436;//坐标列表
        public const int BATTLE_SCENE_GLOBAL_SKILL_POINT = 440;//作用坐标
        public const int BATTLE_SCENE_GROUP_NO_AND_TYPE_LIST = 434;//组列表
        public const int BATTLE_SCENE_IS_ONLY_SET_CD = 438;//
        public const int BATTLE_SCENE_GLOBAL_SKILL_HAS_POINT = 439;
        public const int BATTLE_SERVER_FIGHT_ID = 501;//战斗Id
        public const int BATTLE_SERVER_AUTH_CODE = 503;//战斗服验证码
        public const int BATTLE_SERVER_BATTLE_IP_4_CLIENT = 507;//战斗服ip
        public const int BATTLE_SERVER_BATTLE_PORT_4_CLIENT = 508;//战斗服端口
        public const int BATTLE_SERVER_FIGHTER_DATA = 502;//战斗数据
        public const int BATTLE_SERVER_MAP_ID = 511;//战斗地图ID
        public const int GAME_SERVER_AUTH_CODE = 703;//游戏服校验码

        public const int CLIENT_REPONSE_FIGHT_END_DATA_FOR_POS_1 = 601;//1号玩家数据
        public const int CLIENT_REPONSE_FIGHT_END_DATA_REASON = 603;//结束原因
        public const int CLIENT_REPONSE_FIGHT_END_DATA_IS_WINNER = 604;//是否是赢家
        public const int CLIENT_REPONSE_FIGHT_END_DATA_PVP_SCORE_OLD = 605;//早前积分
        public const int CLIENT_REPONSE_FIGHT_END_DATA_PVP_SCORE_NEW = 606;//最新积分
        public const int CLIENT_REPONSE_FIGHT_END_DATA_ENERGY_POINT_ADD = 607;//增加的能量点
        public const int CLIENT_REPONSE_FIGHT_END_DATA_RESOURCE_USED = 608;//使用的资源量
        public const int CLIENT_REPONSE_FIGHT_END_DATA_USED_HEROS = 609;//使用的英雄
        public const int CLIENT_REPONSE_FIGHT_END_DATA_TIME_COST = 610;//耗时

        public const int CLIENT_REPONSE_FIGHT_END_DATA_FOR_POS_2 = 602;//2号玩家数据
        public const int BATTLE_SCENE_UNIT_COUNT = 441;//单位数量
        public const int START_X = 209;//开始x
        public const int START_Y = 210;//开始y
        public const int START_Z = 211;//开始z
        public const int SPACELET_ID = 443;//服务端配置的单位Id
        public const int CAMP = 203;//阵营
        public const int CONFIG_ID = 445;//单位配置id
        public const int UNIT_LIST = 1107;//列表
        public const int UNIT_CLIENT_ID = 1108;//客户端id
        public const int BATTLE_SCENE_MOVE_TARGET_POINT = 419;
        public const int BATTLE_SCENE_CLICK_TYPE = 442;
        #endregion

        #region 新的Battle
        public const int ROOM_UPDATE_SPEED = 226;
        public const int ROOM_CAMP = 203;
        public const int ROOM_BATTLE_START_TIME = 204;//服务器开始的时间
        public const int LOADING_PROGRESS = 105;//加载进度
        public const int BATTLE_SCENE_UNIT_UNIT_LIST = 446;//单位信息
        public const int BATTLE_SCENE_UNIT_ID = 443;//服务器单位id
        public const int BATTLE_SCENE_UNIT_STRONG_HOLD_ID = 448;//据点id
        public const int ROOM_START_OCCUPY_TIME = 216;//开始占领的时间
        public const int RESOURCE = 106;//资源量
        public const int GET_RESOURCE_SPEED = 107;//速度
        public const int BASE_RESOURCE_TIME = 108; //秒  上次计算资源时间
        public const int ROOM_FIGHT_UNIT_INFOS = 220;//场景所有单位数据 包括水晶
        public const int BATTLE_SCENE_UNIT_NOW_HP = 454;//当前Hp
        public const int BATTLE_SCENE_UNIT_CONFIG_ID = 445;//配置id
        public const int BATTLE_SCENE_UNIT_NOW_SHIELD_HP = 455;//当前盾
        public const int ROOM_BATTLE_TIME = 221;//房间战斗时间
        public const int BATTLE_SCENE_BATTLE_SCENE_SKILL_ID = 442;//技能id
        public const int BATTLE_SCENE_UNIT_ATTACK_UNIT_LIST = 449;//发起攻击的单位
        public const int BATTLE_SCENE_UNIT_TARGET_ID = 450;//目标单位id
        public const int ROOM_FIGHT_ID = 219;//此次攻击战斗id

        public const int ROOM_END_X = 212;
        public const int ROOM_END_Y = 213;
        public const int ROOM_END_Z = 214;

        public const int ROOM_BATTLE_SKILL_CDS = 222;
        public const int CLIENT_REPONSE_FIGHT_END_DATA_KILL_HERO_COUNT = 611;
        public const int CLIENT_REPONSE_FIGHT_END_DATA_KILL_SINGLEPAWN_COUNT = 612;
        public const int CLIENT_REPONSE_FIGHT_END_DATA_OCCUPY_STRONG_HOLD_COUNT = 613;
        public const int CLIENT_REPONSE_FIGHT_END_INNER_FIGHT_CAPACITY = 614;//<int>(614) 战斗力
        public const int PVP_SCENE_HEROID = 303;
        public const int HERO_PVP_REWARD_ENERGY_POINT = 2001;
        public const int RACE_TYPE = 19011;
        public const int RACE_LV = 19012;
        public const int RACE_EXP = 19013;//种族经验
        public const int PVP_STAR = 26019;//pvp start
        public const int DELTA_EXP = 19014;//增加的种族经验
        public const int HERO_CHALLENGE_REWARD_EXPECT_ENERGY_POINT = 6006;

        public const int PVP_SAFE_SCORE = 26021;//保护积分
        public const int PVP_SAFE_MAX_SCORE = 26022;//最大保护积分
        public const int PVP_BATTLE_DEGREE_COUNT = 26023;
        public const int PVP_BATTLE_DEGREE_TOTAL_COUNT = 26024;
        public const int BATTLE_PREPARATION_INNER_PVP_TRUE_STAR = 341;
        

        public const int PVP_CANCLE_MATCH_STATE = 26025;//取消状态

        public const int PVP_MATCH_TIME_OUT_REWARD = 26020;//匹配失败奖励


        #endregion

        #region 账号相关
        public const int ACCOUNT_QUDAO = 101;//渠道id
        public const int ACCOUNT_PWD = 104;//密码
        public const int ACCOUNT_REL_UID = 102;//手机号
        public const int ACCOUNT_VERIFY_CODE_TYPE = 115;//验证码类型
        public const int ACCOUNT_VERIFY_CODE = 114;//验证码
        #endregion

        public const int MONTH_CARD = 12000;
        public const int MONTH_CARD_REMAIN_DAY = MONTH_CARD + 1;//月卡剩余天数

        #region 支付
        public const int PAY = 10000;
        public const int PAY_APP_ID = PAY + 1;// 应用id
        public const int PAY_SDK_CODE = PAY + 2;// sdkid
        public const int PAY_PRODUCT_ID = PAY + 3;// 商品id
        public const int PAY_PRODUCT_NUM = PAY + 4;// 商品数量
        public const int PAY_PRICE = PAY + 5;// 价格
        public const int PAY_ORDER_ID = PAY + 6;// 订单id
        public const int PAY_RECEIPT = PAY + 7;// 支付收据
        public const int PAY_CALLBACK_URL = PAY + 8;// 支付回调地址
        public const int PAY_CHANNEL_SDK_ID = PAY + 9;// 渠道sdkid
        public const int PAY_EXT_AWARD = PAY + 10;// 额外奖励
        public const int PAY_CURRENCY = PAY + 11;// 币种
        public const int PAY_360_PAY_CALLBACK_URL = PAY + 12;// 360支付回调地址
        public const int PAY_SDK_ORDER_ID = PAY + 13;// SDK ORDERID
        public const int PAY_SDK_UID = PAY + 14;
        #endregion

        #region 好友
        public const int FRIENDS_LIST = 28004;         //好友列表
        public const int FRIENDS_ONLINE_STATUS = 28007;//好友在线状态。1：在线，2：离线
        public const int FRIENDS_PVP_STAR = 26019;       //种族对应的段位
        public const int FRIENDS_PLAYER_LEVEL = 109;   //好友玩家等级
        public const int FRIENDS_EXT0 = 110;           //玩家PVPDATA数据
        public const int FRIENDS_RECEIVER = 28001;     //消息的接受者
        public const int FRIENDS_SENDER = 28002;       //消息的发送者
        public const int FRIENDS_INVITE_LIST = 28005;  //申请加好友的列表
        public const int FRIENDS_CONFRIM = 28006;      //反馈好友申请。1：同意，2拒绝
        public const int FRIENDS_FRIEND_ID = 28008;    //在线状态的玩家Id
        public const int FRIENDS_SIMPLE_INFO_QUERY_KEYS = 28009;    //好友详情参数
        public const int FRIENDS_CHAT_CONTENT = 28003; //聊天内容
        public const int FRIENDS_SIMPLE_INFO_QUERY_PACK_CONTENT = 28010;//好友详情
        #endregion

        #region 运营活动

        #region 六一活动
        public const int ACTIVITY = 22000;
        /// <summary>
        /// "progress的名字full name" String
        /// </summary>
        public const int ACTIVITY_EX_PROGRESS_NAME = ACTIVITY+22;
        /// <summary>
        /// 进度的值 Map
        /// </summary>
        public const int ACTIVITY_EX_PROGRESS_VALUE = ACTIVITY+23;
        /// <summary>
        /// 兑换消耗  id - 数量
        /// </summary>
        public const int ACTIVITY_EX_COST_ITEMS = ACTIVITY + 24;
        /// <summary>
        /// 奖励物品 List<List<int>>
        /// </summary>
        public const int ACTIVITY_EX_REWARD_ITEMS = ACTIVITY + 25;
        /// <summary>
        /// 奖励物品的转化物品  List<List<int>>
        /// </summary>
        public const int ACTIVITY_EX_CONVERTED_ITEMS = ACTIVITY + 26;
        /// <summary>
        /// 所有的活动数据状态  Map<String, List<Progress>>   string:活动名字   Progress: 表示所有的活动Map<int , Object>
        /// </summary>
        public const int ACTIVITY_EX_PROGRESS_COLLECTION = ACTIVITY + 27;
        /// <summary>
        /// 所有的活动数据状态
        /// </summary>
        public const int ACTIVITY_EX_ITEM_PICK_TYPE = ACTIVITY + 28; // 
        /// <summary>
        /// 活动名字
        /// </summary>
        public const int ACTIVITY_EX_ITEM_ACTIVITY_NAME = ACTIVITY + 29; // 
        /// <summary>
        /// Bundle下标
        /// </summary>
        public const int ACTIVITY_EX_DELIVER_INDEX = ACTIVITY + 30; // 
        /// <summary>
        /// 指向的BundleId;
        /// </summary>
        public const int ACTIVITY_EX_DELIVER_ID = ACTIVITY + 31; // 
        /// <summary>
        /// 所有的活动数据状态
        /// </summary>
        public const int ACTIVITY_EX_ITEM_EXCHANGE_TYPE = ACTIVITY + 32; // 
        /// <summary>
        /// Bundle下标
        /// </summary>
        public const int ACTIVITY_EX_EXCHANGE_INDEX = ACTIVITY + 33; // 
        /// <summary>
        /// 指向的BundleId;
        /// </summary>
        public const int ACTIVITY_EX_EXCHANGE_ID = ACTIVITY + 34; // 
        /// <summary>
        /// 关闭的活动ID list
        /// </summary>
        public const int ACTIVITY_EX_CLOSED_ACTIVITY = ACTIVITY + 35;//
        /// <summary>
        /// openid
        /// </summary>
        public const int ACTIVITY_EX_OPEN_ID = ACTIVITY + 36;                      //openid
        /// <summary>
        /// sdkcode
        /// </summary>
        public const int ACTIVITY_EX_SDK_CODE = ACTIVITY + 37;
        #endregion

        #endregion

        #region 公告
        public const int NOTICE_MSG = 29001;//公告信息
        #endregion

        #region 多人匹配
        public const int PVP_INVITE_INVITEE = 26040;            //被邀请的ID
        public const int PVP_INVITE_ROOM_DATA = 26036;    //房间内玩家信息
        public const int PVP_INVITE_IS_HOST = 26037;            //是否房主
        public const int PVP_INVITE_CAN_INVITE = 26038;      //邀请权限
        public const int PVP_INVITE_INVITER = 26039;            //邀请人       
        public const int PVP_INVITE_AGREE_OR_REFUSE = 26041;// 1接受 0拒绝  
        public const int PVP_INVITE_BE_HOST = 26042;            //要成为房主的人
        public const int PVP_INVITE_BE_RACE = 26043;            //选择的种族
        public const int PVP_TEAM_LIST = 26028;                     //队伍中每个玩家的信息
        public const int PVP_TEAM_ID = 26033;                       //组队房主Playerid
        public const int PVP_PLAYER_NICKNAME = 26030;       //队员rank
        public const int PVP_INNER_FIGHT_CAPACITY = 26031;//队员名称
        public const int PVP_INNER_TRUE_STAR = 26032;         //真是星数
        public const int PVP_INNER_PROTECT_SCORE = 26034;//保护分

        #endregion

    }

    public class RsCode : BaseCodeMap.BaseRsCode
    {
        public const int ERR_CODE_SRV_ERRO = 1;
        public const int ERR_CODE_NO_NORMAL_LOGIN = 3;
        public const int ERRO_CODE_INVALIDE_TOKEN = 4;// 无效的token
        public const int ERRO_CODE_TOKEN_EXPIRED = 5;// token过期
        public const int ERRO_CODE_PARAM = 6; // 错误传参
        public const int ERRO_CODE_FORCE_UPDATE_VERSION = 7; // 强制更新版本
        public const int ERRO_CODE_ITEM_OP_NO_ENOUGH = 4001;//道具数量不足
        public const int ERRO_CODE_INVALID_ITEM_CID = 4002;//无法出售道具
        public const int ERRO_CODE_MAIL_NOT_EXIST = 15001;//邮件不存在
        public const int ERRO_CODE_MAIL_AWARD_NOT_EXIST = 15002;//附件不存在
        public const int ERRO_CODE_MAIL_AWARD_YET = 15003;//附件已领取
        public const int ERRO_CODE_BAG_FULL = 4004;//背包格子不足
        public const int ERRO_CODE_PLAYER_OP_SAVE_DATA_EXCEPTION = 1001;//玩家数据异常
        public const int ERRO_CODE_PLAYER_OP_NAME_EXIST = 1002;//名称已存在
        public const int ERRO_CODE_PLAYER_OP_NAME_INVALID = 1003;//名称非法
        public const int ERRO_CODE_PLAYER_OP_HEAD_INVALID = 1004;//头像id非法
        public const int ERRO_CODE_OPTION_OUT_BOUNDARY = 16001;//设置项数量超长
        #region 任务
        public const int ERRO_CODE_TASK__OP_INVALID_TASK_ID = 2001;//无效的任务
        public const int ERRO_CODE_TASK_OP_NO_ENOUGH_ACTIVE_POINT = 2002;//活跃点不够
        public const int ERRO_CODE_TASK_OP_NO_ACTIVE_POINT_GET_TIMES = 2003;//无活跃点奖励领取次数
        public const int ERRO_CODE_TASK__OP_TASK_REWARD_ALEADY_GET = 2004;//任务奖励已领取
        #endregion
        public const int ERRO_CODE_ACTIVITY_NOT_EXIST = 17002;//无活跃点奖励领取次数
        public const int ERRO_CODE_ACTIVITY_NOT_OPEN = 17004;//任务奖励已领取
        public const int ERRO_CODE_PLAYER_OP_STRENGTH_NOT_ENOUGH = 1005;//体力不足
        public const int ERRO_CODE_ACTIVITY_COUNT_LIMIT = 17001;//活动次数限制

        public const int ERRO_CODE_HERO_CHALLENGE_NOT_ENOUGH_STR = 3001;//体力不够
        public const int ERRO_CODE_HERO_CHALLENGE_NO_CHALLENGE_TIMES = 3002;//挑战次数用完

        //守卫战
        public const int ERRO_CODE_GUARD_BATTLE_NOT_ENOUGH_STRE = 5001;//体力不够
        public const int ERRO_CODE_GUARD_BATTLE_NO_CHALLENGE_TIMES = 5002;//挑战次数用完
        public const int ERRO_CODE_GUARD_BATTLE_INVALIDE_DEGREE = 5003;//错误的难度挑战
        public const int ERRO_CODE_GUARD_BATTLE_CHEATING = 5004;//玩家作弊

        #region 单人征服
        private const int ERRO_CODE_CONQUER = 18000;
        public const int ERRO_CODE_CONQUER_TIER_TOO_HIGH = ERRO_CODE_CONQUER + 1;//征服挑战层数超出范围
        public const int ERRO_CODE_CONQUER_NOT_CHECKED = ERRO_CODE_CONQUER + 2;//尚未通过挑战前的检测
        public const int ERRO_CODE_CONQUER_AWARD_COMPLETELY = ERRO_CODE_CONQUER + 3;//层数奖励已经领取过
        public const int ERRO_CODE_CONQUER_NOT_PASS = ERRO_CODE_CONQUER + 4;//当前层数尚未通过
        #endregion

        #region 英雄
        private const int ERRO_CODE_HEROS = 6000;
        /// <summary>
        /// 物品不足
        /// </summary>
        public const int ERRO_CODE_HERO_ITEM_NO_ENOUGH = ERRO_CODE_HEROS + 1;
        /// <summary>
        /// 等级限制
        /// </summary>
        public const int ERRO_CODE_HERO_LV_LIMIT = ERRO_CODE_HEROS + 2;
        /// <summary>
        /// 英雄不存在
        /// </summary>
        public const int ERRO_CODE_HERO_NO_SPECIFY_HERO = ERRO_CODE_HEROS + 5;
        /// <summary>
        /// 已经激活了该槽位
        /// </summary>
        public const int ERRO_CODE_EVOLUTION_ALREADY_ACTIVATE_POSITION = 27001;
        /// <summary>
        /// 非法槽位
        /// </summary>
        public const int ERRO_CODE_EVOLUTION_INAVLID_POSITION = 27002;
        /// <summary>
        /// 已经升到了顶级
        /// </summary>
        public const int ERRO_CODE_EVOLUTION_REACH_MAX_LEVEL = 27003;
        /// <summary>
        /// 升级槽位没有完全解锁
        /// </summary>
        public const int ERRO_CODE_EVOLUTION_LEVEL_UP_ACTIVATE_POSITION_NOT_ENOUGH = 27004;
        /// <summary>
        /// 皮肤Id无效
        /// </summary>
        public const int ERRO_CODE_HERO_SKIN_ID_INVALID = ERRO_CODE_HEROS + 9;
        /// <summary>
        /// 皮肤Id无效
        /// </summary>
        public const int ERRO_CODE_HERO_SKIN_NOT_MATCH_ROLE = ERRO_CODE_HEROS + 10;
        /// <summary>
        /// 无效的升级
        /// </summary>
        public const int ERRO_CODE_HERO_INVALID_ONE_KEY_LEVEL_UP = ERRO_CODE_HEROS + 11;
        #endregion

        #region 物品合成
        /// <summary>
        /// 道具不能被合成
        /// </summary>
        public const int ERRO_CODE_SYNTHETICS_NOT_AVAILABLE = 22001;
        #endregion

        //PVE
        public const int ERRO_CODE_DUP_OP_RSCODE = 19000;
        public const int ERRO_CODE_DUP_OP_NO_AVAILABLE_COUNT = ERRO_CODE_DUP_OP_RSCODE + 7;//通关剩余次数不足
        public const int ERRO_CODE_DUP_OP_INVALID_STAR_LV = ERRO_CODE_DUP_OP_RSCODE + 2;//关卡星数不足

        //sign
        public const int ERRO_CODE_OP_SIGN_RSCODE = 20000;
        public const int ERRO_CODE_OP_SIGN_REPEAT_SIGN = ERRO_CODE_OP_SIGN_RSCODE + 1;//该日期已经签到过
        public const int ERRO_CODE_OP_SIGN_DAY_INVALID = ERRO_CODE_OP_SIGN_RSCODE + 3;//签到日期不合法
        public const int ERRO_CODE_OP_SIGN_NO_SIGN_TIME = ERRO_CODE_OP_SIGN_RSCODE + 4;//补签次数不足
        public const int ERRO_CODE_OP_SIGN_IN_LESS_DAY = ERRO_CODE_OP_SIGN_RSCODE + 2;//连续登录天数不足
        public const int ERRO_CODE_OP_SIGN_CUMULATE_AWARD_COMPLETE = ERRO_CODE_OP_SIGN_RSCODE + 5;//已经领取过连续登录奖励

        #region 能量宝箱
        private const int ERRO_CODE_ENERGY_BOX = 23000;
        /// <summary>
        /// 宝箱列表已达最大值
        /// </summary>
        public const int ERRO_CODE_ENERGY_BOX_LEN_MAX = ERRO_CODE_ENERGY_BOX + 0;
        /// <summary>
        /// 能量id错误
        /// </summary>
        public const int ERRO_CODE_ENERGY_BOX_ENERGY_ID_ERROR = ERRO_CODE_ENERGY_BOX + 5;
        /// <summary>
        ///  能量不足
        /// </summary>
        public const int ERRO_CODE_ENERGY_BOX_ENERGY_NOT_ENOUGH = ERRO_CODE_ENERGY_BOX + 2;
        /// <summary>
        /// 宝箱不存在
        /// </summary>
        public const int ERRO_CODE_ENERGY_BOX_NOT_EXIST = ERRO_CODE_ENERGY_BOX + 3;
        /// <summary>
        /// 宝箱正在冷却
        /// </summary>
        public const int ERRO_CODE_ENERGY_BOX_CALMING_DOWN = ERRO_CODE_ENERGY_BOX + 4;
        #endregion

        #region 商城
        public const int ERRO_CODE_SHOP_CITY_HAVE_NO_SHOP = 24003;
        public const int ERRO_CODE_SHOP_CITY_HAVE_NO_ITEM_ID = 24002;
        public const int ERRO_CODE_ERRO_CODE_EXCHANGE_KEY_NOT_EXISTS = 28001;// (28001)//兑换类型错误
        public const int ERRO_CODE_SHOP_CITY_NO_BUY_COUNT = 24005;
        public const int ERRO_CODE_SHOP_CITY_MONEY_NOT_ENOUGH = 24004;
        public const int ERRO_CODE_SHOP_CITY_CAN_NOT_REFRESH = 24001;
        public const int ERRO_CODE_SHOP_CITY_MONEY_NOT_MATCH = 24006;
        public const int ERRO_CODE_SHOP_CITY_LIMIT_REFRESH = 24010;
        #endregion

        #region 购买体力
        public const int ERRO_CODE_STRENGTH_BUY_MAX = 25001;
        #endregion

        #region 帐号操作
        public const int ERRO_ACCOUNT_OP = 100;
        public const int ERRO_ACCOUNT_OP_INVALID_REL_CODE = ERRO_ACCOUNT_OP + 1;//无效的第三方关联code
        public const int ERRO_ACCOUNT_OP_INVALID_SRV_ID = ERRO_ACCOUNT_OP + 2;//无效的服务器id
        public const int ERRO_ACCOUNT_OP_SRV_ID_IS_NULL = ERRO_ACCOUNT_OP + 3;//srvid为空
        public const int ERRO_ACCOUNT_OP_PWD = ERRO_ACCOUNT_OP + 4;//密码错误
        public const int ERRO_ACCOUNT_OP_NO_EXIST_ACCOUNT = ERRO_ACCOUNT_OP + 5;//账号不存在
        public const int ERRO_ACCOUNT_OP_RELCODE_ALREADY_BINDED = ERRO_ACCOUNT_OP + 6;//三方账号已绑定
        public const int ERRO_ACCOUNT_OP_THIS_UID_ALREAD_BINDED_AND_REL_CODE_NOT_REL_UID = ERRO_ACCOUNT_OP + 7;//当前账号已关联其他三方账号并且该三方账号未关联任何账号
        public const int ERRO_ACCOUNT_OP_BIND_SUCCESS = ERRO_ACCOUNT_OP + 8;//绑定成功
        #endregion

        #region 竞技积分宝箱
        public const int ERRO_CODE_PVP_NO_REWARD = 26001;
        #endregion

        // 踢下线
        public const int ERRO_CODE_KICK_OFF = 12000;
        public const int ERRO_CODE_KICK_OFF_ACCOUNT_LOGIN_ELSEWHERE = ERRO_CODE_KICK_OFF + 1;// 玩家在别处登录
        public const int ERRO_CODE_KICK_OFF_VERSION_FORCE = ERRO_CODE_KICK_OFF + 2;// 版本强制更新
        public const int ERRO_CODE_KICK_OFF_SRV_MAINTAIN = ERRO_CODE_KICK_OFF + 3;// 服务器维护

        //
        public const int PLAYER_DATA_OUT_OF_DATE = 1;
        public const int AUTH_FAILED = 2;
        public const int NO_VALID_BATTLE_SERVER = 3;

        #region 账号相关
        public const int ACCOUNT_ERR_1 = 111;//验证码已发送
        public const int ACCOUNT_ERR_2 = 112;//验证码失效，请重新获取验证码
        public const int ACCOUNT_ERR_3 = 113;//验证码错误，重新输入
        public const int ACCOUNT_ERR_4 = 114;//帐号已注册
        public const int ACCOUNT_ERR_5 = 115;//不支持修改密码
        public const int ACCOUNT_ERR_6 = 116;//手机号未注册
        public const int ACCOUNT_ERR_7 = 117;//验证码发送频繁
        #endregion
        #region 支付模块
        public const int ERRO_PAY = 7000;
        public const int ERRO_PAY_INVALIDE_PRODUCT = ERRO_PAY + 1;// 无效的商品
        public const int ERRO_PAY_INVALIDE_ORDER_ID = ERRO_PAY + 2;// 无效的订单id
        public const int ERRO_PAY_INVALIDE_RECEIPT = ERRO_PAY + 3;// 无效的支付票据
        public const int ERRO_PAY_TIME_OUT = ERRO_PAY + 4;// 购买超时
        public const int ERRO_PAY_FINISHED = ERRO_PAY + 5;// 已购买完成
        public const int ERRO_PAY_CAN_NOT_REPEAT_BUY_NEW_PLAYER_PACKAGE_PRODUCT = ERRO_PAY + 6;// 不可重复购买新手礼包
        public const int ERRO_PAY_INVALIDE_SDK_ID = ERRO_PAY + 7;// 无效的skd渠道id
        public const int ERRO_PAY_ORDER_PORCESSING = ERRO_PAY + 8;// 订单处理中
        #endregion
        #region 月卡
        public const int ERRO_CODE_MONTH_CARD = 9000;
        public const int ERRO_CODE_MONTH_CARD_EXPIRED = ERRO_CODE_MONTH_CARD + 1;// 月卡过期
        public const int ERRO_CODE_MONTH_CARD_INVALIDE_BUY_NUM = ERRO_CODE_MONTH_CARD + 2;// 月卡无效购买次数
        public const int ERRO_CODE_MONTH_CARD_AREADY_BUY = ERRO_CODE_MONTH_CARD + 3;// 月卡已购买
        public const int ERRO_CODE_MONTH_CARD_REWARD_AREADY_GET_TODAY = ERRO_CODE_MONTH_CARD + 4;// 月卡今日已领取
        #endregion
        #region
        public const int ERRO_CODE_ACTIVITY_AWARD_NOT_AVAILABLE = 17005;//奖励不可用
        #endregion

        #region
        public const int ERRO_FRIENDS_CAN_NOT_MANIPULATE_YOUR_SELF = 28001;     //不能删除自己
        public const int ERRO_FRIENDS_NOT_YOUR_FRIEND = 28008;                  //不是好友
        public const int ERRO_FRIENDS_IN_COOL_DOWN_TIME = 28003;                //同一玩家连续操作时间限制
        public const int ERRO_FRIENDS_SENDER_EXCEED_MAX_FRIEND_COUNT = 28004;   //自己的好友数量已满
        public const int ERRO_FRIENDS_RECEIVER_EXCEED_MAX_FRIEND_COUNT = 28005; //对方的好友数量已满
        public const int ERRO_FRIENDS_ALREADY_YOUR_FRIEND = 28007;              //玩家已经是你的好友
        public const int ERRO_FRIENDS_INVALID_PID = 28009;                      //不存在此玩家的ID
        #endregion
        public const int ERRO_HERO_ALREADY_EXISTS = 6003;                      //已拥有此英雄
        public const int ERRO_HERO_SKIN_ALREADY_EXISTS = 6007;                 //已拥有此皮肤


    }
}
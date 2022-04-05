using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace LitFramework.Persistent
{
    public class AccountLocalData : LocalDataBase
    {
        public long CreateAccountTime = 0L;

        [Header("最近本地化时间(毫秒)")]
        //[SerializeField]
        public long localSaveTime;

        [Header("体力")]
        public int PhysicalPoint = 10;
        [Header("零钱")]
        public int Gold = 100;
        [Header("威望")]
        public int Prestige = 0;

        /// <summary>
        /// 当前玩家拥有的卡组信息 卡牌ID-等级-数量
        /// </summary>
        [Header("当前玩家拥有的卡牌实体信息 卡牌ID-等级")]
        public List<CardLocalData> AllCardsGotList = new List<CardLocalData>();
        [Header("当前战斗关卡数（天梯当前进度）")]
        public int CurrentBattleLevel = 1000;

        [Header("当前存钱罐积攒的金币")]
        public int RestoredCoin = 0;

        [Header("零钱罐当前等级")]
        public int RestoredLevel = 1;

    }
}


[System.Serializable]
[StructLayout(LayoutKind.Auto)]
public struct CardLocalData
{
    public int cardID;
    public int cardLevel;
    public bool isSelect;
}

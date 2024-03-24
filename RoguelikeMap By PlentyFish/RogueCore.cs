using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PFRoguelike;

namespace PFRoguelike
{
[Serializable]
public class RogueCore : Singleton<RogueCore>
{
    #region 核心基本参数
    //核心带有的地图
    public RogueMap map;
    //核心种子
    public static int seed;
    //核心种子最大值 种子为8位 1亿
    public int seedRange = 100000000;//1,0000,0000
    //public static string seedRoomName = "Room";
    //种子词典 已过时 基本实现原理
    //public Dictionary<SeedType, AbstractSeed> seedDictionary = new Dictionary<SeedType, AbstractSeed>();
    //System式Random
    //种子的种子
    public System.Random parentRandom;
    //房间种子
    public System.Random roomRandom;
    private static int randomValue;
    #endregion

    #region 基本函数
    static RogueCore()
    {
    }
    public override void Initialize()
    {
        base.Initialize();
            Debug.Log("核心成功初始化");
    }
    #endregion

    #region 种子互动
    /// <summary>
    /// 设置种子
    /// </summary>
    /// <param name="sed"></param>
    public void SetSeed(int sed)
    {
        seed = sed;
        Debug.Log("种子设置为" + seed);
        //生成和设置种子
        parentRandom = new System.Random(seed);
        roomRandom = new System.Random(parentRandom.Next(99999999));

        //Unity式实现
        //UnityEngine.Random.InitState(seed);
        //seedDictionary.Add(SeedType.Room, new AbstractSeed(Get8BitNumber(), Get8BitNumber(), Get8BitNumber()));//生成3个随机的8位数字，并将其添加到词典中
        //Debug.Log("种类文本" + SeedType.Room.ToString());
    }
    /// <summary>
    /// 使用指定种子序列，获得范围内随机小数值
    /// </summary>
    /// <param name="random"></param>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    public static float GetRandomFloat(System.Random random ,float minValue = 0,float maxValue = 1)
    {
        return (float)(random.NextDouble() * (maxValue - minValue)  + minValue);
    }
    /// <summary>
    /// 使用指定种子序列，获得范围内随机整数值
    /// </summary>
    /// <param name="random"></param>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    public static int GetRandomInt(System.Random random,int minValue = 0,int maxValue = 1)
    {
        //多走一道方便修改
        return random.Next(minValue, maxValue);
    }
    /// <summary>
    /// 获得随机8位房间码
    /// </summary>
    /// <returns></returns>
    public static int GetRandomRoomCode()
    {
        return GetRandomInt(RogueCore.Instance.roomRandom, 10000000, 99999999);
    }
    #endregion

    #region Unity式实现
    //public enum SeedType
    //{
    //    Room, Item
    //}
    //public int GetRandomSeed(SeedType type)
    //{
    //    if (!seedDictionary.ContainsKey(type))
    //    {
    //        //理论上可以重新生成，但是没必要，防止乱序
    //        Debug.LogError("错误代码-50001:尝试获取一个未初始化的种子序列，请检查生成函数！！！");
    //        return 0;
    //    }
    //    return seedDictionary[type].GetSeedResult();
    //}
    //public float GetRandomPercentage(SeedType type)
    //{
    //    return seedDictionary[type].GetSeedResult();
    //}
    public int Get8BitNumber()
    {
        return UnityEngine.Random.Range(10000000, 99999999);
    }
    public int Get6BitNumber()
    {
        return UnityEngine.Random.Range(100000, 999999);
    }
    #endregion
}
}

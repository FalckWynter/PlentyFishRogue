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
    #region ���Ļ�������
    //���Ĵ��еĵ�ͼ
    public RogueMap map;
    //��������
    public static int seed;
    //�����������ֵ ����Ϊ8λ 1��
    public int seedRange = 100000000;//1,0000,0000
    //public static string seedRoomName = "Room";
    //���Ӵʵ� �ѹ�ʱ ����ʵ��ԭ��
    //public Dictionary<SeedType, AbstractSeed> seedDictionary = new Dictionary<SeedType, AbstractSeed>();
    //SystemʽRandom
    //���ӵ�����
    public System.Random parentRandom;
    //��������
    public System.Random roomRandom;
    private static int randomValue;
    #endregion

    #region ��������
    static RogueCore()
    {
    }
    public override void Initialize()
    {
        base.Initialize();
            Debug.Log("���ĳɹ���ʼ��");
    }
    #endregion

    #region ���ӻ���
    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="sed"></param>
    public void SetSeed(int sed)
    {
        seed = sed;
        Debug.Log("��������Ϊ" + seed);
        //���ɺ���������
        parentRandom = new System.Random(seed);
        roomRandom = new System.Random(parentRandom.Next(99999999));

        //Unityʽʵ��
        //UnityEngine.Random.InitState(seed);
        //seedDictionary.Add(SeedType.Room, new AbstractSeed(Get8BitNumber(), Get8BitNumber(), Get8BitNumber()));//����3�������8λ���֣���������ӵ��ʵ���
        //Debug.Log("�����ı�" + SeedType.Room.ToString());
    }
    /// <summary>
    /// ʹ��ָ���������У���÷�Χ�����С��ֵ
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
    /// ʹ��ָ���������У���÷�Χ���������ֵ
    /// </summary>
    /// <param name="random"></param>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    public static int GetRandomInt(System.Random random,int minValue = 0,int maxValue = 1)
    {
        //����һ�������޸�
        return random.Next(minValue, maxValue);
    }
    /// <summary>
    /// ������8λ������
    /// </summary>
    /// <returns></returns>
    public static int GetRandomRoomCode()
    {
        return GetRandomInt(RogueCore.Instance.roomRandom, 10000000, 99999999);
    }
    #endregion

    #region Unityʽʵ��
    //public enum SeedType
    //{
    //    Room, Item
    //}
    //public int GetRandomSeed(SeedType type)
    //{
    //    if (!seedDictionary.ContainsKey(type))
    //    {
    //        //�����Ͽ����������ɣ�����û��Ҫ����ֹ����
    //        Debug.LogError("�������-50001:���Ի�ȡһ��δ��ʼ�����������У��������ɺ���������");
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

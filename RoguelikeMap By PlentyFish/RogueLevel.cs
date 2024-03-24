using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PFRoguelike;

namespace PFRoguelike
{
[Serializable]
public class RogueLevel 
{
    #region 楼层参数
    //楼层节点数组 * 不用List以方便确认位置关系
    /// <summary>
    /// 【注意】 这个变量只能通过函数修改，避免房间码丢失
    /// </summary>
    public RogueNode[] levelNodes;
    //楼层高度
    public int y;
    //楼层是否不会被随机删除
    public bool NotRemove = false;
    #endregion

    #region 基本函数
    public RogueLevel()
    {

    }
    /// <summary>
    /// 构造函数，包括最大节点数、楼层高度、是否可删除
    /// </summary>
    /// <param name="nodeCounts"></param>
    /// <param name="high"></param>
    /// <param name="remove"></param>
    public RogueLevel(int nodeCounts,int high = 0, bool remove = false)
    {
        y = high;
        NotRemove = remove;
        //初始化节点数组
        levelNodes = new RogueNode[nodeCounts];
        for(int i = 0; i< levelNodes.Length; i++)
        {
            //为数组填充空节点
            levelNodes[i] = new NodeMonster() ;
            //获取随机房间码
            levelNodes[i].nodeSeedCode = RogueCore.GetRandomRoomCode();
            //Debug.Log("坐标" + i + "," + y + "房间种子" + levelNodes[i].nodeSeedCode);
            //设置节点坐标
            levelNodes[i].SetPosition(new Vector2(i, y));
        }
    }
    /// <summary>
    /// 设置楼层高度
    /// </summary>
    /// <param name="pos"></param>
    public void SetPosition(int pos)
    {
        y = pos;
    }
    #endregion

    #region 取参函数
    /// <summary>
    /// 获取最左边节点的位置
    /// </summary>
    /// <returns></returns>
    public int GetLeftPlace()
    {
        return (int)GetLeftNode().position.x;
    }
    /// <summary>
    /// 获取最右边节点的位置
    /// </summary>
    /// <returns></returns>
    public int GetRightPlace()
    {
        return (int)GetRightNode().position.x;
    }
    /// <summary>
    /// 获取左节点脚本
    /// </summary>
    /// <returns></returns>
    public RogueNode GetLeftNode()
    {
        for(int i = 0; i< levelNodes.Length;i++)
        {
            if (levelNodes[i] != null)
                return levelNodes[i];
        }
        return null;
    }
    /// <summary>
    /// 获取右节点位置
    /// </summary>
    /// <returns></returns>
    public RogueNode GetRightNode()
    {
        for(int i = levelNodes.Length - 1;i >= 0 ; i--)
        {
            if(levelNodes[i] != null)
            {
                return levelNodes[i];
            }
        }
        return null;
    }
    /// <summary>
    /// 获取所有节点的横坐标
    /// </summary>
    /// <returns></returns>
    public List<int> GetNodesPositionX()
    {
        List<int> temp = new List<int>();
        for(int i = 0; i<levelNodes.Length; i++)
        {
            if (levelNodes[i] != null)
                temp.Add(i);
        }
        return temp;
    }
    /// <summary>
    /// 获取节点的数量
    /// </summary>
    /// <returns></returns>
    public int GetNodesCount()
    {
        int count = 0;
        for(int i = 0; i < levelNodes.Length;i++)
        {
            if (levelNodes[i] != null)
                count++;
        }
        return count;
    }
    #endregion

    #region 改参函数
    /// <summary>
    /// 修改楼层中的坐标 *【注意】 由于有房间码的存在，替换房间必须使用此函数
    /// </summary>
    /// <param name="place"></param>
    /// <param name="node"></param>
    public void SetNode(int place,RogueNode node)
    {
        //RogueNode temp = levelNodes[place];
        //修改房间坐标
        node.position = levelNodes[place].position;
        //传递房间种子码
        node.nodeSeedCode = levelNodes[place].nodeSeedCode;
        //替换房间
        levelNodes[place] = node;
    }
    #endregion
}
}
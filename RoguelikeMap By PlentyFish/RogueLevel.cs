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
    #region ¥�����
    //¥��ڵ����� * ����List�Է���ȷ��λ�ù�ϵ
    /// <summary>
    /// ��ע�⡿ �������ֻ��ͨ�������޸ģ����ⷿ���붪ʧ
    /// </summary>
    public RogueNode[] levelNodes;
    //¥��߶�
    public int y;
    //¥���Ƿ񲻻ᱻ���ɾ��
    public bool NotRemove = false;
    #endregion

    #region ��������
    public RogueLevel()
    {

    }
    /// <summary>
    /// ���캯�����������ڵ�����¥��߶ȡ��Ƿ��ɾ��
    /// </summary>
    /// <param name="nodeCounts"></param>
    /// <param name="high"></param>
    /// <param name="remove"></param>
    public RogueLevel(int nodeCounts,int high = 0, bool remove = false)
    {
        y = high;
        NotRemove = remove;
        //��ʼ���ڵ�����
        levelNodes = new RogueNode[nodeCounts];
        for(int i = 0; i< levelNodes.Length; i++)
        {
            //Ϊ�������սڵ�
            levelNodes[i] = new NodeMonster() ;
            //��ȡ���������
            levelNodes[i].nodeSeedCode = RogueCore.GetRandomRoomCode();
            //Debug.Log("����" + i + "," + y + "��������" + levelNodes[i].nodeSeedCode);
            //���ýڵ�����
            levelNodes[i].SetPosition(new Vector2(i, y));
        }
    }
    /// <summary>
    /// ����¥��߶�
    /// </summary>
    /// <param name="pos"></param>
    public void SetPosition(int pos)
    {
        y = pos;
    }
    #endregion

    #region ȡ�κ���
    /// <summary>
    /// ��ȡ����߽ڵ��λ��
    /// </summary>
    /// <returns></returns>
    public int GetLeftPlace()
    {
        return (int)GetLeftNode().position.x;
    }
    /// <summary>
    /// ��ȡ���ұ߽ڵ��λ��
    /// </summary>
    /// <returns></returns>
    public int GetRightPlace()
    {
        return (int)GetRightNode().position.x;
    }
    /// <summary>
    /// ��ȡ��ڵ�ű�
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
    /// ��ȡ�ҽڵ�λ��
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
    /// ��ȡ���нڵ�ĺ�����
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
    /// ��ȡ�ڵ������
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

    #region �Ĳκ���
    /// <summary>
    /// �޸�¥���е����� *��ע�⡿ �����з�����Ĵ��ڣ��滻�������ʹ�ô˺���
    /// </summary>
    /// <param name="place"></param>
    /// <param name="node"></param>
    public void SetNode(int place,RogueNode node)
    {
        //RogueNode temp = levelNodes[place];
        //�޸ķ�������
        node.position = levelNodes[place].position;
        //���ݷ���������
        node.nodeSeedCode = levelNodes[place].nodeSeedCode;
        //�滻����
        levelNodes[place] = node;
    }
    #endregion
}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PFRoguelike;

namespace PFRoguelike
{
    [Serializable]
public class RogueNode
{
    #region �ڵ����
    //�ڵ�����
    public string label;
    //�ڵ�λ��
    public Vector2 position = new Vector2();
    //������
    public int nodeSeedCode = 10000000;

    //�Ƿ��ѱ������ڵ�����
    public bool isConnected = false;
    //���ӵ��Ľڵ�λ��
    public List<Vector2> connectNodeList = new List<Vector2>();
    //�Ƿ����ӵ��� �� �ҽڵ�
    public bool connectLeft = false, connectMid = false, connectRight = false;

    //�ڵ�ͼƬ
    public Sprite sprite;
    //�ڵ�ͼƬ·�� Resources��ʽ
    public string spritePath = "Images/Element/";

    #endregion

    #region ��������
    public RogueNode()
    {
        label = "NotNull";
        Initialize();
    }
    public virtual void Initialize()
    {
        //��ʼ��ʱ����ͼƬ
        sprite = Resources.Load<Sprite>(spritePath + label);
    }
    #endregion

    #region ��������
    /// <summary>
    /// ���ýڵ�λ��
    /// </summary>
    /// <param name="pos"></param>
    public void SetPosition(Vector2 pos)
    {
        position = pos;
    }
    /// <summary>
    /// ������ӵ��Ľڵ�
    /// </summary>
    /// <param name="target"></param>
    public void AddConnectNode(Vector2 target)
    {
        //��������ӵ�������
        if (connectNodeList.Contains(target))
            return;
        //���ڵ���ӵ��б�
        connectNodeList.Add(target);
        //����Ŀ��ڵ�λ����������״̬
        if (target.x > position.x)
            connectRight = true;
        else if (target.x < position.x)
            connectLeft = true;
        else
            connectMid = true;
        //����Ŀ��ڵ�Ϊ�ѱ��ڵ�����
        RogueCore.Instance.map.levelData[(int)target.y].levelNodes[(int)target.x].isConnected = true;
    }
    #endregion

    #region �汾ע��
    /*
     * ��˾���� : Զ��������
     * 

    */
    #endregion
}
}

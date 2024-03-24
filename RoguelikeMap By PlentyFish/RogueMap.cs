using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PFRoguelike;

namespace PFRoguelike
{
[Serializable]
public class RogueMap
{
    #region ��ͼ��������
    //*ע ���ڵ���ԭ�� ��һ��ű�������������ĵ��õĸĲΣ��д��Ż�
    //��ͼ���
    public int mapWidth =4,mapHeight = 8;
    //���ɵ�·�Ŀ����� Խ���·Խ�� ԽС��·Խƫ���м� * ԭ����鿴˵���ĵ�
    public float addOffsetEdgePossible = 0.25f;

    //Ҫɾ���ķ�������
    public int deleteRoomCount = 2;//���ֹ���ᱨ�� û������
    //ÿ�����ٽڵ���
    public int leastNodeCountPreLevel = 2;

    //�㼶����
    public List<RogueLevel> levelData = new List<RogueLevel>();
    //�������ɵĲ�������
    public List<int> freeLevelList = new List<int>();

    //���ⷿ����б�
    public Dictionary<string, List<RogueNode>> specialNodeDic = new Dictionary<string, List<RogueNode>>();
    //������ - ���䷿ ���˵��������
    public int chestNodeCountMax = 4, chestNodeCountNow = 0, shopNodeCountMax = 4, shopNodeCountNow = 0;//Ĭ������Ϊһ�е�����
    //ȫΪ�������Ĳ���
    public int rewardLevelCount = 2;//Ĭ��Ϊ2 ��Ҫ������Э��
    //�������������
    public string rewardPoolName = "Reward";

    #endregion

    #region ��ͼ��ʼ��(��ʵ������)
    public RogueMap()
    {
        Initialize();
    }
    public void Initialize()
    {

    }
    #endregion

    #region ��ͼ���ܺ���

    #region �������ɵ�ͼ(������)
    /// <summary>
    /// ���õ�ͼ���ݣ����������µĵ�ͼ��������ִ�й��ܺ���
    /// </summary>
    public void ResetMap()
    {
        //���ò���
        ResetParameter();
        //����ǰ�ò���
        SetPreParameter();
        //���÷�����б�
        ResetNodePool();

        //���ģ����Ը��������Լ�д����
        //����ȫ�սڵ�
        CreateMap();
        //����BOSS�ڵ�
        SetBossNode();
        //������Ϣ��ڵ�
        SetRestNode();
        //���ɹ̶���������
        SetRewardNode();

        //ɾ������ڵ�
        DeleteRandomNode();

        //���ɱ�·
        SpawnEdge();
        //������·
        SpawnMainEdge();
    }
    #endregion

    #region ��������
    /// <summary>
    /// ���ü�������¥��洢
    /// </summary>
    public void ResetParameter()
    {
        //���ý����������������¥������
        chestNodeCountNow = 0;
        shopNodeCountNow = 0;
        levelData = new List<RogueLevel>();
        freeLevelList = new List<int>();
        specialNodeDic.Clear();
    }
    /// <summary>
    /// ��������ڵ���б�
    /// </summary>
    public void ResetNodePool()
    {
        //��ӽ�����
        List<RogueNode> tempNodeList = new List<RogueNode>();
        for(int i = 0;i<chestNodeCountMax;i++)
        {
            tempNodeList.Add(new NodeChest());
        }
        for (int i = 0; i < shopNodeCountMax; i++)
        {
            tempNodeList.Add(new NodeShop());
        }
        specialNodeDic.Add(rewardPoolName, tempNodeList);
        Debug.Log("�ʵ�����" + specialNodeDic.Count + "�ڵ���" +  specialNodeDic[rewardPoolName].Count);
    }
    /// <summary>
    /// ���ò������ͼ�͹������йصĲ���
    /// </summary>
    public void SetPreParameter()
    {
        //�������㷿��������Ϊ���ͼ���һ�� * ע ������ÿ����������2��mapWidth������ 
        chestNodeCountMax = mapWidth;
        shopNodeCountMax = mapWidth;
    }
    #endregion

    #region ��ͼ������̶�����
    /// <summary>
    /// �����յ�ͼ�����Ĭ�Ϸ���;
    /// </summary>
    public void CreateMap()
    {
        //��ӵ�ͼ�߶Ȳ�����¥����
        for (int i = 0; i < mapHeight; i++)
        {
            //temp.SetPosition(i);
            //��¥������������²� *ע�� ¥�������շ���ĺ�����Level���Դ�
            levelData.Add(new RogueLevel(mapWidth, i));
            //��¥����ӵ������������б�
            freeLevelList.Add(i);
        }
    }
    /// <summary>
    /// ����BOSS��(Ĭ��Ϊ��ͼ��߲�)
    /// </summary>
    public void SetBossNode()
    {
        //BOSSλ�ڵ�ͼ��߲�
        int bossLevel = levelData.Count - 1;
        //��BOSS����������ɱ��Ƴ�
        freeLevelList.Remove(bossLevel);
        //����BOSS��Ϊ����ɾ��Ӱ��
        levelData[bossLevel].NotRemove = true;
        //����BOSS��ڵ㣬ֻ����һ���ڵ���n/2���������
        for (int i = 0; i < levelData[bossLevel].levelNodes.Length;i++)
        {
            if (i != levelData[bossLevel].levelNodes.Length / 2)
                levelData[bossLevel].levelNodes[i] = null;
            else
                //��λ��n/2�Ľڵ���ΪBOSS�� ����Ϊż��ʱ����ƫ��
                levelData[bossLevel].SetNode(i,new NodeBoss()) /*levelNodes[i] = new NodeBoss()*/;
        }
    }
    /// <summary>
    /// ������Ϣ�㣬Ĭ��Ϊ��BOSS��һ��
    /// </summary>
    public void SetRestNode()
    {
        //��Ϣ��λ�ڵ�ͼBOSS���·�
        int restLevel = levelData.Count - 2;
        //����Ϣ����������ɱ��Ƴ�
        freeLevelList.Remove(restLevel);
        //����Ϣ����Ϊ����ɾ��Ӱ��
        levelData[restLevel].NotRemove = true;
        //������ ���ڼ���Ƿ��Ѿ�������� �ڽ�������ɷ�Χʱһ����ȫ������
        bool seted = false;
        //������Ϣ��ڵ�
        for (int i = 0; i < levelData[restLevel].levelNodes.Length; i++)
        {
            //������겻Ϊ�������ҵķ��� ����
            if (i != levelData[restLevel].levelNodes.Length / 2 && i != (levelData[levelData.Count - 1].levelNodes.Length / 2 - 1))
            {
                levelData[restLevel].levelNodes[i] = null;
                continue;
            }
            //��һ��if����������ɷ�Χ�� �����Ѿ������� ��ֱ������
            if (seted)
                continue;

             //Debug.Log("һ���������" + RogouCore.GetRandomFloat(RogouCore.Instance.roomRandom));
             //������ ������� һ���Ǳ�������һ�������� �������
            if (RogueCore.GetRandomFloat(RogueCore.Instance.roomRandom) < 0.5f)
            {
                levelData[restLevel].SetNode(i, new NodeChest());
                levelData[restLevel].SetNode(i + 1, new NodeShop());
            }
            else
            {
                levelData[restLevel].SetNode(i + 1, new NodeChest());
                levelData[restLevel].SetNode(i, new NodeShop());
            }
            //��������Bool��Ϊ��
            seted = true;
            
        }
        
    }
    /// <summary>
    /// ���ý����� ������������ȡ������λ�õ�ĸ������ʵ�����ú���Ϊ���ΰ汾
    /// </summary>
    public void SetRewardNode()
    {
        //�����ｫ ����BOSS ��Ϣ������� ��ϣ�����ֵĲ�ĸ߶� �Ƴ� 
        //�˴��Ƴ�����Ϣ�����һ�������
        freeLevelList.Remove(mapHeight - 3);
        freeLevelList.Remove(0);
        //�������� ��Ϊֻ����2�������� �����жϵڶ����һ���Ƿ��ص����� *ʵ����һ���̶���͹��� 
        int rewardPlace = 0, firstPlace = 0;
        //��ȡ���ɲ��б�
        List<int> tempList = freeLevelList;

        //�����͵��Լ�����
        int tim = 0;
        //������ɲ�����
        foreach (int i in freeLevelList)
        {
            tim++;
            //Debug.Log("�����б����λ��" + i + "����" + tim);
        }

        //���ɽ����������Ĳ���
        for (int i = 0; i < rewardLevelCount; i++)
        {
            //Ҫ��ȡ�Ĳ���ID
            int randomIndex = RogueCore.GetRandomInt(RogueCore.Instance.roomRandom,0, tempList.Count);
            //��ȡ��������ɲ���
            rewardPlace = tempList[randomIndex];
            //�����ķ��� ��Ϊֻ��2�� * ���ʱ��ʵ�ַ���Ϊ�������ɵĽ�����ʹ��һ��List����ټ���Ƿ���List�о����ϵ������һ������½���ֻ��һ��������
            if (i == 0)
                //����ǵ�һ������ ������Ϊ������
                firstPlace = rewardPlace;
            else
            {
                //������������������
                if (Math.Abs(rewardPlace - firstPlace) == 1)
                    //�½�����λ��Ϊ : ����²���ھɲ� ���²�����һ��(�ڰ�����ʱ���ٳ��Դ���)�� ����²���ھɲ㣬������һ��
                    rewardPlace = rewardPlace > firstPlace ? tempList[(randomIndex + 1) % tempList.Count] : tempList[(randomIndex + tempList.Count - 1) % tempList.Count];
            }
            //Debug.Log("�����б�����ѡ��" + rewardPlace + "");
            //���������б����Ƴ�����
            tempList.Remove(rewardPlace);
            //���ø�λ��Ϊ�����㣬���ɽڵ�
            SetRewardNode(rewardPlace);
        }
    }
    /// <summary>
    /// ���ý������ʵ�ʺ�������Ŀ���Ľڵ��滻Ϊ������
    /// </summary>
    /// <param name="levelPlace">Ҫ���õĲ���</param>
    public void SetRewardNode(int levelPlace)
    {
        //���ò���Ϊ����ɾ��Ӱ��
        levelData[levelPlace].NotRemove = true;
        for(int i = 0; i < mapWidth;i++)
        {
            //�ӽ����������г�һ������ * ���԰ѷ���ĳ�List����List�г�һ������
            RogueNode node = DrawRandomNodeFromPool(rewardPoolName);/*GetFixedRewardNode()*/;
            //Debug.Log("�����Ϊ" + node.label + "λ��" + i);
            //��ָ��λ�õĽڵ���Ϊ�鵽�ķ���
            levelData[levelPlace].SetNode(i, node);
        }
    }
    #endregion

    #region ��ͼ�ڵ�ɾ��
    /// <summary>
    /// ɾ����������������ڵ㣬ֻ�����ڵ�ͼ����ʱ����
    /// </summary>
    public void DeleteRandomNode()
    {
        //*ע ��Ϊ������������ֻ������ʱ�ã����Բ����������
        int counter = 0;//��ֹѭ����ը�ļ�����
        //���Ƴ��Ľڵ��б��ӿ��Ƴ��ڵ�Ĳ��л�ȡ
        List<RogueNode> nodeList = new List<RogueNode>();

        //������ȡ
        foreach(RogueLevel level in levelData)
        {
            //����ò㲻���Ƴ�������
            if (level.NotRemove)
                continue;
            //���б��ڽڵ���ӵ��ڵ��б���
            nodeList.AddRange(level.levelNodes);
        }
        //ѭ��ɾ��Ԥ�������ڵ�
        for(int i = 0; i < deleteRoomCount;i++)
        {
            //����������������ɾ���������࣬ѭ�����ý�
            if(counter > 100)
            {
                Debug.LogWarning("ѭ���������࣡�����Ƿ����������");
                break;
            }
            counter++;
            //��ȡһ������ڵ�
            int j = RogueCore.GetRandomInt(RogueCore.Instance.roomRandom,0, nodeList.Count);
            RogueNode node = nodeList[j];
            //���ʣ�������������
            if (levelData[(int)nodeList[j].position.y].GetNodesCount() > leastNodeCountPreLevel)
            {
                //�÷��䲻Ϊ�� ʵ�����ǲ�����ֵ����
                if (nodeList[j] == null)
                    continue;

                //�Ƴ��ýڵ�
                levelData[(int)node.position.y].levelNodes[(int)node.position.x] = null;
                //���ڵ���б����Ƴ�
                nodeList.RemoveAt(j);
                //Debug.Log("ɾ����λ��" + j + "�ĸ���");
            
            }
            else
            {
                //*δ������ �������ٷ��䱣��ʱֱ�ӽ���Щ�����Ƴ� ���ܻ���Ϊû���㹻�ķ����������
                List<RogueNode> protectList = nodeList.FindAll(x => x.position.y == node.position.y);
                for(int k = 0; k< protectList.Count; k++)
                {
                    nodeList.Remove(protectList[k]);
                }

                //�������� ����ѭ��
                i--;
                continue;
            }
        }
    }
    #endregion

    #region ����·��
    //ԭ����鿴˵���ĵ�
    /// <summary>
    /// ���ɱ�··��
    /// </summary>
    public void SpawnEdge()
    {
        //�������ϱ���ÿһ��
        foreach(RogueLevel level in levelData)
        {
            //BOSS��������
            if (level.y == mapHeight - 1)
                continue;
            //����ڵ�����һ�����ڵ�����
            level.levelNodes[level.GetLeftPlace()].AddConnectNode(levelData[level.y + 1].GetLeftNode().position); /*connectNodeList.Add(levelData[level.y + 1].GetLeftNode().position);*/
            //�����ڵ�����ҽڵ� �� ��һ����ڵ�Ҳ�����ҽڵ� ������һ������ ��ֹ�ظ����(��������������������ڵ����㶼ֻ��1���ڵ�)
            if ((level.GetRightNode().position == level.GetLeftNode().position) && (levelData[level.y + 1].GetLeftNode().position == levelData[level.y + 1].GetRightNode().position))
                continue;
            //���ҽڵ�����һ����ҽڵ�����
            level.levelNodes[level.GetRightPlace()].AddConnectNode(levelData[level.y + 1].GetRightNode().position); /* connectNodeList.Add(levelData[level.y + 1].GetRightNode().position);*/
        }
    }
    /// <summary>
    /// �������нڵ��·��
    /// </summary>
    public void SpawnMainEdge()
    {
        //*ע ��һ������·�����ɰ�����Ե�ڵ� ����ԭ����鿴˵���ĵ�

        //����˳�� ������(��ֹ�ýڵ㲻���κνڵ�����)-������·��-������·��-������·��(һ�ζ��ף���ֹ���Ҷ�û����·��)-���ζ���(�м�û�з��䣬���������Ѱ�ҷ��䣬ԭ����鿴˵���ĵ�)

        //��ͷ��һ��if ����̫����
        //����ÿһ��
        for (int j = 0; j < levelData.Count; j++)
        {
            if (j >= levelData.Count - 1)
                continue;
            //��ȡ�ò�Ľڵ�X�����б����������ڵ㱣֤����� ԭ�������Level����
            List<int> times = levelData[j].GetNodesPositionX();
            //��ȡ���ýڵ�����
            int count = times.Count;
            //�������ýڵ�
            for (int i = 0; i < count; i++)
            {
                //�ӽڵ���������һ�����겢�ӽڵ�����Ƴ�
                int place = times[RogueCore.Instance.roomRandom.Next(0, times.Count)];
                times.Remove(place);
                //��ȡ�ýڵ�
                RogueNode node = levelData[j].levelNodes[place];
                //������:����ýڵ�û�б����� ���Ƿ�0��Ľڵ㣬������Ѱ������
                if(node.position.y != 0 && node.isConnected == false)
                {

                    Vector2 getPos = SearchNodeFromTarget(node.position,-1);
                    Debug.Log("Ҫ���׵�λ��" + node.position + "���ж��׵�λ��" + getPos);
                    levelData[(int)getPos.y].levelNodes[(int)getPos.x].AddConnectNode(node.position);
                }
                //*ע ��������� ������������Ӧ��������������� ����Ӱ�첻�� ������ ��ɫ.jpg
                //if����ж����
                //������ڵ�
                if (node.position.x != 0)
                {
                    //�����ڵ���ڡ���߽ڵ������û�������ҽڵ�
                    if (levelData[(int)node.position.y + 1].levelNodes[(int)node.position.x - 1] != null && levelData[(int)node.position.y ].levelNodes[(int)node.position.x - 1] != null && !levelData[(int)node.position.y ].levelNodes[(int)node.position.x - 1].connectRight)
                    {
                        //����ж��ɹ� ����Ӹýڵ�
                        if (RogueCore.GetRandomFloat(RogueCore.Instance.roomRandom) < addOffsetEdgePossible)
                        {
                            //Debug.Log("������·" + node.position.x + "," + node.position.y + "����");
                            node.AddConnectNode(levelData[(int)node.position.y + 1].levelNodes[(int)node.position.x - 1].position);
                        }
                    }


                }
                //�����ҽڵ�
                if (node.position.x != levelData[(int)node.position.y + 1].levelNodes.Length - 1)
                {
                    //����ҽڵ���ڡ��Ҹ߽ڵ������û��������ڵ�
                    if (levelData[(int)node.position.y + 1].levelNodes[(int)node.position.x + 1] != null && levelData[(int)node.position.y].levelNodes[(int)node.position.x + 1] != null && !levelData[(int)node.position.y ].levelNodes[(int)node.position.x + 1].connectLeft)
                    {
                        //����ж��ɹ� ����Ӹýڵ�
                        if (RogueCore.GetRandomFloat(RogueCore.Instance.roomRandom) < addOffsetEdgePossible)
                        {
                            //Debug.Log("������·" + node.position.x + "," + node.position.y + "����");
                            node.AddConnectNode(levelData[(int)node.position.y + 1].levelNodes[(int)node.position.x + 1].position);
                        }
                    }


                }
                //����м�ڵ����
                if (levelData[(int)node.position.y + 1].levelNodes[(int)node.position.x] != null )
                {
                    //�����м�Ľڵ� �������������ǿ�Ƴ�������
                    if (RogueCore.GetRandomFloat(RogueCore.Instance.roomRandom) < addOffsetEdgePossible || node.connectNodeList.Count <= 0)
                    {
                        //Debug.Log("������·" + node.position.x + "," + node.position.y + "����" + test);
                        node.AddConnectNode(levelData[(int)node.position.y + 1].levelNodes[(int)node.position.x].position);
                        //Debug.Log("����" + levelData[(int)node.position.y].levelNodes[(int)node.position.x].isConnected);
                    }
                }
                //���� �������������м�û�з��� ���Ҷ�û��������·����� ������� ���Ըĳ������ * δ���
                if(node.connectNodeList.Count <= 0)
                {

                    Vector2 getPos = SearchNodeFromTarget(node.position, 1);
                    Debug.Log("�����󶵵�" + getPos);
                    node.AddConnectNode(getPos);
                    //Debug.Log("Ҫ���׵�λ��" + node.position + "���ж��׵�λ��" + getPos);
                    //levelData[(int)getPos.y].levelNodes[(int)getPos.x].AddConnectNode(node.position);
                    //if (levelData[(int)node.position.y + 1].levelNodes[(int)node.position.x + 1] != null)
                    //node.AddConnectNode(levelData[(int)node.position.y + 1].levelNodes[(int)node.position.x + 1].position);
                    //if (levelData[(int)node.position.y + 1].levelNodes[(int)node.position.x - 1] != null)
                    //node.AddConnectNode(levelData[(int)node.position.y + 1].levelNodes[(int)node.position.x - 1].position);
                }

            }
        }
        
    }
    /// <summary>
    /// �����Ѱ��ָ������ĵ�һ�����ýڵ�
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="height"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    public Vector2 SearchNodeFromTarget(Vector2 startPos,int height, int width = 1)
    {
        Debug.Log(startPos + "���ж���");
        //���һ����ʼ����
        int offset = 1;
        int forward = 1/*(RogouCore.GetRandomFloat(RogouCore.Instance.roomRandom) < 0.5f ? -1 : 1)*/;
        //offset = offset * forward;

        if(startPos.y <= 0 && height < 0)
        {
            Debug.LogError("��С�ļ��"); 
            return new Vector2(0, 0);
        }
        //�����ֱ�����нڵ� ���ظýڵ�
        if (levelData[(int)startPos.y + height].levelNodes[(int)startPos.x] != null)
            return new Vector2((int)startPos.x, (int)startPos.y + height);
        //for (int i = 0;startPos.x + width * offset >= 0 && startPos.x +width * offset < mapWidth && i < mapWidth +5;i++)
        //{

        //}
        for(int i = 0;i < mapWidth + 5; i++)
        {
            Debug.Log("��ʼ���" + startPos + "ƫ��" + offset);
            //����ұ�
            if (startPos.x + offset * forward < mapWidth)
            {
                //����ұ�
                if (levelData[(int)startPos.y + height].levelNodes[(int)startPos.x + offset * forward] != null)
                    return new Vector2((int)startPos.x + offset * forward, (int)startPos.y + height);

            }
            //�л��������
            forward = -1;
            if (startPos.x + offset * forward >= 0)
            {
                //������
                if (levelData[(int)startPos.y + height].levelNodes[(int)startPos.x + offset * forward] != null)
                    return new Vector2((int)startPos.x + offset * forward, (int)startPos.y + height);


            }
            //�л������ұ�
            forward = 1;
            //������������
            offset++;
        }
        #region ����©�ı����ϰ�
        //if (RogouCore.GetRandomFloat(RogouCore.Instance.roomRandom) < 0.5f)
        //{
        //for(int i = 0; startPos.x + width * i >= 0 && startPos.x + width * i < mapWidth && i < mapWidth + 5; i++)
        //{
        //    Debug.Log(startPos + "�󶵵������" + (startPos.x + width * i));
        //    if (levelData[(int)startPos.y + height].levelNodes[(int)startPos.x + width * i] != null)
        //        return new Vector2((int)startPos.x + width * i, (int)startPos.y + height);
        //    i = i * -1;
        //}



        ////Debug.Log(startPos + "�󶵵�");
        ////����
        //for(int i = 0; startPos.x + width * i >= 0 && startPos.x + width * i< mapWidth && i < mapWidth + 5;i--)
        //{
        //    Debug.Log(startPos + "�󶵵������" + (startPos.x + width * i));
        //    if (levelData[(int)startPos.y + height].levelNodes[(int)startPos.x + width * i] != null)
        //        return new Vector2((int)startPos.x + width * i, (int)startPos.y + height);

        //}
        ////����
        //for (int i = 0; startPos.x + width * i >= 0 && startPos.x + width * i< mapWidth && i < mapWidth + 5; i++)
        //{
        //    Debug.Log(startPos + "�󶵵��Ҳ���" + (startPos.x + width * i));
        //    if (levelData[(int)startPos.y + height].levelNodes[(int)startPos.x + width * i] != null)
        //        return new Vector2((int)startPos.x + width * i, (int)startPos.y + height);
        //}
        //}
        //else
        //{

        ////����
        //for (int i = 0; startPos.x + width * i >= 0 && startPos.x + width * i < mapWidth && i < mapWidth + 5; i++)
        //{
        //    Debug.Log(startPos + "�Ҷ����Ҳ���" + (startPos.x + width * i));
        //    if (levelData[(int)startPos.y + height].levelNodes[(int)startPos.x + width * i] != null)
        //        return new Vector2((int)startPos.x + width * i, (int)startPos.y + height);
        //}
        ////����
        //for (int i = 0; startPos.x + width * i >= 0 && startPos.x + width * i < mapWidth && i < mapWidth + 5; i--)
        //{
        //    Debug.Log(startPos + "�Ҷ��������" + (startPos.x + width * i));
        //    if (levelData[(int)startPos.y + height].levelNodes[(int)startPos.x + width * i] != null)
        //        return new Vector2((int)startPos.x + width * i, (int)startPos.y + height);

        //}
        //}
        #endregion
        Debug.LogError("�����ǿ��У�");
        return new Vector2 (0,0);
    }
    #endregion

    #region ����ػ���
    public RogueNode GetFixedRewardNode()
    {
        //Debug.Log("��ǰ������������" +chestNodeCountNow + "�̵�" + shopNodeCountNow);
        if (RogueCore.GetRandomFloat(RogueCore.Instance.roomRandom) < 0.5f)
        {
            if (chestNodeCountNow < chestNodeCountMax)
            {
                chestNodeCountNow++;
                return new NodeChest();
            }
            else if (shopNodeCountNow < shopNodeCountMax)
            {
                shopNodeCountNow++;
                return new NodeShop();
            }
            Debug.LogError("�����˳������Ƶ�����");
            return null;
        }
        else
        {
            if (shopNodeCountNow < shopNodeCountMax)
            {
                shopNodeCountNow++;
                return new NodeShop();
            }
            else if (chestNodeCountNow < chestNodeCountMax)
            {
                chestNodeCountNow++;
                return new NodeChest();
            }
            Debug.LogError("�����˳������Ƶ�����");
            return null;
        }
    }
    /// <summary>
    /// ���ض�������г�ȡ����
    /// </summary>
    /// <param name="poolName"></param>
    /// <returns></returns>
    public RogueNode DrawRandomNodeFromPool(string poolName)
    {
        if(!specialNodeDic.ContainsKey(poolName) || specialNodeDic[poolName].Count <= 0)
        {
            Debug.LogError("���Ի�ȡ�����ڵĳ��ӣ�");
            return null;
        }
        int place = RogueCore.GetRandomInt(RogueCore.Instance.roomRandom, 0, specialNodeDic[poolName].Count);
        RogueNode node = specialNodeDic[poolName][place];
        specialNodeDic[poolName].RemoveAt(place);
        return node;
    }

    public RogueNode GetRandomRewardNode()
    {
        if (RogueCore.GetRandomFloat(RogueCore.Instance.roomRandom) < 0.5f)
        {
            return new NodeChest();
        }
        else
            return new NodeShop();
    }
    #endregion

    #endregion

    #region  ��Ȩ����
        /* Copyright (c) 2024 Huang DongYuan��Alos known as PlentyFish / FalckWynter / FarWinter Studio��
         * Comply with the MIT protocol.
         * All code in the namespace "PlentyFishRoguelike" are written by PlentyFish as the first author
         * 
         *                                                                                   PlentyFish
         *                                                                                   2024/3/24
         */
    #endregion
}
}
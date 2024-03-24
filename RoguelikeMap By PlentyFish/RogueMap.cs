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
    #region 地图基本参数
    //*注 由于调试原因 这一组脚本代码有许多明文调用的改参，有待优化
    //地图宽高
    public int mapWidth =4,mapHeight = 8;
    //生成道路的可能性 越大道路越多 越小道路越偏向中间 * 原理请查看说明文档
    public float addOffsetEdgePossible = 0.25f;

    //要删除的房间数量
    public int deleteRoomCount = 2;//数字过大会报错 没做防呆
    //每层最少节点数
    public int leastNodeCountPreLevel = 2;

    //层级数据
    public List<RogueLevel> levelData = new List<RogueLevel>();
    //自由生成的层数数量
    public List<int> freeLevelList = new List<int>();

    //特殊房间池列表
    public Dictionary<string, List<RogueNode>> specialNodeDic = new Dictionary<string, List<RogueNode>>();
    //奖励层 - 宝箱房 商人的最大数量
    public int chestNodeCountMax = 4, chestNodeCountNow = 0, shopNodeCountMax = 4, shopNodeCountNow = 0;//默认数量为一行的数量
    //全为奖励房的层数
    public int rewardLevelCount = 2;//默认为2 需要和上面协调
    //奖励房间池名称
    public string rewardPoolName = "Reward";

    #endregion

    #region 地图初始化(无实际内容)
    public RogueMap()
    {
        Initialize();
    }
    public void Initialize()
    {

    }
    #endregion

    #region 地图功能函数

    #region 重新生成地图(主函数)
    /// <summary>
    /// 重置地图数据，重新生成新的地图，按序列执行功能函数
    /// </summary>
    public void ResetMap()
    {
        //重置参数
        ResetParameter();
        //设置前置参数
        SetPreParameter();
        //重置房间池列表
        ResetNodePool();

        //这个模块可以根据需求自己写定制
        //创建全空节点
        CreateMap();
        //设置BOSS节点
        SetBossNode();
        //设置休息层节点
        SetRestNode();
        //生成固定奖励房间
        SetRewardNode();

        //删除随机节点
        DeleteRandomNode();

        //生成边路
        SpawnEdge();
        //生成主路
        SpawnMainEdge();
    }
    #endregion

    #region 参数互动
    /// <summary>
    /// 重置计数器和楼层存储
    /// </summary>
    public void ResetParameter()
    {
        //重置奖励层计数器、重置楼层数据
        chestNodeCountNow = 0;
        shopNodeCountNow = 0;
        levelData = new List<RogueLevel>();
        freeLevelList = new List<int>();
        specialNodeDic.Clear();
    }
    /// <summary>
    /// 生成特殊节点池列表
    /// </summary>
    public void ResetNodePool()
    {
        //添加奖励池
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
        Debug.Log("词典数量" + specialNodeDic.Count + "节点数" +  specialNodeDic[rewardPoolName].Count);
    }
    /// <summary>
    /// 重置部分与地图客观属性有关的参数
    /// </summary>
    public void SetPreParameter()
    {
        //将奖励层房间数量设为与地图宽度一样 * 注 按需求，每个奖励层有2种mapWidth个房间 
        chestNodeCountMax = mapWidth;
        shopNodeCountMax = mapWidth;
    }
    #endregion

    #region 地图创建与固定设置
    /// <summary>
    /// 创建空地图，填充默认房间;
    /// </summary>
    public void CreateMap()
    {
        //添加地图高度层数的楼层数
        for (int i = 0; i < mapHeight; i++)
        {
            //temp.SetPosition(i);
            //在楼层数据中添加新层 *注意 楼层中填充空房间的函数在Level中自带
            levelData.Add(new RogueLevel(mapWidth, i));
            //将楼层添加到可自由生成列表
            freeLevelList.Add(i);
        }
    }
    /// <summary>
    /// 设置BOSS层(默认为地图最高层)
    /// </summary>
    public void SetBossNode()
    {
        //BOSS位于地图最高层
        int bossLevel = levelData.Count - 1;
        //将BOSS层从自由生成表移除
        freeLevelList.Remove(bossLevel);
        //设置BOSS层为不受删除影响
        levelData[bossLevel].NotRemove = true;
        //设置BOSS层节点，只保留一个节点于n/2，其他设空
        for (int i = 0; i < levelData[bossLevel].levelNodes.Length;i++)
        {
            if (i != levelData[bossLevel].levelNodes.Length / 2)
                levelData[bossLevel].levelNodes[i] = null;
            else
                //将位于n/2的节点设为BOSS房 表现为偶数时中轴偏右
                levelData[bossLevel].SetNode(i,new NodeBoss()) /*levelNodes[i] = new NodeBoss()*/;
        }
    }
    /// <summary>
    /// 设置休息层，默认为比BOSS低一层
    /// </summary>
    public void SetRestNode()
    {
        //休息层位于地图BOSS层下方
        int restLevel = levelData.Count - 2;
        //将休息层从自由生成表移除
        freeLevelList.Remove(restLevel);
        //将休息层设为不受删除影响
        levelData[restLevel].NotRemove = true;
        //懒狗了 用于检测是否已经生成完毕 在进入可生成范围时一次性全部生成
        bool seted = false;
        //遍历休息层节点
        for (int i = 0; i < levelData[restLevel].levelNodes.Length; i++)
        {
            //如果坐标不为中轴左右的房间 跳过
            if (i != levelData[restLevel].levelNodes.Length / 2 && i != (levelData[levelData.Count - 1].levelNodes.Length / 2 - 1))
            {
                levelData[restLevel].levelNodes[i] = null;
                continue;
            }
            //砍一层if，如果在生成范围里 但是已经生成了 则直接跳过
            if (seted)
                continue;

             //Debug.Log("一次随机参数" + RogouCore.GetRandomFloat(RogouCore.Instance.roomRandom));
             //懒狗了 暴力穷举 一个是宝箱则另一个是商人 机会均等
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
            //将已生成Bool设为是
            seted = true;
            
        }
        
    }
    /// <summary>
    /// 设置奖励层 ，这是用来获取奖励层位置的母函数，实际设置函数为带参版本
    /// </summary>
    public void SetRewardNode()
    {
        //在这里将 除了BOSS 休息层以外的 不希望出现的层的高度 移除 
        //此处移除了休息层的下一层和起点层
        freeLevelList.Remove(mapHeight - 3);
        freeLevelList.Remove(0);
        //又懒狗了 因为只生成2个奖励层 所以判断第二与第一个是否重叠就行 *实际上一个固定层就够了 
        int rewardPlace = 0, firstPlace = 0;
        //获取自由层列表
        List<int> tempList = freeLevelList;

        //防爆和调试计数器
        int tim = 0;
        //检测自由层内容
        foreach (int i in freeLevelList)
        {
            tim++;
            //Debug.Log("自由列表可用位置" + i + "内容" + tim);
        }

        //生成奖励层数量的层数
        for (int i = 0; i < rewardLevelCount; i++)
        {
            //要读取的层数ID
            int randomIndex = RogueCore.GetRandomInt(RogueCore.Instance.roomRandom,0, tempList.Count);
            //获取随机的自由层数
            rewardPlace = tempList[randomIndex];
            //暴力的方案 因为只有2层 * 多层时的实现方案为：已生成的奖励层使用一个List，穷举检测是否与List有距离关系，不过一般情况下建议只作一个奖励层
            if (i == 0)
                //如果是第一次生成 则标记它为已生成
                firstPlace = rewardPlace;
            else
            {
                //如果两个奖励层紧贴着
                if (Math.Abs(rewardPlace - firstPlace) == 1)
                    //新奖励层位置为 : 如果新层高于旧层 则将新层上移一层(在矮层数时减少尝试次数)， 如果新层低于旧层，则下移一层
                    rewardPlace = rewardPlace > firstPlace ? tempList[(randomIndex + 1) % tempList.Count] : tempList[(randomIndex + tempList.Count - 1) % tempList.Count];
            }
            //Debug.Log("自由列表生成选择" + rewardPlace + "");
            //从自由行列表中移除该行
            tempList.Remove(rewardPlace);
            //设置该位置为奖励层，生成节点
            SetRewardNode(rewardPlace);
        }
    }
    /// <summary>
    /// 设置奖励层的实际函数，将目标层的节点替换为奖励房
    /// </summary>
    /// <param name="levelPlace">要设置的层数</param>
    public void SetRewardNode(int levelPlace)
    {
        //将该层设为不受删除影响
        levelData[levelPlace].NotRemove = true;
        for(int i = 0; i < mapWidth;i++)
        {
            //从奖励房池子中抽一个房间 * 可以把房间改成List，从List中抽一个房间
            RogueNode node = DrawRandomNodeFromPool(rewardPoolName);/*GetFixedRewardNode()*/;
            //Debug.Log("载入的为" + node.label + "位置" + i);
            //将指定位置的节点设为抽到的房间
            levelData[levelPlace].SetNode(i, node);
        }
    }
    #endregion

    #region 地图节点删除
    /// <summary>
    /// 删除设置数量的随机节点，只建议在地图生成时调用
    /// </summary>
    public void DeleteRandomNode()
    {
        //*注 因为设计上这个函数只在生成时用，所以不做输入参数
        int counter = 0;//防止循环爆炸的计数器
        //可移除的节点列表，从可移除节点的层中获取
        List<RogueNode> nodeList = new List<RogueNode>();

        //遍历获取
        foreach(RogueLevel level in levelData)
        {
            //如果该层不可移除则跳过
            if (level.NotRemove)
                continue;
            //将列表内节点添加到节点列表中
            nodeList.AddRange(level.levelNodes);
        }
        //循环删除预设数量节点
        for(int i = 0; i < deleteRoomCount;i++)
        {
            //防爆计数器，避免删除数量过多，循环不得解
            if(counter > 100)
            {
                Debug.LogWarning("循环次数过多！！！是否代码有问题");
                break;
            }
            counter++;
            //获取一个随机节点
            int j = RogueCore.GetRandomInt(RogueCore.Instance.roomRandom,0, nodeList.Count);
            RogueNode node = nodeList[j];
            //如果剩余多于两个房间
            if (levelData[(int)nodeList[j].position.y].GetNodesCount() > leastNodeCountPreLevel)
            {
                //该房间不为空 实际上是不会出现的情况
                if (nodeList[j] == null)
                    continue;

                //移除该节点
                levelData[(int)node.position.y].levelNodes[(int)node.position.x] = null;
                //将节点从列表中移除
                nodeList.RemoveAt(j);
                //Debug.Log("删除了位于" + j + "的格子");
            
            }
            else
            {
                //*未经测试 触发最少房间保护时直接将这些房间移除 可能会因为没有足够的房间产生报错
                List<RogueNode> protectList = nodeList.FindAll(x => x.position.y == node.position.y);
                for(int k = 0; k< protectList.Count; k++)
                {
                    nodeList.Remove(protectList[k]);
                }

                //计数撤回 继续循环
                i--;
                continue;
            }
        }
    }
    #endregion

    #region 生成路径
    //原理请查看说明文档
    /// <summary>
    /// 生成边路路径
    /// </summary>
    public void SpawnEdge()
    {
        //从下往上遍历每一层
        foreach(RogueLevel level in levelData)
        {
            //BOSS层则跳过
            if (level.y == mapHeight - 1)
                continue;
            //令左节点与上一层的左节点链接
            level.levelNodes[level.GetLeftPlace()].AddConnectNode(levelData[level.y + 1].GetLeftNode().position); /*connectNodeList.Add(levelData[level.y + 1].GetLeftNode().position);*/
            //如果左节点等于右节点 且 下一级左节点也等于右节点 结束这一次载入 防止重复添加(用于特殊情况：上下相邻的两层都只有1个节点)
            if ((level.GetRightNode().position == level.GetLeftNode().position) && (levelData[level.y + 1].GetLeftNode().position == levelData[level.y + 1].GetRightNode().position))
                continue;
            //令右节点与上一层的右节点链接
            level.levelNodes[level.GetRightPlace()].AddConnectNode(levelData[level.y + 1].GetRightNode().position); /* connectNodeList.Add(levelData[level.y + 1].GetRightNode().position);*/
        }
    }
    /// <summary>
    /// 生成所有节点的路径
    /// </summary>
    public void SpawnMainEdge()
    {
        //*注 这一函数的路径生成包括边缘节点 具体原理请查看说明文档

        //生成顺序： 反兜底(防止该节点不被任何节点连接)-生成左路线-生成右路线-生成中路线(一次兜底，防止左右都没生成路线)-二次兜底(中间没有房间，随机向两侧寻找房间，原理请查看说明文档)

        //回头解一下if 层数太多了
        //遍历每一层
        for (int j = 0; j < levelData.Count; j++)
        {
            if (j >= levelData.Count - 1)
                continue;
            //获取该层的节点X坐标列表，用于随机抽节点保证随机性 原理请查阅Level代码
            List<int> times = levelData[j].GetNodesPositionX();
            //获取可用节点数量
            int count = times.Count;
            //遍历可用节点
            for (int i = 0; i < count; i++)
            {
                //从节点表中随机抽一个坐标并从节点表中移除
                int place = times[RogueCore.Instance.roomRandom.Next(0, times.Count)];
                times.Remove(place);
                //获取该节点
                RogueNode node = levelData[j].levelNodes[place];
                //反兜底:如果该节点没有被链接 且是非0层的节点，则向下寻找链接
                if(node.position.y != 0 && node.isConnected == false)
                {

                    Vector2 getPos = SearchNodeFromTarget(node.position,-1);
                    Debug.Log("要兜底的位置" + node.position + "进行兜底的位置" + getPos);
                    levelData[(int)getPos.y].levelNodes[(int)getPos.x].AddConnectNode(node.position);
                }
                //*注 理想情况下 先生成左还是右应该做个函数随机抽 不过影响不大 懒狗了 特色.jpg
                //if好像卸不掉
                //不是左节点
                if (node.position.x != 0)
                {
                    //如果左节点存在、左高节点存在且没有链接右节点
                    if (levelData[(int)node.position.y + 1].levelNodes[(int)node.position.x - 1] != null && levelData[(int)node.position.y ].levelNodes[(int)node.position.x - 1] != null && !levelData[(int)node.position.y ].levelNodes[(int)node.position.x - 1].connectRight)
                    {
                        //如果判定成功 则添加该节点
                        if (RogueCore.GetRandomFloat(RogueCore.Instance.roomRandom) < addOffsetEdgePossible)
                        {
                            //Debug.Log("生成左路" + node.position.x + "," + node.position.y + "坐标");
                            node.AddConnectNode(levelData[(int)node.position.y + 1].levelNodes[(int)node.position.x - 1].position);
                        }
                    }


                }
                //不是右节点
                if (node.position.x != levelData[(int)node.position.y + 1].levelNodes.Length - 1)
                {
                    //如果右节点存在、右高节点存在且没有链接左节点
                    if (levelData[(int)node.position.y + 1].levelNodes[(int)node.position.x + 1] != null && levelData[(int)node.position.y].levelNodes[(int)node.position.x + 1] != null && !levelData[(int)node.position.y ].levelNodes[(int)node.position.x + 1].connectLeft)
                    {
                        //如果判定成功 则添加该节点
                        if (RogueCore.GetRandomFloat(RogueCore.Instance.roomRandom) < addOffsetEdgePossible)
                        {
                            //Debug.Log("生成右路" + node.position.x + "," + node.position.y + "坐标");
                            node.AddConnectNode(levelData[(int)node.position.y + 1].levelNodes[(int)node.position.x + 1].position);
                        }
                    }


                }
                //如果中间节点存在
                if (levelData[(int)node.position.y + 1].levelNodes[(int)node.position.x] != null )
                {
                    //链接中间的节点 如果触发兜底则强制尝试链接
                    if (RogueCore.GetRandomFloat(RogueCore.Instance.roomRandom) < addOffsetEdgePossible || node.connectNodeList.Count <= 0)
                    {
                        //Debug.Log("生成主路" + node.position.x + "," + node.position.y + "坐标" + test);
                        node.AddConnectNode(levelData[(int)node.position.y + 1].levelNodes[(int)node.position.x].position);
                        //Debug.Log("坐标" + levelData[(int)node.position.y].levelNodes[(int)node.position.x].isConnected);
                    }
                }
                //兜底 极端情况下如果中间没有房间 左右都没有生成线路的情况 先连左边 可以改成随机数 * 未完成
                if(node.connectNodeList.Count <= 0)
                {

                    Vector2 getPos = SearchNodeFromTarget(node.position, 1);
                    Debug.Log("触发大兜底" + getPos);
                    node.AddConnectNode(getPos);
                    //Debug.Log("要兜底的位置" + node.position + "进行兜底的位置" + getPos);
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
    /// 从起点寻找指定方向的第一个可用节点
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="height"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    public Vector2 SearchNodeFromTarget(Vector2 startPos,int height, int width = 1)
    {
        Debug.Log(startPos + "进行兜底");
        //随机一个初始方向
        int offset = 1;
        int forward = 1/*(RogouCore.GetRandomFloat(RogouCore.Instance.roomRandom) < 0.5f ? -1 : 1)*/;
        //offset = offset * forward;

        if(startPos.y <= 0 && height < 0)
        {
            Debug.LogError("过小的检测"); 
            return new Vector2(0, 0);
        }
        //如果垂直方向有节点 返回该节点
        if (levelData[(int)startPos.y + height].levelNodes[(int)startPos.x] != null)
            return new Vector2((int)startPos.x, (int)startPos.y + height);
        //for (int i = 0;startPos.x + width * offset >= 0 && startPos.x +width * offset < mapWidth && i < mapWidth +5;i++)
        //{

        //}
        for(int i = 0;i < mapWidth + 5; i++)
        {
            Debug.Log("开始检测" + startPos + "偏移" + offset);
            //检测右边
            if (startPos.x + offset * forward < mapWidth)
            {
                //检测右边
                if (levelData[(int)startPos.y + height].levelNodes[(int)startPos.x + offset * forward] != null)
                    return new Vector2((int)startPos.x + offset * forward, (int)startPos.y + height);

            }
            //切换方向到左边
            forward = -1;
            if (startPos.x + offset * forward >= 0)
            {
                //检测左边
                if (levelData[(int)startPos.y + height].levelNodes[(int)startPos.x + offset * forward] != null)
                    return new Vector2((int)startPos.x + offset * forward, (int)startPos.y + height);


            }
            //切换方向到右边
            forward = 1;
            //扩大搜索距离
            offset++;
        }
        #region 有遗漏的暴力废案
        //if (RogouCore.GetRandomFloat(RogouCore.Instance.roomRandom) < 0.5f)
        //{
        //for(int i = 0; startPos.x + width * i >= 0 && startPos.x + width * i < mapWidth && i < mapWidth + 5; i++)
        //{
        //    Debug.Log(startPos + "左兜底左查找" + (startPos.x + width * i));
        //    if (levelData[(int)startPos.y + height].levelNodes[(int)startPos.x + width * i] != null)
        //        return new Vector2((int)startPos.x + width * i, (int)startPos.y + height);
        //    i = i * -1;
        //}



        ////Debug.Log(startPos + "左兜底");
        ////向左
        //for(int i = 0; startPos.x + width * i >= 0 && startPos.x + width * i< mapWidth && i < mapWidth + 5;i--)
        //{
        //    Debug.Log(startPos + "左兜底左查找" + (startPos.x + width * i));
        //    if (levelData[(int)startPos.y + height].levelNodes[(int)startPos.x + width * i] != null)
        //        return new Vector2((int)startPos.x + width * i, (int)startPos.y + height);

        //}
        ////向右
        //for (int i = 0; startPos.x + width * i >= 0 && startPos.x + width * i< mapWidth && i < mapWidth + 5; i++)
        //{
        //    Debug.Log(startPos + "左兜底右查找" + (startPos.x + width * i));
        //    if (levelData[(int)startPos.y + height].levelNodes[(int)startPos.x + width * i] != null)
        //        return new Vector2((int)startPos.x + width * i, (int)startPos.y + height);
        //}
        //}
        //else
        //{

        ////向右
        //for (int i = 0; startPos.x + width * i >= 0 && startPos.x + width * i < mapWidth && i < mapWidth + 5; i++)
        //{
        //    Debug.Log(startPos + "右兜底右查找" + (startPos.x + width * i));
        //    if (levelData[(int)startPos.y + height].levelNodes[(int)startPos.x + width * i] != null)
        //        return new Vector2((int)startPos.x + width * i, (int)startPos.y + height);
        //}
        ////向左
        //for (int i = 0; startPos.x + width * i >= 0 && startPos.x + width * i < mapWidth && i < mapWidth + 5; i--)
        //{
        //    Debug.Log(startPos + "右兜底左查找" + (startPos.x + width * i));
        //    if (levelData[(int)startPos.y + height].levelNodes[(int)startPos.x + width * i] != null)
        //        return new Vector2((int)startPos.x + width * i, (int)startPos.y + height);

        //}
        //}
        #endregion
        Debug.LogError("下面是空行？");
        return new Vector2 (0,0);
    }
    #endregion

    #region 房间池互动
    public RogueNode GetFixedRewardNode()
    {
        //Debug.Log("当前载入数量宝箱" +chestNodeCountNow + "商店" + shopNodeCountNow);
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
            Debug.LogError("载入了超过限制的数量");
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
            Debug.LogError("载入了超过限制的数量");
            return null;
        }
    }
    /// <summary>
    /// 从特定房间池中抽取房间
    /// </summary>
    /// <param name="poolName"></param>
    /// <returns></returns>
    public RogueNode DrawRandomNodeFromPool(string poolName)
    {
        if(!specialNodeDic.ContainsKey(poolName) || specialNodeDic[poolName].Count <= 0)
        {
            Debug.LogError("尝试获取不存在的池子？");
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

    #region  版权声明
        /* Copyright (c) 2024 Huang DongYuan（Alos known as PlentyFish / FalckWynter / FarWinter Studio）
         * Comply with the MIT protocol.
         * All code in the namespace "PlentyFishRoguelike" are written by PlentyFish as the first author
         * 
         *                                                                                   PlentyFish
         *                                                                                   2024/3/24
         */
    #endregion
}
}
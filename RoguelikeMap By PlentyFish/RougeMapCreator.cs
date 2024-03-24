using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using PFRoguelike;

namespace PFRoguelike
{
    [Serializable]
public class RogueMapCreator : MonoBehaviour
{
    #region 基本参数
    //节点对象位置
    public GameObject[,] mapObjectMap;
    //要生成的Prefab 要挂载的父物体 可自己写代码设置
    //MonoBehaviour可以去掉 但是这两个参数要自己设置输入
    //Prefab请挂载LineRender组件
    public GameObject prefab,parent;
    //可用节点列表
    public List<GameObject> nodeList = new List<GameObject>();
    //路径的图片
    public Sprite PathImage;
    #endregion

    #region 功能函数
    /// <summary>
    /// 生成地图
    /// </summary>
    /// <param name="map"></param>
    public void CreateMap(RogueMap map)
    {
        //清空地图
        DeleteMap();
        //重设地图
        mapObjectMap = new GameObject[map.mapWidth, map.mapHeight];
        //遍历节点
        for(int j = 0;j < map.mapHeight; j++)
        {
            for(int i = 0;i<map.mapWidth;i++)
            {
                //为空则跳过
                if (map.levelData[j].levelNodes[i] == null)
                    continue;
                //获取节点坐标
                Vector2 roomPos = map.levelData[j].levelNodes[i].position;
                //生成物体
                GameObject temp = GameObject.Instantiate(prefab, parent.transform);
                //设置数据
                temp.GetComponent<RoomMono>().SetRoomData(roomPos);
                //添加节点到列表
                nodeList.Add(temp);
                //将节点放入数组
                mapObjectMap[i, j] = temp;
                //设置名称、图片和坐标
                //temp.GetComponent<TextMeshPro>().text = map.levelData[j].levelNodes[i].label;
                temp.transform.Find("Square").GetComponent<SpriteRenderer>().sprite = map.levelData[j].levelNodes[i].sprite;
                temp.transform.position = new Vector3(2*i, 2*j, 0);
            }
        }
       //生成节点路径画线
        foreach(GameObject temp in nodeList)
        {
            //获取节点的画线器
            LineRenderer tempLineRender = temp.GetComponent<LineRenderer>();
            //设置线条图片
            tempLineRender.material.mainTexture = PathImage.texture;
            //获取节点位置
            Vector2 pos = temp.GetComponent<RoomMono>().roomPosition;
            //获取对应节点数据
            RogueNode node = map.levelData[(int)pos.y].levelNodes[(int)pos.x];
            //获取节点连接点数量
            tempLineRender.positionCount = node.connectNodeList.Count * 2;
            int counter = 0;
            foreach (Vector2 room in node.connectNodeList)
            {
                //Debug.Log("房间坐标" + room +  "目标" + mapObjectMap[(int)room.x, (int)room.y].transform.position + "计数"  + counter);
                //tempLineRender.SetPosition(counter * 2, /*mapObjectMap[(int)room.connectRoom[counter].position.x, (int)room.connectRoom[counter].position.y].transform.position*/new Vector3(room.position.x * 2,room.position.y * 2,0));
                //画线到连接点
                tempLineRender.SetPosition(counter * 2, mapObjectMap[(int)room.x, (int)room.y].transform.position);
                //返回到起点
                tempLineRender.SetPosition(counter * 2 + 1, temp.transform.position);
                //增加计数
                counter++;
            }
        }
    }
    /// <summary>
    /// 清空地图
    /// </summary>
    public void DeleteMap()
    {
        //删除游戏对象
        for(int i = 0; i<nodeList.Count; i ++)
        {
            GameObject.Destroy(nodeList[i].gameObject);
        }
        //重置对象地图 节点列表
        mapObjectMap = null;
        nodeList = new List<GameObject>();
    }
    #endregion
}
}

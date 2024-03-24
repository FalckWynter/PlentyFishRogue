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
    #region ��������
    //�ڵ����λ��
    public GameObject[,] mapObjectMap;
    //Ҫ���ɵ�Prefab Ҫ���صĸ����� ���Լ�д��������
    //MonoBehaviour����ȥ�� ��������������Ҫ�Լ���������
    //Prefab�����LineRender���
    public GameObject prefab,parent;
    //���ýڵ��б�
    public List<GameObject> nodeList = new List<GameObject>();
    //·����ͼƬ
    public Sprite PathImage;
    #endregion

    #region ���ܺ���
    /// <summary>
    /// ���ɵ�ͼ
    /// </summary>
    /// <param name="map"></param>
    public void CreateMap(RogueMap map)
    {
        //��յ�ͼ
        DeleteMap();
        //�����ͼ
        mapObjectMap = new GameObject[map.mapWidth, map.mapHeight];
        //�����ڵ�
        for(int j = 0;j < map.mapHeight; j++)
        {
            for(int i = 0;i<map.mapWidth;i++)
            {
                //Ϊ��������
                if (map.levelData[j].levelNodes[i] == null)
                    continue;
                //��ȡ�ڵ�����
                Vector2 roomPos = map.levelData[j].levelNodes[i].position;
                //��������
                GameObject temp = GameObject.Instantiate(prefab, parent.transform);
                //��������
                temp.GetComponent<RoomMono>().SetRoomData(roomPos);
                //��ӽڵ㵽�б�
                nodeList.Add(temp);
                //���ڵ��������
                mapObjectMap[i, j] = temp;
                //�������ơ�ͼƬ������
                //temp.GetComponent<TextMeshPro>().text = map.levelData[j].levelNodes[i].label;
                temp.transform.Find("Square").GetComponent<SpriteRenderer>().sprite = map.levelData[j].levelNodes[i].sprite;
                temp.transform.position = new Vector3(2*i, 2*j, 0);
            }
        }
       //���ɽڵ�·������
        foreach(GameObject temp in nodeList)
        {
            //��ȡ�ڵ�Ļ�����
            LineRenderer tempLineRender = temp.GetComponent<LineRenderer>();
            //��������ͼƬ
            tempLineRender.material.mainTexture = PathImage.texture;
            //��ȡ�ڵ�λ��
            Vector2 pos = temp.GetComponent<RoomMono>().roomPosition;
            //��ȡ��Ӧ�ڵ�����
            RogueNode node = map.levelData[(int)pos.y].levelNodes[(int)pos.x];
            //��ȡ�ڵ����ӵ�����
            tempLineRender.positionCount = node.connectNodeList.Count * 2;
            int counter = 0;
            foreach (Vector2 room in node.connectNodeList)
            {
                //Debug.Log("��������" + room +  "Ŀ��" + mapObjectMap[(int)room.x, (int)room.y].transform.position + "����"  + counter);
                //tempLineRender.SetPosition(counter * 2, /*mapObjectMap[(int)room.connectRoom[counter].position.x, (int)room.connectRoom[counter].position.y].transform.position*/new Vector3(room.position.x * 2,room.position.y * 2,0));
                //���ߵ����ӵ�
                tempLineRender.SetPosition(counter * 2, mapObjectMap[(int)room.x, (int)room.y].transform.position);
                //���ص����
                tempLineRender.SetPosition(counter * 2 + 1, temp.transform.position);
                //���Ӽ���
                counter++;
            }
        }
    }
    /// <summary>
    /// ��յ�ͼ
    /// </summary>
    public void DeleteMap()
    {
        //ɾ����Ϸ����
        for(int i = 0; i<nodeList.Count; i ++)
        {
            GameObject.Destroy(nodeList[i].gameObject);
        }
        //���ö����ͼ �ڵ��б�
        mapObjectMap = null;
        nodeList = new List<GameObject>();
    }
    #endregion
}
}

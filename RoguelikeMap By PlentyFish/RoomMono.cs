using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PFRoguelike;

namespace PFRoguelike
{
[Serializable]
public class RoomMono : MonoBehaviour
{
    public Vector2 roomPosition = new Vector2();
    public void SetRoomData(Vector2 pos)
    {
        roomPosition = pos;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
}
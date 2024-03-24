using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PFRoguelike;

namespace PFRoguelike
{
public class NodeBoss : RogueNode
{
    public NodeBoss(string la = "NotNull")
    {
        label = "Boss";
        Initialize();
    }
    public override void Initialize()
    {
        base.Initialize();

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
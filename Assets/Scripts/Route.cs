using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Route
{
    public List<Node> nodes = new List<Node>();
    public float cost = 0;

    public void Add(Node node)
    {
        nodes.Add(node);
/*        if (nodes.Count > 0)
        {
            cost += AStarMgr.instance.Distance(nodes[nodes.Count - 1], node);
        }*/
    }

    public void Clear()
    {
        nodes.Clear();
        cost = 0;
    }
}

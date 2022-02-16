using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node parent;

    public List<Node> successors;
    public bool closed = false;
    public bool obstacle = false;

    public float f = Mathf.Infinity;
    public float g = Mathf.Infinity;

    private void Awake()
    {
    }

    public void SetParent(Node par)
    {
        parent = par;
    }

}

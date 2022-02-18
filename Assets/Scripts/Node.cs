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

    public Renderer renderer;

    private LineRenderer lr;

    public void Awake()
    {
        renderer = transform.GetComponent<Renderer>();
        lr = GetComponent<LineRenderer>();
    }

    public void Reset()
    {
        f = Mathf.Infinity;
        g = Mathf.Infinity;
        successors = new List<Node>();
        parent = null;
        closed = false;
        obstacle = false;
    }

    public void SetParent(Node par)
    {
        parent = par;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position  + ((par.transform.position - transform.position) * 0.5f) );
    }

}

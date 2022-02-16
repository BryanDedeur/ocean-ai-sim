using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarMgr : MonoBehaviour
{
    public static AStarMgr instance;
    public bool enabled = false;
    public Vector2Int searchSpaceResolution;

    public GameObject nodePrefab;
    public List<List<Node>> nodes;

    private float xStepSize = 0;
    private float zStepSize = 0;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        CreateNodes(new Vector2(-50, -50), new Vector2(50, 50));
        CheckForObstacles();
        CacheNeighbors();
    }

    // Start is called before the first frame update
    void Start()
    {
        /*StartCoroutine(ComputeNodeNeighbors());*/
    }

    void ClearNodes()
    {
        if (nodes != null)
        {
            for (int c = 0; c < nodes.Count; c++)
            {
                if (nodes[c] != null)
                {
                    for (int r = 0; r < nodes[c].Count; r++)
                    {
                        Destroy(nodes[c][r]);
                    }
                    nodes[c].Clear();
                }
            }
            nodes.Clear();
        }
    }

    void CreateNodes(Vector2 corner1, Vector2 corner2)
    {
        xStepSize = Mathf.Abs(corner1.x - corner2.x) / searchSpaceResolution.x;
        zStepSize = Mathf.Abs(corner1.y - corner2.y) / searchSpaceResolution.y;

        float startX = Mathf.Min(corner1.x, corner2.x);
        float startZ = Mathf.Min(corner1.y, corner2.y);

        // Reset the nodes array
        ClearNodes();

        nodes = new List<List<Node>>();

        int i = 0;
        for (int x = 0; x < searchSpaceResolution.x; x++)
        {
            nodes.Add(new List<Node>());
            for (int z = 0; z < searchSpaceResolution.y; z++)
            {
                GameObject nodeClone = Instantiate(nodePrefab);
                nodeClone.transform.position = new Vector3(startX + (x * xStepSize), 0, startZ + (z * zStepSize));
                nodes[x].Add(nodeClone.GetComponent<Node>());
                nodeClone.name = "node" + i.ToString();
                i++;
            }
        }
    }

/*    IEnumerator ComputeNodeNeighbors()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            Node node = nodes[i];
            foreach (Node otherNode in nodes)
            {
                if (node != otherNode)
                {
                    float xDistance = Mathf.Abs(node.transform.position.x - otherNode.transform.position.x);
                    if (xDistance <= xStepSize)
                    {
                        float zDistance = Mathf.Abs(node.transform.position.z - otherNode.transform.position.z);
                        if (zDistance <= zStepSize)
                        {
                            node.neighbors.Add(otherNode);
                        }
                    }
                }
            }
            if (i % 10 == 0)
            {
                yield return null;
            }
        }
    }*/

    public Node GetNodeNearestToPoint(Vector3 point)
    {
        Node closest = nodes[0][0];
        float closestDistanceSq = Mathf.Pow(Mathf.Abs(point.x - closest.transform.position.x),2) + Mathf.Pow(Mathf.Abs(point.z - closest.transform.position.z),2);
        float distanceSq = 0;
        for (int c = 0; c < nodes.Count; c++)
        {
            for (int r = 0; r < nodes[c].Count; r++)
            {
                distanceSq = Mathf.Pow(Mathf.Abs(point.x - nodes[c][r].transform.position.x), 2) + Mathf.Pow(Mathf.Abs(point.z - nodes[c][r].transform.position.z), 2);
                if (distanceSq < closestDistanceSq)
                {
                    closestDistanceSq = distanceSq;
                    closest = nodes[c][r];
                }
            }
        }

        return closest;
    }


    public void CheckForObstacles()
    {
        Vector3 offset = new Vector3(0, 100, 0);
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.

        RaycastHit hit;
        for (int c = 0; c < nodes.Count; c++)
        {
            for (int r = 0; r < nodes[c].Count; r++)
            {
                if (Physics.Raycast(nodes[c][r].transform.position + offset, Vector3.down, out hit, Mathf.Infinity, layerMask))
                {
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                    nodes[c][r].obstacle = true;
                }
            }
        }
    }

    public float Distance(Node a, Node b)
    {
        return (a.transform.position - b.transform.position).magnitude;
    }

    public Route ComputeRoute(Vector3 start, Vector3 end)
    {
        List<Node> modified = new List<Node>();

        Route route = new Route();
        Node goal = GetNodeNearestToPoint(end);

        Node begin = GetNodeNearestToPoint(start);
        begin.f = Distance(begin, goal);
        begin.g = 0;
        modified.Add(begin);

        List<Node> open = new List<Node>();

        open.Add(begin);

        int counter = 0;
        int max = searchSpaceResolution.x * searchSpaceResolution.y;
        while (open.Count > 0)
        {
            Node q = null;
            float lowestCost = Mathf.Infinity;
            foreach (Node node in open)
            {
                if (node.f < lowestCost)
                {
                    lowestCost = node.f;
                    q = node;
                }
            }

            open.Remove(q);
            q.closed = true;
            counter++;
            //Renderer r = q.transform.GetComponent<Renderer>();
            //r.material.color = new Color(0, 0, 0);

            if (q == goal)
            {
                break;
            }

            foreach (Node successor in q.successors)
            {
                if (successor.closed)
                {
                    continue;
                }

                float g = q.g + Distance(q, successor);
                float h = Distance(q, goal);
                float f = g + h;

                if (f < successor.f)
                {
                    successor.f = f;
                    successor.g = g;
                    successor.SetParent(q);
                    open.Add(successor);
                    modified.Add(successor);
                    //Renderer r = successor.transform.GetComponent<Renderer>();
                    //r.material.color = new Color(1, 0, 0);
                }
            }
            if (counter >= max)
            {
                return null;
            }
        }

        if (goal.parent == null)
        {
            return null;
        }

        if (begin != goal)
        {
            Node current = goal;
            while (current.parent != begin)
            {
                route.nodes.Insert(0, current);
                current = current.parent;
            }
        }

        foreach (Node node in modified)
        {
            node.f = Mathf.Infinity;
            node.g = Mathf.Infinity;
            node.parent = null;
            node.closed = false;
        }
        modified.Clear();

        return route;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CacheNeighbors()
    {
        /*
        Generating all the 8 successor of this cell
 
            N.W   N   N.E
            \   |   /
            \  |  /
            W----Cell----E
                / | \
            /   |  \
            S.W    S   S.E
 
        Cell-->Popped Cell (i, j)
        N -->  North       (i-1, j)
        S -->  South       (i+1, j)
        E -->  East        (i, j+1)
        W -->  West           (i, j-1)
        N.E--> North-East  (i-1, j+1)
        N.W--> North-West  (i-1, j-1)
        S.E--> South-East  (i+1, j+1)
        S.W--> South-West  (i+1, j-1)*/
        for (int i = 0; i < nodes.Count; i++)
        {
            for (int j = 0; j < nodes[i].Count; j++)
            {
                if (i > 0 && j > 0 && i < nodes.Count - 1 && j < nodes[i].Count - 1)
                {
                    // Not at a boundary we can add all successors
                    if (!nodes[i - 1][j].obstacle)
                        nodes[i][j].successors.Add(nodes[i - 1][j]);
                    if (!nodes[i + 1][j].obstacle)
                        nodes[i][j].successors.Add(nodes[i + 1][j]);
                    if (!nodes[i][j - 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i][j - 1]);
                    if (!nodes[i][j + 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i][j + 1]);
                    if (!nodes[i - 1][j - 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i - 1][j - 1]);
                    if (!nodes[i + 1][j - 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i + 1][j - 1]);
                    if (!nodes[i - 1][j + 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i - 1][j + 1]);
                    if (!nodes[i + 1][j + 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i + 1][j + 1]);
                }
                else if (i == 0 && j == 0)
                {
                    if(!nodes[i + 1][j].obstacle)
                        nodes[i][j].successors.Add(nodes[i + 1][j]);
                    if(!nodes[i][j + 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i][j + 1]);
                    if(!nodes[i + 1][j + 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i + 1][j + 1]);
                }
                else if (i == 0 && j == nodes[i].Count - 1)
                {
                    if(!nodes[i + 1][j].obstacle)
                        nodes[i][j].successors.Add(nodes[i + 1][j]);
                    if(!nodes[i][j - 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i][j - 1]);
                    if(!nodes[i + 1][j - 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i + 1][j - 1]);
                }
                else if (i == nodes.Count - 1 && j == 0)
                {
                    if(!nodes[i - 1][j].obstacle)
                        nodes[i][j].successors.Add(nodes[i - 1][j]);
                    if(!nodes[i][j + 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i][j + 1]);
                    if(!nodes[i - 1][j + 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i - 1][j + 1]);
                }
                else if (i == nodes.Count - 1 && j == nodes[i].Count - 1)
                {
                    if(!nodes[i - 1][j].obstacle)
                        nodes[i][j].successors.Add(nodes[i - 1][j]);
                    if(!nodes[i][j - 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i][j - 1]);
                    if(!nodes[i - 1][j - 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i - 1][j - 1]);
                }
                else if (i == 0)
                {
                    // no i -
                    if(!nodes[i + 1][j].obstacle)
                        nodes[i][j].successors.Add(nodes[i + 1][j]);
                    if(!nodes[i][j - 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i][j - 1]);
                    if(!nodes[i][j + 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i][j + 1]);
                    if(!nodes[i + 1][j - 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i + 1][j - 1]);
                    if(!nodes[i + 1][j + 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i + 1][j + 1]);
                }
                else if (i == nodes.Count - 1)
                {
                    // no i +
                    if(!nodes[i - 1][j].obstacle)
                        nodes[i][j].successors.Add(nodes[i - 1][j]);
                    if(!nodes[i][j - 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i][j - 1]);
                    if(!nodes[i][j + 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i][j + 1]);
                    if(!nodes[i - 1][j - 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i - 1][j - 1]);
                    if(!nodes[i - 1][j + 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i - 1][j + 1]);
                }
                else if (j == 0)
                {
                    // no j -
                    if(!nodes[i - 1][j].obstacle)
                        nodes[i][j].successors.Add(nodes[i - 1][j]);
                    if(!nodes[i + 1][j].obstacle)
                        nodes[i][j].successors.Add(nodes[i + 1][j]);
                    if(!nodes[i][j + 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i][j + 1]);
                    if(!nodes[i - 1][j + 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i - 1][j + 1]);
                    if(!nodes[i + 1][j + 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i + 1][j + 1]);
                }
                else if (j == nodes[i].Count - 1)
                {
                    // no j +
                    if(!nodes[i - 1][j].obstacle)
                        nodes[i][j].successors.Add(nodes[i - 1][j]);
                    if(!nodes[i + 1][j].obstacle)
                        nodes[i][j].successors.Add(nodes[i + 1][j]);
                    if(!nodes[i][j - 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i][j - 1]);
                    if(!nodes[i - 1][j - 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i - 1][j - 1]);
                    if(!nodes[i + 1][j - 1].obstacle)
                        nodes[i][j].successors.Add(nodes[i + 1][j - 1]);
                }
            }
        }
    }
}

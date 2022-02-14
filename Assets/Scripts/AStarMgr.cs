using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarMgr : MonoBehaviour
{
    public static AStarMgr instance;
    public Vector2Int searchSpaceResolution;

    public GameObject nodePrefab;
    public List<Node> nodes;

    private float xStepSize = 0;
    private float zStepSize = 0;

    private List<Node> markedNodes;

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
        markedNodes = new List<Node>();
        PlotNodes(new Vector2(-50, -50), new Vector2(50, 50));
        CheckForObstacles();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ComputeNodeNeighbors());
    }

    void PlotNodes(Vector2 corner1, Vector2 corner2)
    {
        xStepSize = Mathf.Abs(corner1.x - corner2.x) / searchSpaceResolution.x;
        zStepSize = Mathf.Abs(corner1.y - corner2.y) / searchSpaceResolution.y;

        float startX = Mathf.Min(corner1.x, corner2.x);
        float startZ = Mathf.Min(corner1.y, corner2.y);

        int i = 0;
        for (int x = 0; x < searchSpaceResolution.x; x++)
        {
            for (int z = 0; z < searchSpaceResolution.y; z++)
            {
                GameObject nodeClone = Instantiate(nodePrefab);
                nodeClone.transform.position = new Vector3(startX + (x * xStepSize), 0, startZ + (z * zStepSize));
                nodes.Add(nodeClone.GetComponent<Node>());
                nodeClone.name = "node" + i.ToString();
                i++;
            }
        }
    }

    IEnumerator ComputeNodeNeighbors()
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
    }

    public void ResetNodes()
    {
        foreach (Node node in nodes)
        {
            node.considered = false;
        }
    }

    public Node GetNearestToPoint(Vector3 point)
    {
        Node closest = nodes[0];
        float closestDistanceSq = Mathf.Pow(Mathf.Abs(point.x - closest.transform.position.x),2) + Mathf.Pow(Mathf.Abs(point.z - closest.transform.position.z),2);
        foreach (Node node in nodes)
        {
            float distanceSq = Mathf.Pow(Mathf.Abs(point.x - node.transform.position.x), 2) + Mathf.Pow(Mathf.Abs(point.z - node.transform.position.z), 2);
            if (distanceSq < closestDistanceSq)
            {
                closestDistanceSq = distanceSq;
                closest = node;
            }
        }
        return closest;
    }

    public void CheckForObstacles()
    {
        Vector3 offset = new Vector3(0, 100, 0);
        foreach (Node node in nodes)
        {

            // Bit shift the index of the layer (8) to get a bit mask
            int layerMask = 1 << 8;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.

            RaycastHit hit;
            if (Physics.Raycast(node.transform.position + offset, Vector3.down, out hit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                Debug.Log("Did Hit");
                node.obstacle = true;
            }
        }
    }



    public Route ComputeRoute(Vector3 start, Vector3 end)
    {
        ResetNodes();

        Route route = new Route();
        Node begin = GetNearestToPoint(start);
        route.Add(begin);

        Node goal = GetNearestToPoint(end);
        route = begin.ComputeNextDepth(route, goal);

        return route;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

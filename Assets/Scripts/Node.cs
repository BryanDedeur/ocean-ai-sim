using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> neighbors;
    public bool considered = false;
    public bool obstacle;

    public Route ComputeNextDepth(Route currentRoute, Node goal)
    {
        considered = true;
        if (neighbors.Count > 0 && !obstacle)
        {
            Node best = neighbors[0];
            float bestCost = Mathf.Infinity;
            foreach (Node neighbor in neighbors)
            {
                if (!neighbor.obstacle) {
                    if (!neighbor.considered)
                    {
                        float cost = currentRoute.cost + (neighbor.transform.position - goal.transform.position).magnitude;
                        if (cost < bestCost)
                        {
                            bestCost = cost;
                            best = neighbor;
                        }
                    }
                    neighbor.considered = true;
                }


            }

            currentRoute.Add(best);

            if (best != goal)
            {
                currentRoute = best.ComputeNextDepth(currentRoute, goal);
            }

            return currentRoute;
        }
        print(name);
        return null;
    }

}

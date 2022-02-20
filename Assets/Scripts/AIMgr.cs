using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMgr : MonoBehaviour
{
    public static AIMgr instance;
    void Awake()
    {
        if (!instance)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }
    }

    private float dragTime = 0;
    public GameObject waypoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void CreateWaypoint(Vector3 pos, Entity ent)
    {
        GameObject wp = Instantiate(waypoint);
        pos.y = 0;
        wp.transform.position = pos;
        Waypoint wayp = wp.transform.GetComponent<Waypoint>();
        wayp.entity = ent;
        ent.unitAI.AddTask(wayp);
        wp.transform.parent = SceneMgr.instance.currentScene.transform;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            dragTime = 0;
        }

        if (Input.GetMouseButton(1))
        {
            dragTime += Time.deltaTime;
        }


        if (Input.GetMouseButtonUp(1) && dragTime < 0.5f)
        {
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                foreach (Entity ent in EntityMgr.instance.selectedEntities)
                {
                    ent.unitAI.Clear();
                    ent.movement.desiredSpeed = 0;
                }
            }

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, float.PositiveInfinity))
            {
                Transform objectHit = hit.transform;
                // make sure not hitting anything selected
                if (EntityMgr.instance.EntitiesInRange(EntityMgr.instance.selectionDistance, hit.point).Count == 0)
                {
                    // make sure something is selected
                    foreach (Entity ent in EntityMgr.instance.selectedEntities)
                    {
                        //CreateWaypoint(ent);

                        if (AStarMgr.instance.active)
                        {
                            Route route = AStarMgr.instance.ComputeRoute(ent.unitAI.GetLastWaypointPosition(), hit.point);
                            //StartCoroutine(AStarMgr.instance.ComputeRoute(ent.unitAI.GetLastWaypointPosition(), hit.point));
                            if (route != null)
                            {
                                foreach (Node node in route.nodes)
                                {
                                    CreateWaypoint(node.transform.position, ent);
                                }
                            }
                        } else
                        {
                            CreateWaypoint(hit.point, ent);
                        }
                    }
                }
            }
        }

/*        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            foreach (Entity ent in EntityMgr.instance.selectedEntities)
            {
                if (ent.unitAI.targets.Count > 0)
                {
                    Waypoint wp = ent.unitAI.targets[ent.unitAI.targets.Count - 1];
                    ent.unitAI.targets.RemoveAt(ent.unitAI.targets.Count - 1);
                    Destroy(wp.gameObject);
                }
            }
        }*/
    }
}

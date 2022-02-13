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

    public GameObject waypoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftAlt))
        {
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
                        GameObject wp = Instantiate(waypoint);
                        wp.transform.position = hit.point;
                        Vector3 pos = wp.transform.position;
                        pos.y = 0;
                        wp.transform.position = pos;
                        Waypoint wayp = wp.transform.GetComponent<Waypoint>();
                        wayp.entity = ent;
                        ent.unitAI.AddTask(wayp);
                    }
                }


                
            }
        }
    }
}

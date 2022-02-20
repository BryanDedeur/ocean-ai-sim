using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMgr : MonoBehaviour
{
    public static EntityMgr instance;
    void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public List<GameObject> entityPrefabs;

    public float selectionDistance = 2;
    public List<Entity> entities;
    public List<Entity> selectedEntities;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Reset()
    {
        entities = new List<Entity>();
        selectedEntities = new List<Entity>();
    }

    float EntityInRange(Entity ent, float range, Vector3 point)
    {
        float distance = (ent.transform.position - point).magnitude;
        if (distance <= range)
        {
            return distance;
        }

        return 0;
    }

    public List<Entity> EntitiesInRange(float range, Vector3 point)
    {
        List<Entity> entInRange = new List<Entity>();
        foreach (Entity ent in entities)
        {
            if (EntityInRange(ent, selectionDistance, point) != 0)
            {
                entInRange.Add(ent);
            }
        }
        return entInRange;
    }

    void ToggleSelectionOnEntity(Entity ent)
    {
        if (ent.selector.IsSelected())
        {
            ent.selector.Deselect();
            selectedEntities.Remove(ent);
        }
        else
        {
            ent.selector.Select();
            selectedEntities.Add(ent);
        }
    }

    void ToggleSelectionOnEntities(List<Entity> entitiesSet)
    {
        foreach (Entity ent in entitiesSet)
        {
            ToggleSelectionOnEntity(ent);
        }
    }

    void DeselectAll()
    {
        foreach (Entity ent in selectedEntities)
        {
            ent.selector.Deselect();
        }
        selectedEntities.Clear();
    }

    void CreateEntity(int id, Vector3 pos)
    {
        GameObject newObj = Instantiate(entityPrefabs[id]);
        newObj.transform.position = pos;
        newObj.transform.parent = SceneMgr.instance.currentScene.transform;
    }

    void DestroyEntity(Entity ent)
    {
        entities.Remove(ent);
        selectedEntities.Remove(ent);
        Destroy(ent.transform.gameObject);
    }

    void DestroySelected()
    {
        while (selectedEntities.Count > 0)
        {
            DestroyEntity(selectedEntities[0]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, float.PositiveInfinity))
            {
                if (Input.GetKey(KeyCode.Alpha1))
                {
                    CreateEntity(0, hit.point);
                } 
                else if (Input.GetKey(KeyCode.Alpha2))
                {
                    CreateEntity(1, hit.point);

                }
                else if (Input.GetKey(KeyCode.Alpha3))
                {
                    CreateEntity(2, hit.point);
                }

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    ToggleSelectionOnEntities(EntitiesInRange(selectionDistance, hit.point));
                }
                else
                {
                    DeselectAll();
                    // Select one entity
                    Entity best = null;
                    float bestDistance = Mathf.Infinity;
                    foreach (Entity ent in entities)
                    {
                        float distance = EntityInRange(ent, selectionDistance, hit.point);
                        if (distance < bestDistance && distance != 0.0f)
                        {
                            bestDistance = distance;
                            best = ent;
                        }
                    }
                    if (best)
                    {
                        ToggleSelectionOnEntity(best);
                    }
                }

            }
        }

        if (Input.GetKey(KeyCode.Delete))
        {
            DestroySelected();
        }

        foreach (Entity ent in selectedEntities)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                ent.orientor.DecreaseDesiredHeading();
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                ent.orientor.IncreaseDesiredHeading();
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                ent.movement.DecreaseDesiredSpeed();
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                ent.movement.IncreaseDesiredSpeed();
            }
        }
    }
}

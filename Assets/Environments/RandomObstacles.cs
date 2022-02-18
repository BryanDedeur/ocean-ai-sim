using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObstacles : MonoBehaviour
{
    public Vector2 corner1;
    public Vector2 corner2;

    public Vector2 minScale;
    public Vector2 maxScale;

    public List<GameObject> objects;
    public List<int> quantities;

    public int objectIndex;
    public int quantitiesIndex;

    private GameObject container;

    // Start is called before the first frame update
    void Awake()
    {
        container = new GameObject();
        container.transform.parent = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {

            quantitiesIndex++;
            if (quantitiesIndex > quantities.Count - 1)
            {
                quantitiesIndex = 0;
                objectIndex++;
                if (objectIndex > objects.Count - 1)
                {
                    objectIndex = 0;
                }
            }

            Destroy(container);
            container = new GameObject();
            container.transform.parent = transform.parent;

            float minx = Mathf.Min(corner1.x, corner2.x);
            float maxx = Mathf.Max(corner1.x, corner2.x);
            float minz = Mathf.Min(corner1.y, corner2.y);
            float maxz = Mathf.Max(corner1.y, corner2.y);
            for (int i = 0; i < quantities[quantitiesIndex]; i++)
            {
                GameObject newObj = Instantiate(objects[objectIndex]);
                newObj.transform.position = new Vector3(Random.Range(minx, maxx),0, Random.Range(minz, maxz));
                newObj.transform.parent = container.transform;
                newObj.transform.localScale = new Vector3(Random.Range(minScale.x, maxScale.x), 1, Random.Range(minScale.y, maxScale.y));
            }
            AStarMgr.instance.Reset();
        }
    }
}

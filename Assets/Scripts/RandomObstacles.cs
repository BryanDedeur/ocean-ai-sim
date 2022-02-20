using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObstacles : MonoBehaviour
{
    public Vector2 corner1;
    public Vector2 corner2;

    private Vector2 scaleRange = new Vector2(5,15);

    public GameObject potentialFieldPrefab;

    public List<GameObject> objects;
    public List<int> quantities;

    public int objectIndex;
    public int quantitiesIndex;

    private GameObject container;

    // Start is called before the first frame update
    void Start()
    {
        container = new GameObject();
        container.transform.parent = SceneMgr.instance.currentScene.transform;
        Generate();
    }

    void Generate()
    {
        Destroy(container);
        container = new GameObject();
        container.transform.parent = SceneMgr.instance.currentScene.transform;

        float minx = Mathf.Min(corner1.x, corner2.x);
        float maxx = Mathf.Max(corner1.x, corner2.x);
        float minz = Mathf.Min(corner1.y, corner2.y);
        float maxz = Mathf.Max(corner1.y, corner2.y);

        float dim = Random.Range(scaleRange.x, scaleRange.y);

        for (int i = 0; i < quantities[quantitiesIndex]; i++)
        {
            GameObject newObj = Instantiate(objects[objectIndex]);
            newObj.transform.position = new Vector3(Random.Range(minx, maxx), 0, Random.Range(minz, maxz));
            newObj.transform.parent = container.transform;
            newObj.transform.localScale = new Vector3(dim, 1, dim);
            dim = Random.Range(scaleRange.x, scaleRange.y);
/*            if (objectIndex == 0)
            {
                GameObject go = Instantiate(potentialFieldPrefab, newObj.transform);
                go.transform.position = newObj.transform.position + new Vector3(dim/2, 0, dim);
            }*/
        }
        StartCoroutine(AStarMgr.instance.Reset());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Generate();
        }
    }
}

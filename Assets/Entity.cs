using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Selector selector;
    public Orientor orientor;
    public Movement movement;
    public UnitAI unitAI;

    public Material focusedMat;
    public Material unfocusedMat;

    // Start is called before the first frame update
    void Awake()
    {
        selector = transform.GetComponent<Selector>();
        orientor = transform.GetComponent<Orientor>();
        movement = transform.GetComponent<Movement>();
        unitAI = transform.GetComponent<UnitAI>();
    }

    private void Start()
    {
        EntityMgr.instance.entities.Add(this);
        if (selector.IsSelected())
        {
            EntityMgr.instance.selectedEntities.Add(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

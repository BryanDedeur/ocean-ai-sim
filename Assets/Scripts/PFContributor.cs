using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PFContributor : MonoBehaviour {

    public Vector3 potentialVector;
    public float potentialForceExperienced;

    private Entity entity;

    private void Awake()
    {
        entity = GetComponent<Entity>();
    }

    // Use this for initialization
    void Start()
    {
        PFMgr.instance.TrackPFContributor(this);
    }

    private void Update()
    {
        PFMgr.instance.CalculatePotentialAtPos(this);
        entity.orientor.directionVector += potentialVector;
    }

}
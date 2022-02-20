using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PFContributor : MonoBehaviour {

    public Vector3 potentialVector;
    public float potentialForceExperienced;
    private GameObject potentialFieldVisual;

    private bool active;

    [System.Serializable]
    public class PotentialEntry
    {
        public float constantC;
        public float constantE;
    }

    public List<PotentialEntry> constantPairs;

    private Entity entity;

    public void SetActiveState(bool state)
    {
        active = state;
        potentialFieldVisual.SetActive(active);
    }

    private void Awake()
    {
        entity = GetComponent<Entity>();

        potentialFieldVisual = Instantiate(PFMgr.instance.potentialFieldVisual);
        potentialFieldVisual.transform.position = transform.position - new Vector3(0, 0.05f, 0);
        potentialFieldVisual.transform.parent = transform;
        float maxX = 0;
        foreach (PotentialEntry pair in constantPairs)
        {
            float temp = -pair.constantE / pair.constantC;
            if (temp > maxX)
            {
                maxX = temp;
            }
        }
        potentialFieldVisual.transform.localScale = new Vector3(maxX, 0, maxX);
        SetActiveState(PFMgr.instance.active);
    }

    private void OnDestroy()
    {
        PFMgr.instance.UntrackPFContributor(this);
    }

    // Use this for initialization
    void Start()
    {
        PFMgr.instance.TrackPFContributor(this);
    }

    private void Update()
    {
        if (entity)
        {
            if (active)
            {
                PFMgr.instance.CalculatePotentialAtPos(this);
                entity.orientor.directionVector += potentialVector;
            }
        }

    }

}
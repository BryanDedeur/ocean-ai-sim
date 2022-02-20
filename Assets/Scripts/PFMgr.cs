using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PFMgr : MonoBehaviour {

    public static PFMgr instance;
    public List<PFContributor> contributors;
    public GameObject potentialFieldVisual;

    public void Reset()
    {
        contributors = new List<PFContributor>();
    }

    public bool active;

    public void Toggle(bool state)
    {
        active = state;
        SetActiveState(active);
    }
    void SetActiveState(bool state)
    {
        foreach(PFContributor contributor in contributors)
        {
            contributor.SetActiveState(state);
        }
    }

    // using formula cd^e where d = 1/distance and c and e are tunable

    // Use this for initialization
    void Awake () {
        contributors = new List<PFContributor>();
        instance = this;
    }

    public void TrackPFContributor(PFContributor pf)
    {
        contributors.Add(pf);
    }

    public void UntrackPFContributor(PFContributor pf)
    {
        contributors.Remove(pf);
    }

    private PFContributor locatePF(PFContributor pf)
    {
        if (pf != null)
        {
            PFContributor pfComponent = pf.GetComponent<PFContributor>();
            if (pfComponent != null)
            {
                return pfComponent;
            }
        }

        return null;
    }

    // ------------------- POTENTIAL FIELD CALCULATIONS --------------------- //


    // Calculates force PRODUCED by the position of interest (not sum of all potential emitters)
    private float CalculateIndividualForce(PFContributor contributor, float distance)
    {
        float result = 0;
        if (distance > 0)
        {
            for(int i = 0; i < contributor.constantPairs.Count; i++)
            {
                //distance += constantPairs[i].constantC * Mathf.Pow(distance, constantPairs[i].constantE);
                float temp = contributor.constantPairs[i].constantC * distance + contributor.constantPairs[i].constantE;
                if (temp < 0)
                {
                    continue;
                }
                result += temp;
            }
        }
        return result;
    }

    // Calculates sum of force focusEntity EXPERIENCED by all other entities
    public void CalculatePotentialAtPos(PFContributor experiencer)
    {
        Vector3 resultVec = Vector3.zero;
        float sumForce = 0;
        foreach (var contributor in contributors)
        {
            // Skip the caculation is the current focus
            if (experiencer == contributor)
            {
                continue;
            }

            Vector3 diff = (experiencer.transform.position - contributor.transform.position);
            float individualForce = CalculateIndividualForce(contributor, diff.magnitude);
            sumForce += individualForce;
            resultVec += diff.normalized * individualForce;

        }

        experiencer.potentialForceExperienced = sumForce;
        experiencer.potentialVector = resultVec;
    }

}

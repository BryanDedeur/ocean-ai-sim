using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PFMgr : MonoBehaviour {

    public static PFMgr instance;
    public List<PFContributor> contributors;

    [System.Serializable]
    public class PotentialEntry
    {
        public float constantC;
        public float constantE;
    }

    public List<PotentialEntry> constantPairs;


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
    private float CalculateIndividualForce(Vector3 diff)
    {
        // force is same as 'd' in 'cd^e', ignore if 0
        float distance = diff.magnitude;
        float result = 0;
        if (distance > 0)
        {
            for(int i = 0; i < constantPairs.Count; i++)
            {
                //distance += constantPairs[i].constantC * Mathf.Pow(distance, constantPairs[i].constantE);
                float temp = constantPairs[i].constantC * distance + constantPairs[i].constantE;
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
            float individualForce = CalculateIndividualForce(diff);
            sumForce += individualForce;
            resultVec += diff.normalized * individualForce;

        }

        experiencer.potentialForceExperienced = sumForce;
        experiencer.potentialVector = resultVec;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    private Entity entity;
    private const float turnRate = 15;
    private Transform selectionIndicator;
    private float degrees;

    // Start is called before the first frame update
    void Awake()
    {
        selectionIndicator = transform.Find("SelectionIndicator");
        entity = transform.GetComponent<Entity>();
    }
    public bool IsSelected()
    {
        return selectionIndicator.gameObject.activeSelf;
    }

    public void Select()
    {
        selectionIndicator.gameObject.SetActive(true);
        entity.unitAI.SetFocus(true);
        entity.orientor.SetFocus(true);
    }

    public void Deselect()
    {
        selectionIndicator.gameObject.SetActive(false);
        entity.unitAI.SetFocus(false);
        entity.orientor.SetFocus(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (IsSelected())
        {
            degrees += turnRate * Time.deltaTime;
            if (degrees > 360)
            {
                degrees = 0;
            }
            selectionIndicator.eulerAngles = new Vector3(0, degrees,0);
        }
    }
}

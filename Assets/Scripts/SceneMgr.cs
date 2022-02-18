using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMgr : MonoBehaviour
{
    public static SceneMgr instance;

    public List<GameObject> scenes;

    public GameObject currentScene;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public void LoadScene(int sceneId)
    {
        Destroy(currentScene);
        currentScene = Instantiate(scenes[sceneId]);
        EntityMgr.instance.Reset();
        PFMgr.instance.Reset();
        AStarMgr.instance.Reset();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            LoadScene(0);
        }else if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            LoadScene(1);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            LoadScene(2);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            LoadScene(3);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            LoadScene(4);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            LoadScene(5);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            LoadScene(6);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            LoadScene(7);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            LoadScene(8);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public List<GameObject> Warriors = new List<GameObject>();
    private Vector2[] startingPositions;
    private CinemachineVirtualCamera vcam;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("GM").AddComponent<GameManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);

        startingPositions = new Vector2[]
        {
            new Vector2(-1.5f, 0.5f),
            new Vector2(-0.5f, -1.5f),
            new Vector2(-1.5f, -1.5f),
            new Vector2(-0.5f, 0.5f),
            new Vector2(-1f, -0.5f)
        };
    }

    private void Start()
    {
        for (int i = 0; i < Warriors.Count; i++)
        {
            Warriors[i].SetActive(true);
        }
    }

    public void desactivateWarrior(GameObject warrior)
    {
        for (int i = 0; i < Warriors.Count; i++)
        {
            if (Warriors[i].name == warrior.name)
            {
                Warriors.RemoveAt(i);
                Destroy(warrior);
            }
        }
    }

    public void setCamera(GameObject optionB)
    {
        vcam = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
        GameObject main = GameObject.Find("Alistair");
        if (main != null)
            vcam.Follow = main.transform;
        else
            vcam.Follow = optionB.transform;
    }

    public GameObject[] getWarriors()
    {
        return Warriors.ToArray();
    }

    public Vector2[] getPositions()
    {
        return startingPositions;
    }



}

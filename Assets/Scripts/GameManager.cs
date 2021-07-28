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
    private List<GameObject> WarriorsTemp;
    private Vector2[] startingPositions;
    private CinemachineVirtualCamera vcam;
    private string warriorName;
    private int difficulty;
    private bool defeat;

    AudioSource defeatAS, battleAS, menuAS, hitAS, winAS;
    public AudioClip defeatSound;
    public AudioClip winSound;
    public AudioClip battleSound;
    public AudioClip menuSound;
    public AudioClip hitSound;

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

    public int Difficulty { get => difficulty; set => difficulty = value; }
    public string WarriorName { get => warriorName; set => warriorName = value; }

    public bool Defeat { get => defeat; set => defeat = value; }

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

        defeatAS = gameObject.AddComponent<AudioSource>();
        menuAS = gameObject.AddComponent<AudioSource>();
        battleAS = gameObject.AddComponent<AudioSource>();
        hitAS = gameObject.AddComponent<AudioSource>();
        winAS = gameObject.AddComponent<AudioSource>();

        defeatAS.clip = defeatSound;
        menuAS.clip = menuSound;
        battleAS.clip = battleSound;
        hitAS.clip = hitSound;
        winAS.clip = winSound;
    }

    private void Start()
    {
        WarriorsTemp = new List<GameObject>();

        for (int i = 0; i < Warriors.Count; i++)
        {
            WarriorsTemp.Add(Warriors[i]);
            WarriorsTemp[i].SetActive(true);
        }
        defeat = false;
    }

    public void desactivateWarrior(GameObject warrior)
    {
        Debug.Log(warrior.name);
        for (int i = 0; i < WarriorsTemp.Count; i++)
        {
            if (WarriorsTemp[i].name == warrior.name)
            {
                WarriorsTemp.RemoveAt(i);
                Destroy(warrior);
                if (warrior.name == warriorName)
                {
                    playDefeatSong();
                    defeat = true;
                }
            }
        }
    }

    public void setCamera(GameObject optionB)
    {
        vcam = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
        GameObject main = GameObject.Find(warriorName);
        if (main != null)
            vcam.Follow = main.transform;
        else
            vcam.Follow = optionB.transform;
    }

    public GameObject[] getWarriors()
    {
        return WarriorsTemp.ToArray();
    }

    public Vector2[] getPositions()
    {
        return startingPositions;
    }

    public void startAgainManager()
    {
        battleAS.Stop();
        Start();
        SceneManager.LoadScene("Menu");
    }

    public void playMenuSong() {
        menuAS.loop = true;
        menuAS.Play();
        menuAS.volume = 0.1f;
    }

    public void playBattleSong() {
        menuAS.Stop();
        battleAS.loop = true;
        battleAS.Play();
        battleAS.volume = 0.1f;
    }

    public void playDefeatSong() {
        defeatAS.loop = false;
        defeatAS.Play(); 
    }

    public void playWinSong()
    {
        winAS.loop = false;
        winAS.Play();
    }

    public void playHitSong()
    {
        hitAS.volume = 0.15f;
        hitAS.loop = false;
        hitAS.Play();
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Fase1 : MonoBehaviour
{
    public GameObject tutorial;
    private GameObject[] Slimes;
    private GameObject[] Warriors;
    private List<GameObject> PrefabWarriors = new List<GameObject>();
    private GameObject tempWarrior; 
    private GameObject lostWarrior;
    private Vector2[] startingPositions;
    private bool verify = true;
    private TMP_Text loserText;
    private TMP_Text scoreText;
    private float countdown = 10.0f;
    private string loserName;
    private GameObject defeat;

    void Start()
    {
        startingPositions = GameManager.Instance.getPositions();
        Warriors = GameManager.Instance.getWarriors();

        for (int i = 0; i < Warriors.Length; i++)
        {
            tempWarrior = Instantiate<GameObject>(Warriors[i], startingPositions[i], Quaternion.identity, GameObject.Find("Warriors").transform);
            tempWarrior.name = Warriors[i].name;
            tempWarrior.AddComponent<WarriorFase1>();
            scoreText = GameObject.Find(Warriors[i].name + "T").GetComponent<TMP_Text>();
            tempWarrior.GetComponent<WarriorFase1>().setScoreText(scoreText);
            PrefabWarriors.Add(tempWarrior);
        }

        GameManager.Instance.setCamera(PrefabWarriors[0]);
        lostWarrior = PrefabWarriors[0];
        loserName = PrefabWarriors[0].name;
        loserText = GameObject.Find("LoserText").GetComponent<TMP_Text>();

        defeat = GameObject.Find("Defeat");
        if (GameManager.Instance.Defeat == false) 
            defeat.SetActive(false);
        GameManager.Instance.playBattleSong();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            tutorial.SetActive(false);
            foreach (GameObject warrior in PrefabWarriors)
                if (warrior != null)
                    warrior.GetComponent<WarriorFase1>().StartGame = true;
        }

            
        Slimes = GameObject.FindGameObjectsWithTag("Slime");

        if(Slimes.Length == 0 && verify)
        { 
            
            foreach (GameObject warrior in PrefabWarriors)
            {
                if (lostWarrior.GetComponent<WarriorFase1>().getScore() > 
                    warrior.GetComponent<WarriorFase1>().getScore())
                {
                    lostWarrior = warrior;
                }
            }
            verify = false;
            loserName = lostWarrior.name;
            GameManager.Instance.desactivateWarrior(lostWarrior);
            if (loserName == GameManager.Instance.WarriorName)
                defeat.SetActive(true);
        }

        if(countdown == 0)
        {
            SceneManager.LoadScene("Fase 2");
        }

        if (!verify)
        {
            countdown = Mathf.Clamp(countdown - Time.deltaTime, 0, Mathf.Infinity);
            loserText.text = loserName + " foi o perdedor da rodada\nComešando nova fase em " 
                + Mathf.CeilToInt(countdown) + " segundos";
        }
    }

    public void startAgain() { GameManager.Instance.startAgainManager(); }
}

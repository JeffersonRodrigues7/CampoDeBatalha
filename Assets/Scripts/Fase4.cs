using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Fase4 : MonoBehaviour
{
    public GameObject tutorial;
    GameObject Skeletons;
    private GameObject[] Warriors;
    private List<GameObject> PrefabWarriors = new List<GameObject>();
    private GameObject tempWarrior;
    private GameObject lostWarrior;
    private GameObject winner;
    private Vector2[] startingPositions;
    private bool verify = true;
    private TMP_Text loserText;
    private TMP_Text helthText;
    private float countdown = 10.0f;
    private string loserName;
    private GameObject defeat;

    public TMP_Text timer;
    private float timerValue = 60.0f;

    void Start()
    {
        Skeletons = GameObject.Find("Skeletons");
        startingPositions = GameManager.Instance.getPositions();
        Warriors = GameManager.Instance.getWarriors();

        for (int i = 0; i < Warriors.Length; i++)
        {
            tempWarrior = Instantiate<GameObject>(Warriors[i], startingPositions[i], Quaternion.identity, GameObject.Find("Warriors").transform);
            tempWarrior.name = Warriors[i].name;
            tempWarrior.AddComponent<WarriorFase4>();
            helthText = GameObject.Find(Warriors[i].name + "T").GetComponent<TMP_Text>();
            tempWarrior.GetComponent<WarriorFase4>().setScoreText(helthText);
            PrefabWarriors.Add(tempWarrior);
        }

        GameManager.Instance.setCamera(PrefabWarriors[0]);
        lostWarrior = PrefabWarriors[0];
        winner = PrefabWarriors[1];
        loserName = PrefabWarriors[0].name;
        loserText = GameObject.Find("LoserText").GetComponent<TMP_Text>();

        countdown = 10.0f;
        defeat = GameObject.Find("Defeat");
        if (GameManager.Instance.Defeat == false)
            defeat.SetActive(false);

    }

    void Update()
    {
        if ((GameManager.Instance.Defeat || Input.GetKeyDown(KeyCode.C)) && verify)
        {
            tutorial.SetActive(false);

            foreach (GameObject warrior in PrefabWarriors)
                if (warrior != null)
                    warrior.GetComponent<WarriorFase4>().StartGame = true;

            Skeletons.GetComponent<CreatingSkeletons>().CreateSkeleton = true;
        }

        timer.text = "Tempo Restante: " + Mathf.CeilToInt(timerValue) + " segundos";
        timerValue = Mathf.Clamp(timerValue - Time.deltaTime, 0, Mathf.Infinity);

        if (timerValue == 0 && verify)
        {
            foreach (GameObject warrior in PrefabWarriors)
            {
                if (lostWarrior.GetComponent<WarriorFase4>().getScore() > warrior.GetComponent<WarriorFase4>().getScore())
                {
                    winner = lostWarrior;
                    lostWarrior = warrior;
                }
            }
            verify = false;
            loserName = lostWarrior.name;
            GameManager.Instance.desactivateWarrior(lostWarrior);

            Skeletons.GetComponent<CreatingSkeletons>().CreateSkeleton = false;

            if (loserName == GameManager.Instance.WarriorName)
                defeat.SetActive(true);

            if(winner.name == GameManager.Instance.WarriorName) { GameManager.Instance.playWinSong(); }

        }

        if (countdown == 0)
        {
            GameManager.Instance.startAgainManager();
        }

        if (!verify)
        {
            countdown = Mathf.Clamp(countdown - Time.deltaTime, 0, Mathf.Infinity);
            loserText.text = loserName + " foi o perdedor da rodada\nComeçando nova fase em " + Mathf.CeilToInt(countdown) + " segundos";
        }
    }

    public void startAgain() { GameManager.Instance.startAgainManager(); }

}

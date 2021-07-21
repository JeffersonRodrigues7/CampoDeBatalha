using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Fase2 : MonoBehaviour
{
    private GameObject[] Warriors;
    private List<GameObject> PrefabWarriors = new List<GameObject>();
    private GameObject tempWarrior;
    private GameObject lostWarrior;
    private Vector2[] startingPositions;
    private bool verify = true;
    private TMP_Text loserText;
    private TMP_Text helthText;
    private float countdown = 10.0f;
    private string loserName;
    bool end = false;//vai impedir que dois guerreiros sejam eliminados na rodada

    void Start()
    {
        startingPositions = GameManager.Instance.getPositions();
        Warriors = GameManager.Instance.getWarriors();

        for (int i = 0; i < Warriors.Length; i++)
        {
            tempWarrior = Instantiate<GameObject>(Warriors[i], startingPositions[i], Quaternion.identity, GameObject.Find("Warriors").transform);
            tempWarrior.name = Warriors[i].name;
            tempWarrior.AddComponent<WarriorFase2>();
            helthText = GameObject.Find(Warriors[i].name + "T").GetComponent<TMP_Text>();
            tempWarrior.GetComponent<WarriorFase2>().setHelthText(helthText);
            PrefabWarriors.Add(tempWarrior);
        }

        GameManager.Instance.setCamera(PrefabWarriors[0]);
        lostWarrior = PrefabWarriors[0];
        loserName = PrefabWarriors[0].name;
        loserText = GameObject.Find("LoserText").GetComponent<TMP_Text>();

        countdown = 10.0f;
    }

    void Update()
    {

        foreach (GameObject warrior in PrefabWarriors)
        {
            if (warrior != null && warrior.GetComponent<WarriorFase2>().getHealth() <= 0 && !end)
            {
                end = true;
                loserName = warrior.name;

                lostWarrior = warrior;
                GameManager.Instance.desactivateWarrior(lostWarrior);
                verify = false;

                GameObject[] Skeletons = GameObject.FindGameObjectsWithTag("Skeleton");
                foreach(GameObject skeleton in Skeletons)
                    skeleton.GetComponent<Skeleton>().setAttack(false);
            }
        }
        
        if (countdown == 0)
        {
            SceneManager.LoadScene("Fase 3");
        }

        if (!verify)
        {
            countdown = Mathf.Clamp(countdown - Time.deltaTime, 0, Mathf.Infinity);
            loserText.text = loserName + " foi o perdedor da rodada\nComeçando nova fase em " 
                + Mathf.CeilToInt(countdown) + " segundos";
        }
    }
}

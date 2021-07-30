using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class FaseFinal : MonoBehaviour
{
    private Vector2 position;
    private GameObject[] Warriors;
    private GameObject winner;
    private float countdown;
    public TMP_Text ballonText;
    private bool faderControl;
    public TMP_Text credits;
    public GameObject creditsObject;
    private GameObject defeat;
    Fader fader;

    void Start()
    {
        position = new Vector2(-0.78f, -3.43f);
        Warriors = GameManager.Instance.getWarriors();
        winner = Instantiate<GameObject>(Warriors[0], position, Quaternion.identity, GameObject.Find("Warriors").transform);
        winner.name = Warriors[0].name;
        winner.AddComponent<WarriorFaseAward>();
        countdown = 10.0f;
        fader = FindObjectOfType<Fader>();
        faderControl = true;
        GameManager.Instance.playEndSong();
        ballonText.text = "Renly: Parabéns " + winner.name + ", você foi o grande campeão do nosso torneio e agora faz parte da guarda real!";

        defeat = GameObject.Find("Defeat");
        if (GameManager.Instance.Defeat == false)
            defeat.SetActive(false);

        creditsObject = GameObject.Find("Credits");
    }

    void Update()
    {
        if (countdown == 0 && faderControl)
        {
            if (GameManager.Instance.Defeat == true)
                defeat.SetActive(false);

            creditsObject.SetActive(false);
            faderControl = false;
            fader.FadeToScene("Creditos", 10.0f);
        }
        credits.text = "Indo para os creditos em: " + Mathf.CeilToInt(countdown) + " segundos";
        countdown = Mathf.Clamp(countdown - Time.deltaTime, 0, Mathf.Infinity);
    }

    public void startAgain() { GameManager.Instance.startAgainManager(); }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    GameObject warriorChoice;
    GameObject difficultyChoice;
    GameObject credits;
    public TMP_Text startText;
    private float countdown;
    private bool start;


    void Start()
    {
        warriorChoice = GameObject.Find("EscolherGuerreiro");
        difficultyChoice = GameObject.Find("EscolherDificuldade");
        credits = GameObject.Find("Creditos");
        difficultyChoice.SetActive(false);
        countdown = 1.0f;
        start = false;
        GameManager.Instance.playMenuSong();
    }

    void Update()
    {
        if (countdown == 0)
        {
            SceneManager.LoadScene("Fase 1");
        }

        if (start)
        {
            countdown = Mathf.Clamp(countdown - Time.deltaTime, 0, Mathf.Infinity);
            startText.text = "O jogo vai começar em "+ Mathf.CeilToInt(countdown) + " segundos";
        }
    }

    public void OnClicked(Button button)
    {
        GameManager.Instance.WarriorName = button.name;
        warriorChoice.SetActive(false);
        difficultyChoice.SetActive(true);
    }

    public void OnClicked(int difficulty)
    {
        GameManager.Instance.Difficulty = difficulty;
        difficultyChoice.SetActive(false);
        credits.SetActive(false);
        start = true;
    }

    public void openCredits() { SceneManager.LoadScene("Creditos"); }
}

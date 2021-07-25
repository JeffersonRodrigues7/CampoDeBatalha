using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatingRocks : MonoBehaviour
{
    public GameObject rock;
    float horizontal;
    float vertical;
    Vector2 lookDirection;
    float cooldownTime;
    GameObject warrior;
    float x, y;
    float speedMod;
    float speedModAcres;//as bolinhas ficam mais rápidas com o tempo

    bool launch;//Vai ser desablito no script Fase2

    private List<GameObject> WarriorsList;
    private string warriorName;
    private int difficulty;
    private int rep;

    void Start()
    {
        warriorName = GameManager.Instance.WarriorName;
        difficulty = GameManager.Instance.Difficulty;
        WarriorsList = new List<GameObject>();
        lookDirection = new Vector2(1, 0);       
        cooldownTime = 0.0f;
        speedMod = 0.0f;
        launch = true;

        if (difficulty == 1)
        {
            speedMod = 0.0f;
            speedModAcres = 0.005f;
            rep = 1;
        }
        if (difficulty == 2)
        {
            speedMod = 0.0f;
            speedModAcres = 0.01f;
            rep = 3;
        }
        if (difficulty == 3)
        {
            speedMod = 0.1f;
            speedModAcres = 0.01f;
            rep = 5;
        }
    }

    private void FixedUpdate()
    {
        if (launch)
        {
            if (cooldownTime == 0)
            {
                foreach (GameObject warrior in GameObject.FindGameObjectsWithTag("Player"))
                    WarriorsList.Add(warrior);


                warrior = GameObject.Find(warriorName);
                if(warrior != null)
                    for (int r = 0; r < rep; r++)
                        WarriorsList.Add(warrior);


                warrior = shuffle(WarriorsList.ToArray());//retorna um guerreiro aleatório do campo para atacar

                throwRock(warrior);
                cooldownTime = 0.5f;
            }

            cooldownTime = Mathf.Clamp(cooldownTime - Time.fixedDeltaTime, 0, Mathf.Infinity);
        }
    }

    private GameObject shuffle(GameObject[] warriors)
    {
        // Knuth shuffle algorithm
        for (int t = 0; t < warriors.Length; t++)
        {
            GameObject tmp = warriors[t];
            int r = Random.Range(t, warriors.Length);
            warriors[t] = warriors[r];
            warriors[r] = tmp;
        }

        return warriors[Random.Range(0, warriors.Length)];//retorna um guerreiro aleatório dentro do array embaralhado
    }

    void throwRock(GameObject warrior)
    {
        bool rockH;//Se True pedra está viajando na horizontal se false está na vertical
        float speedX = 0.0f;
        float speedY = 0.0f;
        x = warrior.transform.position.x;
        y = warrior.transform.position.y;
        int rD = Random.Range(0, 2);//Direção da pedra
        int rS = Random.Range(0, 2);//Sentido da pedra

        if (rD == 0 && rS == 0)//Horizontal para direita
        {
            rockH = true;
            x = -7.40f;
            speedX = -0.5f - speedMod;
        }
        else if(rD == 0 && rS == 1)//Horizontal para esquerda
        {
            rockH = true;
            x = 5.45f;
            speedX = -0.5f - speedMod;
        }
        else if (rD == 1 && rS == 0)//Vertical para baixo
        {
            rockH = false;
            y = 4.25f;
            speedY = -0.5f - speedMod;
        }
        else//Vertical para cima
        {
            rockH = false;
            y = -5.9f;
            speedY = -0.5f - speedMod;
        }

        Vector2 pos = new Vector2(x, y);//Posição inicial da pedra
        GameObject obj = Instantiate<GameObject>(rock, pos, Quaternion.identity, transform);
        obj.name = "Rock";
        obj.GetComponent<Rock>().setRockH(rockH);

        Rigidbody2D rigidbody2d = obj.GetComponent<Rigidbody2D>();
        rigidbody2d.velocity = new Vector2(x * speedX, y * speedY);
        speedMod += speedModAcres;
    }

    public void launchRock(bool option) { launch = option; }
}

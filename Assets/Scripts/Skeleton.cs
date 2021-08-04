using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    Animator animator;

    float speed;//velocidade do Skeleto
    float velMod;
    Rigidbody2D rigidbody2d;
    bool hit;
    float cooldownTime;
    bool attack;//Vai ser desabilitado no script Fase2

    float horizontal;
    float vertical;
    Vector2 lookDirection;

    private List<GameObject> WarriorsList;
    private GameObject warrior;

    private string warriorName;
    private int difficulty;
    private float speedMod;
    private int rep;


    private void Start()
    {
        warriorName = GameManager.Instance.WarriorName;
        difficulty = GameManager.Instance.Difficulty;
        WarriorsList = new List<GameObject>();
        lookDirection = new Vector2(1, 0);
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        speed = 3.0f;
        velMod = 0.0f;
        cooldownTime = 0.0f;
        attack = false;

        if (difficulty == 1)
        {
            speedMod = 0.0f;
            rep = 1;
        }
        if (difficulty == 2)
        {
            speedMod = 0.5f;
            rep = 2;
        }
        if (difficulty == 3)
        {
            speedMod = 1.0f;
            rep = 3;
        }
    }

    void FixedUpdate()
    {
        if (attack)
        {
            animator.SetTrigger("Attack01");

            if (cooldownTime == 0)
            {
                foreach (GameObject warrior in GameObject.FindGameObjectsWithTag("Player"))
                    WarriorsList.Add(warrior);

                warrior = GameObject.Find(warriorName);
                if(warrior != null)
                    for (int r = 0; r < rep; r++)
                        WarriorsList.Add(warrior);

                warrior = shuffle(WarriorsList.ToArray());//retorna um guerreiro aleatório do campo para atacar
                if (warrior != null && warrior.name == warriorName) //Skeleto fica mais rapido quando está seguindo o jogador 
                    velMod = speedMod;
                else
                    velMod = 0.0f;

                cooldownTime = 4f;
            }

            move(warrior);
        }
    }

    private void move(GameObject warrior)
    {
        lookDirection = (warrior.transform.position - transform.position).normalized;//Trava a visão do player bem no centro do Slime, assim o raio n muda a medida que o player se aproxima, preciso normalizar para multiplicar pelo speed
        animator.SetFloat("Move X", lookDirection.x);
        animator.SetFloat("Move Y", lookDirection.y);
        animator.SetFloat("Speed", lookDirection.magnitude);

        rigidbody2d.velocity = new Vector2(lookDirection.x * (speed + velMod), lookDirection.y * (speed + velMod));
        cooldownTime = Mathf.Clamp(cooldownTime - Time.fixedDeltaTime, 0, Mathf.Infinity);
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

    void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player" && hit)
        {
                WarriorFase2 warrior = other.collider.GetComponent<WarriorFase2>();
                if (warrior != null) warrior.changeHealth(-1);  
        }
    }

    public void setAttack(bool option) { attack = option; }
    public void attackStart() { hit = true; }
    public void attackEnd() { hit = false; }

}

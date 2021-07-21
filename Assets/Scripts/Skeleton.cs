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
    bool attack;//Vai ser desablito no script Fase2

    float horizontal;
    float vertical;
    Vector2 lookDirection;

    private List<GameObject> WarriorsList;
    private GameObject warrior;


    private void Start()
    {
        WarriorsList = new List<GameObject>();
        lookDirection = new Vector2(1, 0);
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        speed = 3.0f;
        velMod = 0.0f;
        cooldownTime = 0.0f;
        attack = true;
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

                if (true)//Modo dificil, aumenta as chances do jogador ser o escolhido para atacar
                {
                    warrior = GameObject.Find("Alistair");
                    if(warrior != null)
                        for (int r = 0; r < 3; r++)
                            WarriorsList.Add(warrior);
                }

                warrior = shuffle(WarriorsList.ToArray());//retorna um guerreiro aleat�rio do campo para atacar
                if (warrior != null && warrior.name == "Alistair") //Skeleto fica mais rapido quando est� seguindo o jogador 
                    velMod = 0.1f;
                else
                    velMod = 0.0f;

                cooldownTime = 4f;
            }

            move(warrior);
        }
    }

    private void move(GameObject warrior)
    {
        lookDirection = (warrior.transform.position - transform.position).normalized;//Trava a vis�o do player bem no centro do Slime, assim o raio n muda a medida que o player se aproxima, preciso normalizar para multiplicar pelo speed
        animator.SetFloat("Move X", lookDirection.x);
        animator.SetFloat("Move Y", lookDirection.y);
        animator.SetFloat("Speed", lookDirection.magnitude);

        if (lookDirection.x < 0) lookDirection.x -= velMod;
        else lookDirection.x += velMod;

        if (lookDirection.y < 0) lookDirection.y -= velMod;
        else lookDirection.y += velMod;

        rigidbody2d.velocity = new Vector2(lookDirection.x * speed, lookDirection.y * speed);
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

        return warriors[Random.Range(0, warriors.Length)];//retorna um guerreiro aleat�rio dentro do array embaralhado
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

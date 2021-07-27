using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarriorFase3 : MonoBehaviour
{
    Animator animator;

    float speed;
    Rigidbody2D rigidbody2d;

    float horizontal;
    float vertical;
    Vector2 lookDirection;

    public TMP_Text healthText;
    int health;

    float lookDistance;
    float cooldownTime;//Em quanto em quanto tempo ele vai poder renovar a remessa de pedras

    private GameObject[] Rocks;
    private GameObject Rock;
    float distaceMin;

    private GameObject[] Spots;
    private GameObject spot;

    bool newWave;//Essa variável vai indicar se o Player vai poder verificar uma nova remessa de pedras que estão no cenário

    private string warriorName;
    private int difficulty;
    private float speedMod;

    private void Awake()
    {
        health = 10;
    }

    void Start()
    {
        warriorName = GameManager.Instance.WarriorName;
        difficulty = GameManager.Instance.Difficulty;
        lookDirection = new Vector2(1, 0);
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        cooldownTime = 0.0f;
        healthText.text = gameObject.name + ": " + health;
        distaceMin = 2.5f;
        speed = 3.0f;
        Spots = GameObject.FindGameObjectsWithTag("Spot");

        GameObject[] Warriors = GameObject.FindGameObjectsWithTag("Player");

        if (difficulty == 1)
        {
            speedMod = 0.5f;
        }
        if (difficulty == 2)
        {
            speedMod = 1.0f;
        }
        if (difficulty == 3)
        {
            speedMod = 1.5f;
        }
    }

    void Update()
    {
        float px = transform.position.x;
        float py = transform.position.y;

        if (gameObject.name == warriorName) //Humano 
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            Vector2 move = new Vector2(horizontal, vertical);

            if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
            {
                lookDirection.Set(move.x, move.y);
                lookDirection.Normalize();
            }

            animator.SetFloat("Move X", lookDirection.x);
            animator.SetFloat("Move Y", lookDirection.y);
            animator.SetFloat("Speed", move.magnitude);

            if (Input.GetKeyDown(KeyCode.C))
            {
                GameManager.Instance.playHitSong();
                animator.SetTrigger("Attack01");
            }
        }
    }

    private void FixedUpdate()
    {
        float px = transform.position.x;
        float py = transform.position.y;

        if (gameObject.name == warriorName) //Humano 
        {
            Vector2 position = rigidbody2d.position;
            position.x = position.x + speed * horizontal * Time.deltaTime;
            position.y = position.y + speed * vertical * Time.deltaTime;

            rigidbody2d.MovePosition(position);
        }

        else if(cooldownTime == 0)//Máquina
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, 1.5f, LayerMask.GetMask("Rock"));

            if (colliders.Length > 0)
            {
                foreach (Collider2D collider in colliders)
                    verifyPosition(collider.gameObject);
            }

            else
            {
                rigidbody2d.velocity = new Vector2(0, 0);
                animator.SetFloat("Move X", lookDirection.x);
                animator.SetFloat("Move Y", lookDirection.y);
                animator.SetFloat("Speed", 0f);
            }

            cooldownTime = 0.1f;
        }

        cooldownTime = Mathf.Clamp(cooldownTime - Time.fixedDeltaTime, 0, Mathf.Infinity);
    }

    private void verifyPosition(GameObject rock)//Método que vai fazer o Player desviar das pedras
    {
        //Posição do player em x e y atualmente
        float cPx = transform.position.x;
        float cPy = transform.position.y;
        bool move;//Se vai precisar se mover
        float cooldownTimeQ = 0.1f;//Tempo que ele terá pra se mover para cada pedra
        bool rockH = rock.GetComponent<Rock>().getRockH();

        if (rock != null)
        {
            move = false;
            float newY = cPy;
            float newX = cPx;

            float Rx = rock.transform.position.x;//posição em x da pedra atualmente
            float Ry = rock.transform.position.y;//posição em x da pedra atualmente

            Vector2 difference = transform.position - rock.transform.position;
            var distanceInX = Mathf.Abs(difference.x);
            var distanceInY = Mathf.Abs(difference.y);

            if (!rockH)//Verifica se o player vai ser acertado em x pela pedra
            {
                move = true;
                float newXE = cPx - (distaceMin - distanceInX);
                float newXD = cPx + (distaceMin - distanceInX);
                if (newXD >= 4.0f)//Se ir pra esquerda for mais benéfico(Verifica se está  muito perto da parede)
                    newX = newXE;
                else if (newXE <= -6.0f)//Se ir pra direita for mais benéfico(Verifica se está  muito perto da parede)
                    newX = newXD;
                else if (Rx - cPx >= 0)//Se ir pra esquerda for mais benéfico
                    newX = newXE;
                else//Se ir pra direita for mais benéfico
                    newX = newXD;
            }
            
            if (rockH)//Verifica se o player vai ser acertado em y pela pedra
            {
                move = true;
                float newYB = cPy - (distaceMin - distanceInY);
                float newYC = cPy + (distaceMin - distanceInY);
                if (newYC >= 3.0f)//Se ir pra baixo for mais benéfico(Verifica se está  muito perto da parede)
                    newY = newYB;
                else if (newYB <= -5.0f)//Se ir pra cima for mais benéfico(Verifica se está  muito perto da parede)
                    newY = newYC;
                else if (Ry - cPy >= 0)//Se ir pra baixo for mais benéfico
                    newY = newYB;
                else//Se ir pra cima for mais benéfico
                    newY = newYC;
            }

            if (move && cooldownTimeQ > 0)
            {
                Vector3 newP = new Vector3(newX, newY);//posição que queremos ir
                lookDirection = (newP - transform.position).normalized;  
                movePlayer(lookDirection);
            }
            cooldownTimeQ = Mathf.Clamp(cooldownTimeQ - Time.fixedDeltaTime, -1, Mathf.Infinity);
        }
       
    }

    private void movePlayer(Vector2 lookDirection)
    {
        animator.SetFloat("Move X", lookDirection.x);
        animator.SetFloat("Move Y", lookDirection.y);
        animator.SetFloat("Speed", lookDirection.magnitude);

        rigidbody2d.velocity = new Vector2(lookDirection.x * (speed + speedMod), lookDirection.y * (speed + speedMod));
    }

    public void changeHealth(int k)
    {
        health += k;
        healthText.text = gameObject.name + ": " + health;
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag == "Rock")
            changeHealth(-1);
    }

    public void setHelthText(TMP_Text text)
    {
        healthText = text;
    }

    public int getHealth() { return health; }

    public void attackStart() { }
    public void attackEnd() { }
}

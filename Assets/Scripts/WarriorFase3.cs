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

    private void Awake()
    {
        health = 10;
    }

    void Start()
    {
        lookDirection = new Vector2(1, 0);
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        cooldownTime = 0.0f;
        healthText.text = gameObject.name + ": " + health;
        distaceMin = 2.5f;
        Spots = GameObject.FindGameObjectsWithTag("Spot");

        if (gameObject.name == "Alistair")
            speed = 3.0f;
        else
            speed = 4.0f;

        GameObject[] Warriors = GameObject.FindGameObjectsWithTag("Player");
    }

    void Update()
    {
        float px = transform.position.x;
        float py = transform.position.y;

        if (gameObject.name == "Alistair") //Humano 
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
                animator.SetTrigger("Attack01");
            }
        }
    }

    private void FixedUpdate()
    {
        float px = transform.position.x;
        float py = transform.position.y;

        if (gameObject.name == "Alistair") //Humano 
        {
            Vector2 position = rigidbody2d.position;
            position.x = position.x + speed * horizontal * Time.deltaTime;
            position.y = position.y + speed * vertical * Time.deltaTime;

            rigidbody2d.MovePosition(position);
        }

        else if((py <= -5.0f || py >= 3.0f || px >= 4.5f || px <= -5.0) && cooldownTime == 0)
            //Caso esteja muito próximo a uma parede ele vai voltar ao centro
        {
            spot = Spots[UnityEngine.Random.Range(0, 4)];
            lookDirection = (spot.transform.position - transform.position).normalized;
            movePlayer(lookDirection);
            cooldownTime = 0.75f;
        }

        else if(cooldownTime == 0)//Máquina
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, 1.5f, LayerMask.GetMask("Rock"));

            if (colliders.Length > 0)
            {
                speed = 3.0f;
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
                if (Rx - cPx >= 0)//Se ir pra esquerda for mais benéfico
                    newX = cPx - (distaceMin - distanceInX);
                else//Se ir pra direita for mais benéfico
                    newX = cPx + (distaceMin - distanceInX);
            }

            if (rockH)//Verifica se o player vai ser acertado em y pela pedra
            {
                move = true;
                if (Ry - cPy >= 0)//Se ir pra baixo for mais benéfico
                    newY = cPy - (distaceMin - distanceInY);
                else//Se ir pra cima for mais benéfico
                    newY = cPy + (distaceMin - distanceInY);
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

        if (lookDirection.x < 0) lookDirection.x -= 0.5f;
        else if (lookDirection.x > 0) lookDirection.x += 0.5f;

        if (lookDirection.y < 0) lookDirection.y -= 0.5f;
        else if (lookDirection.y > 0) lookDirection.y += 0.5f;

        rigidbody2d.velocity = new Vector2(lookDirection.x * speed, lookDirection.y * speed);
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
}

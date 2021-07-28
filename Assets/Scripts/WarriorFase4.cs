using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WarriorFase4 : MonoBehaviour
{

    private Animator animator;

    private float speed;
    private Rigidbody2D rigidbody2d;

    private float horizontal;
    private float vertical;
    private Vector2 lookDirection;

    private string warriorName;
    private int difficulty;
    private float speedMod;
    private bool red;
    private int min;//Pontos minimos e maximos ao acertar o rival
    private int max;

    private int score;
    private bool hit;//Quando hit for true significa que o guerreiro deu uma espadada(definido do evento das animações de ataque) 
    public TMP_Text scoreText;
    private int points;//Número randomico que vai definir quando pontos o guerreiro vai fazer ao acertar o rival
    private float countdownRival;

    private GameObject[] Skeletons;
    private GameObject[] SkeletonsRed;//Vai ser usada para dificuldade mais dificeis, aqui o guerreiro rival sempre irá caçar o vermelho primeiro
    private GameObject skeleton;

    private void Awake()
    {
        score = 10;
    }

    void Start()
    {
        warriorName = GameManager.Instance.WarriorName;
        difficulty = GameManager.Instance.Difficulty;
        score = 0;
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        speed = 3.0f;
        speedMod = 0.0f;
        countdownRival = 0.0f;

        if (difficulty == 1)
        {
            speedMod = 0.0f;
            red = false;
            min = 1;
            max = 3;
        }
        else if (difficulty == 2)
        {
            speedMod = 0.25f;
            red = true;
            min = 2;
            max = 4;
        }
        else if (difficulty == 3)
        {
            speedMod = 0.5f;
            red = true;
            min = 3;
            max = 6;
        }
    }

    void Update()
    {
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

    void FixedUpdate()
    {
        Skeletons = GameObject.FindGameObjectsWithTag("Skeleton");

        if (gameObject.name == warriorName && countdownRival == 0) //Humano 
        {
            Vector2 position = rigidbody2d.position;
            position.x = position.x + speed * horizontal * Time.deltaTime;
            position.y = position.y + speed * vertical * Time.deltaTime;

            rigidbody2d.MovePosition(position);
        }
        else //Máquina
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, 1.0f, LayerMask.GetMask("Player"));

            if (colliders.Length > 0 && countdownRival == 0)
            {

                foreach (Collider2D collider in colliders)
                {
                    if (collider.name != gameObject.name)
                    {
                        animator.SetTrigger("Attack01");
                        lookDirection = (collider.transform.position - transform.position).normalized;
                        move(lookDirection);
                        countdownRival = 0.2f;
                    }
                }
            }
            
            if (countdownRival == 0)
            {
                skeleton = GameObject.Find("SkeletonR");
                if (red && skeleton != null)
                {
                    lookDirection = (skeleton.transform.position - transform.position).normalized;
                    move(lookDirection);
                }
                else
                {
                    skeleton = GetClosestSkeleton(Skeletons);//pega o skeleto mais próximo para atacar 
                    if (skeleton != null)
                    {
                        animator.SetTrigger("Attack01");
                        lookDirection = (skeleton.transform.position - transform.position).normalized;
                        move(lookDirection);

                    }
                }
                countdownRival = 0.2f;
            }
        }
        countdownRival = Mathf.Clamp(countdownRival - Time.fixedDeltaTime, 0, Mathf.Infinity);
    }

    private void move(Vector2 lookDirection)
    {
        animator.SetFloat("Move X", lookDirection.x);
        animator.SetFloat("Move Y", lookDirection.y);
        animator.SetFloat("Speed", lookDirection.magnitude);

        rigidbody2d.velocity = new Vector2(lookDirection.x * (speed + speedMod), lookDirection.y * (speed + speedMod));
    }

    GameObject GetClosestSkeleton(GameObject[] skeletons)
    {
        GameObject closestSkeleton = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject skeleton in skeletons)
        {
            Vector3 directionToSkeleton = skeleton.transform.position - currentPosition;
            float distanceToSkeleton = directionToSkeleton.sqrMagnitude;
            if (distanceToSkeleton < closestDistance)
            {
                closestDistance = distanceToSkeleton;
                closestSkeleton = skeleton;
            }
        }

        return closestSkeleton;
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag == "Skeleton")
        {
            if (gameObject.name != warriorName) animator.SetTrigger("Attack01");

            if (hit)
            {
                hit = false;

                if(other.gameObject.name == "SkeletonW")
                    changeScore(1);
                if (other.gameObject.name == "SkeletonR")
                    changeScore(10);
                if (other.gameObject.name == "SkeletonC")
                    changeScore(15);

                Destroy(other.gameObject);
            }
        }

        if (other.gameObject.tag == "Player")
        {
            if (hit)
            {
                hit = false;
                other.gameObject.GetComponent<WarriorFase4>().push(gameObject);

                if(gameObject.name == warriorName) points = Random.Range(1, 6);
                else points = Random.Range(min, max);

                changeScore(points);
            }
        }

    }

    public void changeScore(int k)
    {
        score += k;
        scoreText.text = gameObject.name + ": " + score;
    }

    public void push(GameObject warrior)
    {
        float lookDirectionX = -(warrior.transform.position.x - transform.position.x);
        float lookDirectionY = -(warrior.transform.position.y - transform.position.y);
        lookDirection = new Vector2(lookDirectionX, lookDirectionY);

        animator.SetFloat("Move X", lookDirection.x);
        animator.SetFloat("Move Y", lookDirection.y);
        animator.SetFloat("Speed", lookDirection.magnitude);

        rigidbody2d.velocity = new Vector2(lookDirection.x * (speed + 10.0f), lookDirection.y * (speed + 10.0f));
        countdownRival = 0.5f;
    }

    public void setScoreText(TMP_Text text)
    {
        scoreText = text;
    }

    public int getScore() { return score; }
    public void attackStart() { hit = true; }
    public void attackEnd() { hit = false; }
}

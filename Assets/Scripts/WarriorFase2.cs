using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarriorFase2 : MonoBehaviour
{
    Animator animator;

    float speed;//velocidade do guerreiro
    Rigidbody2D rigidbody2d;

    float horizontal;
    float vertical;
    Vector2 lookDirection;

    public TMP_Text healthText;
    private int health;
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;
    Color startColor;

    private GameObject[] Warriors;
    private GameObject[] Skeletons;
    private GameObject skeleton;
    float cooldownTime;

    private string warriorName;
    private int difficulty;
    private float speedMod;

    private bool startGame;

    public bool StartGame { get => startGame; set => startGame = value; }

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
        speed = 3.0f;
        startColor = GetComponent<SpriteRenderer>().color;
        cooldownTime = 0.0f;
        healthText.text = gameObject.name + ": " + health;
        Warriors = GameObject.FindGameObjectsWithTag("Player");

        if (difficulty == 1)
        {
            speedMod = 0.25f;
        }
        if (difficulty == 2)
        {
            speedMod = 0.5f;
        }
        if (difficulty == 3)
        {
            speedMod = 0.5f;
        }
        startGame = false;
    }

    void Update()
    {
        if (startGame)
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
    }


    void FixedUpdate()
    {
        if (startGame)
        {
            if (isInvincible)
            {
                GetComponent<SpriteRenderer>().color = new Color(Random.Range(0.0f, 1.0f), 
                    Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);

                invincibleTimer -= Time.deltaTime;
                if (invincibleTimer < 0)
                {
                    GetComponent<SpriteRenderer>().color = startColor;
                    Collision(false, false, 3.0f);
                }
            }

            if (gameObject.name == warriorName) //Humano 
            {
                Vector2 position = rigidbody2d.position;
                position.x = position.x + speed * horizontal * Time.deltaTime;
                position.y = position.y + speed * vertical * Time.deltaTime;

                rigidbody2d.MovePosition(position);
            }
            else //Máquina
            {

                Skeletons = GameObject.FindGameObjectsWithTag("Skeleton");
                if (cooldownTime == 0)
                {
                    skeleton = GetClosestSkeleton(Skeletons);//pega o skeleto mais próximo pra fugir dele 
                    runAway(skeleton);
                    cooldownTime = 1.25f;
                }

            }
            cooldownTime = Mathf.Clamp(cooldownTime - Time.fixedDeltaTime, 0, Mathf.Infinity);
        }
    }

    private void runAway(GameObject skeleton)
    {
        float newX = transform.position.x;
        float newY = transform.position.y;

        float distX = Mathf.Abs(skeleton.transform.position.x - newX);
        float distY = Mathf.Abs(skeleton.transform.position.y - newY);

        if (distX <= distY)
        {
            if (skeleton.transform.position.x >= -1.0f)
            {
                newX = -7f;
            }
            else
            {
                newX = 5.00f;
            }
        }
        else
        {
            if (skeleton.transform.position.y >= -1.0f)
            {
                newY = -5.5f;
            }
            else
            {
                newY = 4f;
            }
        }

        Vector3 newP = new Vector3(newX, newY);
        lookDirection = (newP - transform.position).normalized;
        animator.SetFloat("Move X", lookDirection.x);
        animator.SetFloat("Move Y", lookDirection.y);
        animator.SetFloat("Speed", lookDirection.magnitude);

        rigidbody2d.velocity = new Vector2(lookDirection.x * (speed + speedMod), 
            lookDirection.y * (speed + speedMod));

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

    public void changeHealth(int k)
    {
        if (isInvincible)
            return;

        invincibleTimer = timeInvincible;

        health += k;
        healthText.text = gameObject.name + ": " + health;

        //Quando o player for atingido ele terá um aumento na velocidade,
        //não vai colidir mais com o esqueleto e começará a piscar
        Collision(true, true, 5.0f);
    }

    public void Collision(bool option, bool isInvincibleOption, float speedOption)
    {
        isInvincible = isInvincibleOption;
        speed = speedOption;

        Skeletons = GameObject.FindGameObjectsWithTag("Skeleton");

        foreach (GameObject skeleton in Skeletons)
            Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), 
                skeleton.GetComponent<Collider2D>(), option);

        foreach (GameObject warrior in Warriors)
            if(warrior != null && warrior.name != gameObject.name)
                Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), 
                    warrior.GetComponent<Collider2D>(), option);
    }

    public void setHelthText(TMP_Text text)
    {
        healthText = text;
    }

    public int getHealth() { return health; }

    public void attackStart() { }
    public void attackEnd() { }


}

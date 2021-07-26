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

    float lookDistance;//contém a distância do primeiro objeto que foi interceptado pelo raio

    private GameObject[] Skeletons;
    private GameObject skeleton;
    float cooldownTime;

    private GameObject[] Spots;
    private GameObject spot;

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
        speed = 3.0f;
        startColor = GetComponent<SpriteRenderer>().color;
        cooldownTime = 0.0f;
        Spots = GameObject.FindGameObjectsWithTag("Spot");
        healthText.text = gameObject.name + ": " + health;

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
        if (isInvincible)
        {
            GetComponent<SpriteRenderer>().color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);

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
                spot = GetBetterSpot(skeleton, 4);
                cooldownTime = 1.5f;
            }

            if(spot != null)
            {
                lookDirection = (spot.transform.position - transform.position).normalized;
                animator.SetFloat("Move X", lookDirection.x);
                animator.SetFloat("Move Y", lookDirection.y);
                animator.SetFloat("Speed", lookDirection.magnitude);

                rigidbody2d.velocity = new Vector2(lookDirection.x * (speed + speedMod), lookDirection.y * (speed + speedMod));

                cooldownTime = Mathf.Clamp(cooldownTime - Time.fixedDeltaTime, 0, Mathf.Infinity);
            }
        }
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

    GameObject GetBetterSpot(GameObject skeleton, int k)//Retorna um ponto aleatório entre os k mais distance do skeleto mais proximo
    {
        GameObject actualSpot;
        GameObject tempSpot;
        List<GameObject> furtherSpots = new List<GameObject>();
        Vector3 currentPosition = skeleton.transform.position;//Posição atual do skeleto

        for (int i = 0; i < Spots.Length; i++)
        {
            actualSpot = Spots[i];
            Vector3 directionToSpot = actualSpot.transform.position - currentPosition;
            float distanceToSpot = directionToSpot.sqrMagnitude;//Distãncia do Skeleto atual para o ponto

            if (furtherSpots == null || furtherSpots.Count < k)
            {
                furtherSpots.Add(actualSpot);
            }
            else
            {
                for (int j = 0; j < k; j++)
                {
                    Vector3 directionToClosestSlime = furtherSpots[j].transform.position - currentPosition;
                    float distanceToClosestSpot = directionToClosestSlime.sqrMagnitude;//Distancia do ponto mais proximo para o esqueleto

                    if (distanceToSpot > distanceToClosestSpot)//verifica se o ponto atual está mais próximo que o ponto que já estava no vetor
                    {
                        tempSpot = furtherSpots[j];
                        furtherSpots[j] = actualSpot;

                        //Abaixo faço a troca e verifico se o ponto que perdeu seu lugar está mais próximo do esqueleto do que os outros que tbm estavam no vetor
                        actualSpot = tempSpot;
                        directionToSpot = actualSpot.transform.position - currentPosition;
                        distanceToSpot = directionToSpot.sqrMagnitude;
                    }
                }
            }

        }

        int position = Random.Range(0, k);
        return furtherSpots.ToArray()[position];//retorna posição aleatória dentro do vetor 
    }

    public void changeHealth(int k)
    {
        if (isInvincible)
            return;

        invincibleTimer = timeInvincible;

        health += k;
        healthText.text = gameObject.name + ": " + health;

        //Quando o player for atingido ele terá um aumento na velocidade, não vai colidir mais com o esqueleto e começará a piscar
        Collision(true, true, 5.0f);
    }

    public void Collision(bool option, bool isInvincibleOption, float speedOption)//ativa e desativa colisão com esqueleto
    {
        isInvincible = isInvincibleOption;
        speed = speedOption;

        Skeletons = GameObject.FindGameObjectsWithTag("Skeleton");
        foreach (GameObject skeleton in Skeletons)
            Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), 
                skeleton.GetComponent<Collider2D>(), option);
    }

    public void setHelthText(TMP_Text text)
    {
        healthText = text;
    }

    public int getHealth() { return health; }

    public void attackStart() { }
    public void attackEnd() { }


}

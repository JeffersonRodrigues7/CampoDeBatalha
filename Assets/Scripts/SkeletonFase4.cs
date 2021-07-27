using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonFase4 : MonoBehaviour
{
    Animator animator;

    float speed;//velocidade do Skeleto
    Rigidbody2D rigidbody2d;
    bool hit;
    float cooldownTime;
    float cooldownTimeC;

    float horizontal;
    float vertical;
    Vector2 lookDirection;

    private List<GameObject> WarriorsList;
    private GameObject[] WarriorsArray;
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
        cooldownTime = 0.0f;
        cooldownTimeC = 0.0f;

        if (gameObject.name == "SkeletonW")
        {
            speed = 3.0f;
        }
        if (gameObject.name == "SkeletonR")
        {
            speed = 0.0f;
        }
        if (gameObject.name == "SkeletonC")
        {
            speed = 8.0f;
        }
    }

    void FixedUpdate()
    {
        WarriorsArray = GameObject.FindGameObjectsWithTag("Player");

        if (gameObject.name == "SkeletonW" && cooldownTime == 0)
        {
            foreach (GameObject warrior in WarriorsArray)
                WarriorsList.Add(warrior);

            warrior = shuffle(WarriorsList.ToArray());//retorna um guerreiro aleatório do campo para atacar

            cooldownTime = 2f;
        }

        if (gameObject.name == "SkeletonR")
        {
            warrior = GetClosestWarrior(WarriorsArray);//pega o guerreiro mais próximo para atacar 
        }

        if (gameObject.name == "SkeletonC")
            GetComponent<SpriteRenderer>().color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);

        if (gameObject.name == "SkeletonC" && cooldownTimeC == 0)
        {
            warrior = GetClosestWarrior(WarriorsArray);//pega o guerreiro mais proximo para fugir
            runAway(warrior);
            cooldownTimeC = 0.2f;
        }

        if (warrior != null && gameObject.name != "SkeletonC")
            move(warrior);

        cooldownTime = Mathf.Clamp(cooldownTime - Time.fixedDeltaTime, 0, Mathf.Infinity);
        cooldownTimeC = Mathf.Clamp(cooldownTimeC - Time.fixedDeltaTime, 0, Mathf.Infinity);
    }

    private void move(GameObject warrior)
    {
        animator.SetTrigger("Attack01");
        lookDirection = (warrior.transform.position - transform.position).normalized;//Trava a visão do player bem no centro do Slime, assim o raio n muda a medida que o player se aproxima, preciso normalizar para multiplicar pelo speed
        animator.SetFloat("Move X", lookDirection.x);
        animator.SetFloat("Move Y", lookDirection.y);
        animator.SetFloat("Speed", lookDirection.magnitude);

        rigidbody2d.velocity = new Vector2(lookDirection.x * speed, lookDirection.y * speed);
    }

    private void runAway(GameObject warrior)
    {
        float newX = transform.position.x;
        float newY = transform.position.y;

        float distX = Mathf.Abs(warrior.transform.position.x - newX);
        float distY = Mathf.Abs(warrior.transform.position.y - newY);

        if(distX <= distY)
        {
            if(warrior.transform.position.x >= -1.0f)
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
            if (warrior.transform.position.y >= -1.0f)
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

        rigidbody2d.velocity = new Vector2(lookDirection.x * speed, lookDirection.y * speed);

    }

    private GameObject shuffle(GameObject[] warriors)
    {
        for (int t = 0; t < warriors.Length; t++)
        {
            GameObject tmp = warriors[t];
            int r = Random.Range(t, warriors.Length);
            warriors[t] = warriors[r];
            warriors[r] = tmp;
        }

        return warriors[Random.Range(0, warriors.Length)];
    }

    GameObject GetClosestWarrior(GameObject[] warriors)
    {
        GameObject closestWarrior = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject warrior in warriors)
        {
            Vector3 directionToWarrior = warrior.transform.position - currentPosition;
            float distanceToWarrior = directionToWarrior.sqrMagnitude;
            if (distanceToWarrior < closestDistance)
            {
                closestDistance = distanceToWarrior;
                closestWarrior = warrior;
            }
        }

        return closestWarrior;
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player" && hit)
        {
            hit = false;
            WarriorFase4 warrior = other.collider.GetComponent<WarriorFase4>();

            if (warrior != null)
            {
                if (gameObject.name == "SkeletonW")
                    warrior.changeScore(-1);
                if (gameObject.name == "SkeletonR")
                    warrior.changeScore(-5);
            }
            
        }
    }

    public void attackStart() { hit = true; }
    public void attackEnd() { hit = false; }

}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarriorFase1 : MonoBehaviour
{
    Animator animator;

    float speed;//velocidade do guerreiro
    Rigidbody2D rigidbody2d;

    float horizontal;
    float vertical;
    Vector2 lookDirection;

    bool hit;//Quando hit for true significa que o guerreiro deu uma espadada(definido do evento das animações de ataque) 
    public TMP_Text scoreText;
    int score;

    float lookDistance;//contém a distância do primeiro objeto que foi interceptado pelo raio
    float Countdown;
    float cooldownTime;//Quando o raio do player atingir alguém esse cooldown é setado para que a direção do player para de se mover e ele consiga ir diretamente pro Slime e acertar
    LineRenderer lineRenderer;//cria os linhas entre os GameObjects

    private GameObject[] Slimes;
    private GameObject slime;
    private int qtdNeigh;
    private float speedMod;//speedMod é somente para a IA

    private string warriorName;
    private int difficulty;
    private bool startGame;

    public bool StartGame { get => startGame; set => startGame = value; }

    void Start()
    {
        warriorName = GameManager.Instance.WarriorName;
        difficulty = GameManager.Instance.Difficulty;
        lookDirection = new Vector2(1, 0);
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        speed = 3.0f;
        score = 0;
        hit = false;
        cooldownTime = 0.0f;
        Slimes = null;
        slime = null;
        startGame = false;

        if (difficulty == 1)
        {
            speedMod = 0.0f;
            qtdNeigh = 5;
        }
        if (difficulty == 2)
        {
            speedMod = 1.0f;
            qtdNeigh = 3;
        }
        if (difficulty == 3)
        {
            speedMod = 1.5f;
            qtdNeigh = 3;
        }
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
            if (gameObject.name == warriorName) //Humano 
            {
                Vector2 position = rigidbody2d.position;
                position.x = position.x + speed * horizontal * Time.deltaTime;
                position.y = position.y + speed * vertical * Time.deltaTime;

                rigidbody2d.MovePosition(position);
            }

            else //Máquina
            {
                Slimes = GameObject.FindGameObjectsWithTag("Slime");

                if (Slimes.Length == 0)
                {
                    slime = null;
                    rigidbody2d.velocity = new Vector2(0, 0);
                    animator.SetFloat("Move X", lookDirection.x);
                    animator.SetFloat("Move Y", lookDirection.y);
                    animator.SetFloat("Speed", 0f);
                }

                else if (cooldownTime == 0 && Slimes.Length < qtdNeigh)
                    slime = GetClosestSlime(Slimes, Slimes.Length);
                else if (cooldownTime == 0)
                    slime = GetClosestSlime(Slimes, qtdNeigh);

                if (slime != null)//Se atigiu algo
                {
                    lookDirection = (slime.transform.position - transform.position).normalized;
                    animator.SetFloat("Move X", lookDirection.x);
                    animator.SetFloat("Move Y", lookDirection.y);
                    animator.SetFloat("Speed", lookDirection.magnitude);

                    rigidbody2d.velocity = new Vector2(lookDirection.x * (speed + speedMod), lookDirection.y * (speed + speedMod));
                    cooldownTime = 0.25f;
                }

                cooldownTime = Mathf.Clamp(cooldownTime - Time.fixedDeltaTime, 0, Mathf.Infinity);
            }
        }
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if (startGame)
        {
            if (other.gameObject.tag == "Slime")
            {
                if (gameObject.name != warriorName) animator.SetTrigger("Attack01");

                if (hit)
                {
                    score++;
                    scoreText.text = gameObject.name + ": " + score;
                    Destroy(other.gameObject);
                }
            }
        }
    }

    GameObject GetClosestSlime(GameObject[] Slimes, int k)
    {
        GameObject actualSlime;
        GameObject tempSlime;
        List<GameObject> closestSlimes = new List<GameObject>();
        Vector3 currentPosition = transform.position;//Posição atual do player

        for (int i = 0; i < Slimes.Length; i++)
        {
            actualSlime = Slimes[i];
            Vector3 directionToSlime = actualSlime.transform.position - currentPosition;
            float distanceToSlime = directionToSlime.sqrMagnitude;//Distãncia do Slime atual para o player

            if (closestSlimes == null || closestSlimes.Count < k)
                closestSlimes.Add(actualSlime);
            else
            {
                for (int j = 0; j < k; j++)
                {
                    Vector3 directionToClosestSlime = closestSlimes[j].transform.position - currentPosition;
                    float distanceToClosestSlime = directionToClosestSlime.sqrMagnitude;//Distancia do Slime
                                                                                        //mais proximo para
                                                                                        //o player
                    if (distanceToSlime < distanceToClosestSlime)//verifica se o slime atual está mais
                                                                 //próximo que o slime que já estava no vetor
                    {
                        tempSlime = closestSlimes[j];
                        closestSlimes[j] = actualSlime;

                        //Abaixo faço a troca e verifico se o slime que perdeu seu lugar está mais próximo
                        //do player do que os outros que tbm estavam no vetor
                        actualSlime = tempSlime;
                        directionToSlime = actualSlime.transform.position - currentPosition;
                        distanceToSlime = directionToSlime.sqrMagnitude;
                    }
                }
            }
        }
        int position = Random.Range(0, k);
        return closestSlimes.ToArray()[position];//retorna posição aleatória dentro do vetor 
    }

    public void setScoreText(TMP_Text text)
    {
         scoreText = text;
    }

    public int getScore() { return score; }
    public void attackStart() { hit = true; }
    public void attackEnd() { hit = false; }


}

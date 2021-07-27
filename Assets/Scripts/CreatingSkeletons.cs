using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatingSkeletons : MonoBehaviour
{
    public GameObject skeletonW;
    public GameObject skeletonR;
    public GameObject skeletonC;
    List<Vector2> occupiedPositions;
    private GameObject[] Skeletons;
    private GameObject[] Warriors;
    private int numSkeletonsW;//Número máximo de esqueletos brancos permitidos no campo
    private float countdownSW;
    private float countdownSR;
    private float countdownSC;
    private bool createSkeleton;

    public bool CreateSkeleton { get => createSkeleton; set => createSkeleton = value; }

    void Start()
    {
        numSkeletonsW = 5;
        countdownSW = 0.0f;
        countdownSR = 5.0f;
        countdownSC = 10.0f;
        createSkeleton = true;
    }

    void Update()
    {
        if (createSkeleton)
        {
            Skeletons = GameObject.FindGameObjectsWithTag("Skeleton");

            if (Skeletons.Length < numSkeletonsW && countdownSW == 0)
            {
                instanceSkeleton(skeletonW);
            }

            if (countdownSR == 0)
            {
                instanceSkeleton(skeletonR);
                countdownSR = 5.0f;
            }

            if (countdownSC == 0)
            {
                instanceSkeleton(skeletonC);
                countdownSC = 10.0f;
            }

            countdownSW = Mathf.Clamp(countdownSW - Time.deltaTime, 0, Mathf.Infinity);
            countdownSR = Mathf.Clamp(countdownSR - Time.deltaTime, 0, Mathf.Infinity);
            countdownSC = Mathf.Clamp(countdownSC - Time.deltaTime, 0, Mathf.Infinity);
        }

    }

    public void instanceSkeleton(GameObject skeletonP)
    {
        Warriors = GameObject.FindGameObjectsWithTag("Player");

        occupiedPositions = new List<Vector2>();//posições do mapa já ocupadas por algum objeto

        foreach (GameObject skeleton in Skeletons)
            occupiedPositions.Add(skeleton.transform.position);
        foreach (GameObject warrior in Warriors)
            occupiedPositions.Add(warrior.transform.position);

        float x, y;
        bool occupied;
        do
        {
            occupied = false;
            x = Random.Range(-7f, 5f);//vamos colocar o esqueleto em um local aleatório
            y = Random.Range(-5f, 3.5f);
            Vector2 tryPosition = new Vector2(x, y);

            foreach (Vector2 otherPosition in occupiedPositions)
            {
                if (Vector2.Distance(tryPosition, otherPosition) < 1f)
                    occupied = true;
            }

        } while (occupied);

        Vector2 pos = new Vector2(x, y);//posição onde vamos colocar o objeto
        occupiedPositions.Add(pos);
        GameObject obj = Instantiate<GameObject>(skeletonP, pos, Quaternion.identity, transform);
        obj.name = skeletonP.name;
        countdownSW = 1.0f;
    }
}

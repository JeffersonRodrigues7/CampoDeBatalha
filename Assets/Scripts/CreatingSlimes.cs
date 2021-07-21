using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatingSlimes : MonoBehaviour
{
    //Esse c�digo vai fazer os Slimes se criarem sozinhos em tempo de execu��o
    public GameObject slime;
    List<Vector2> occupiedPositions;
    private GameObject[] Slimes;
    int numSlimes = 50;//N�mero m�ximo de slimes permitidos no campo

    private void Start()
    {
        occupiedPositions = new List<Vector2>//posi��es do mapa j� ocupadas por algum objeto
        {
            new Vector2(-1f, -0.5f)
        };

        for (int i = 0; i < numSlimes; i++)
        {

            float x, y;
            bool occupied;
            do
            {
                occupied = false;
                x = Random.Range(-7f, 5f);//vamos colocar o slime em um local aleat�rio
                y = Random.Range(-5f, 3.5f);
                Vector2 tryPosition = new Vector2(x, y);

                foreach (Vector2 otherPosition in occupiedPositions)/*Para cada objeto da cena que possui sua posi�a� em occupied Positions eu vou comparar com a nova que vou 
                                                                    *colocar. Se o novo objeto estiver perto de qualquer uma delas, vou gerar uma nova posi��o pra ele*/
                {
                    if (Vector2.Distance(tryPosition, otherPosition) < 1f)
                        occupied = true;
                }

            } while (occupied);

            Vector2 pos = new Vector2(x, y);//posi��o onde vamos colocar o objeto
            occupiedPositions.Add(pos);
            GameObject obj = Instantiate<GameObject>(slime, pos, Quaternion.identity, transform);//objeto, posi��o, orienta��o dele e objeto pai do objeto que vamos posicionar
                                                                                                 //No caso ele ser� filho do GameObject Slimes pois foi neles que colocamos o script CreatingSlimes
            obj.name = "Slime" + i;


        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    bool rockH;

    public void setRockH(bool H)
    {
        rockH = H;
    }
    public bool getRockH()
    {
        return rockH;
    }

    void OnCollisionStay2D(Collision2D other)
    {
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WarriorFaseAward : MonoBehaviour
{

    private Animator animator;
    private Rigidbody2D rigidbody2d;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetFloat("Move X", 0f);
        animator.SetFloat("Move Y", 1f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotPlayAnimation : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.Play("pose 3 - hello"); 
        }
    }
}

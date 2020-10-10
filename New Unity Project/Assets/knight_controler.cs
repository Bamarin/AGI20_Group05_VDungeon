﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class knight_controler : MonoBehaviour
{
    float speed = 4;
    float rotSpeed = 100;
    float rot = 0f;
    float gravity = 8;


    Vector3 moveDir = Vector3.zero;


    CharacterController controller;
    Animator anim;
    AnimatorClipInfo[] m_AnimatorClipInfo;
    int counter = 0;
  




    // Start is called before the first frame update
    void Start()
    {


        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        
    }


    // Update is called once per frame
    void Update()
    {
        Movement();
        UnityChanFace();
        GetInput();
        counter++;
        

    }
    void UnityChanFace()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            anim.SetInteger("face", 0);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            anim.SetInteger("face", 1);
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            anim.SetInteger("face", 2);
        }
        if (Input.GetKey(KeyCode.Alpha4))
        {
            anim.SetInteger("face", 3);
        }
        if (Input.GetKey(KeyCode.Alpha5))
        {
            anim.SetInteger("face", 4);
            anim.Play("disstract1@unitychan");
        }
        
    }
    void Movement()
    {
  
        


        if (Input.GetKey(KeyCode.W))
        {
            if (anim.GetBool("attacking") == true)
            {
                return;
            }
            else if (anim.GetBool("attacking") == false)
            {
                anim.SetBool("running", true);
                anim.SetInteger("condition", 1);
                moveDir = new Vector3(0, 0, 1);
                moveDir *= speed;
                moveDir = transform.TransformDirection(moveDir);
            }
        }

        if (Input.GetKey(KeyCode.S))
        {
            if (anim.GetBool("attacking") == true)
            {
                return;
            }
            else if (anim.GetBool("attacking") == false)
            {
                anim.SetBool("running", true);
                anim.SetInteger("condition", -1);
                moveDir = new Vector3(0, 0, -1);
                moveDir *= speed;
                moveDir *= 0.7f;
                moveDir = transform.TransformDirection(moveDir);
            }
        }

        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
        {
            anim.SetBool("running", false);
            anim.SetInteger("condition", 0);
            moveDir = new Vector3(0, 0, 0);
        }
        
        
        rot += Input.GetAxis("Horizontal") * rotSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, rot, 0);


        moveDir.y -= gravity * Time.deltaTime;
        controller.Move(moveDir * Time.deltaTime);
    }

    void GetInput()
    {
        if (controller.isGrounded)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (anim.GetBool("running") == true)
                {
                    anim.SetBool("running", false);
                    anim.SetInteger("condition", 0);
                }
                if (anim.GetBool("running") == false)
                {
                    Attacking();
                }
            }
        }
    }

    void Attacking()

    {
        StartCoroutine(AttackRoutine());
    }
    IEnumerator AttackRoutine()
    {
        anim.SetBool("attacking", true);
        anim.SetInteger("condition", 2);
        yield return new WaitForSeconds(1);
        anim.SetInteger("condition", 0);
        anim.SetBool("attacking", false);

        


    }
}
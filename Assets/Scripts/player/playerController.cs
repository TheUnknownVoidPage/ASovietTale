using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public float speed = 5.0f;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private string privPos;

    private bool isMovingHorizontally = false;
    private bool isMovingVertically = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        PlayerMovement();
    }

    private void LateUpdate()
    {
        //cameraController.CameraUpdate();
        //print("Camera Pos = " + cameraController.transform.position);
    }

    void PlayerMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (isMovingHorizontally)
        {
            moveY = 0;
        }
        else if (isMovingVertically)
        {
            moveX = 0;
        }
        else
        {
            if (moveX != 0)
            {
                isMovingHorizontally = true;
                isMovingVertically = false;
            }
            else if (moveY != 0)
            {
                isMovingVertically = true;
                isMovingHorizontally = false;
            }
        }

        // Reset movement direction when no input is detected
        if (moveX == 0 && moveY == 0)
        {
            isMovingHorizontally = false;
            isMovingVertically = false;
        }


        animator.SetFloat("VerticalMovement", moveY);
        animator.SetFloat("HorizontalMovement", moveX);

        if (moveX == 1)
        {
            if (moveY == 1) { animator.SetFloat("HorizontalMovement", 0); animator.SetFloat("VerticalMovement", 1); }
            else if (moveY == -1) { animator.SetFloat("HorizontalMovement", 0); animator.SetFloat("VerticalMovement", -1); }
        }
        else
        {
            if (moveX == 1) { animator.SetFloat("VerticalMovement", 0); animator.SetFloat("HorizontalMovement", 1); }
            else if (moveX == -1) { animator.SetFloat("VerticalMovement", 0); animator.SetFloat("HorizontalMovement", -1); }
        }

        if (moveX == 0 && moveY == 0)
        {
            animator.SetBool("isStanding", true);
            previousPosition(privPos);
        }
        else
        {
            animator.SetBool("isStanding", false);
        }

        if (moveX < 0)
        {
            privPos = "left";
        }
        if (moveX > 0)
        {
            privPos = "right";
        }
        if (moveY > 0)
        {
            privPos = "up";
        }
        if (moveY < 0)
        {
            privPos = "down";
        }

        rb.velocity = new Vector2(moveX * speed, moveY * speed);
    }

    private void previousPosition(string privPos)
    {
        switch (privPos)
        {
            case "left":
                animator.Play("playerStand", 0, 0.50f);
                break;
            case "right":
                animator.Play("playerStand", 0, 0.75f);
                break;
            case "down":
                animator.Play("playerStand", 0, 0.25f);
                break;
            case "up":
                animator.Play("playerStand", 0, 0.99f);
                break;
        }
    }


}
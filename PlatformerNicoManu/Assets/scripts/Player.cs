using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed, runningSpeedFactor, rotationSpeed, jumpSpeed, gravity;

    [SerializeField]
    private Transform playerCamTransform;
    private bool doubleJumped = true, playerRunning;

    private CharacterController characterController;
    private Vector3 movementDirection = Vector3.zero;
    private Animator animator;
    private bool iscrouching;
    private float standingHeight, crouchHeight;

    private void Start()
    {
        characterController = this.gameObject.GetComponent<CharacterController>();
        animator = this.gameObject.GetComponent<Animator>();
        standingHeight = characterController.height;
        crouchHeight = standingHeight / 2;
    }
    void Update()
    {
        InputAndMovementAndAnimation();
    }

    void InputAndMovementAndAnimation()
    {
        
        bool playerGrounded = characterController.isGrounded;
        animator.SetBool("grounded", playerGrounded);

        //if the player is running we set the speed acordingly
        float runningSpeed;
        if (IsRunning())
        {
            runningSpeed = runningSpeedFactor;
        }
        else
        {
            runningSpeed = 1f;
        }

        //we get the direction at wich the player is trying to go (and the magnitude at wich it tries to go)
        Vector3 cameraDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        float inputMagnitude = Mathf.Clamp01(cameraDirection.magnitude);
        cameraDirection = Quaternion.AngleAxis(playerCamTransform.rotation.eulerAngles.y, Vector3.up) * cameraDirection;
        cameraDirection.Normalize();

        //if the player stops trying to run we save it
        if (inputMagnitude <= 0.1f && inputMagnitude >= -0.1f)
        {
            //stoped
            playerRunning = false;
        }

        //the forward direction for the player becomes where he's trying to go and the speed becomes the magnitude he's trying to go
        if (cameraDirection != Vector3.zero)
        {
            transform.right = -cameraDirection;
        }
        Vector3 inputMovement = -transform.right * movementSpeed * runningSpeed * inputMagnitude;

        characterController.Move(inputMovement * Time.deltaTime);

        //jumping
        if (Input.GetButtonDown("Jump") && playerGrounded)
        {
            doubleJumped = false;
            movementDirection.y = jumpSpeed;
            animator.Play("sauter");
        }
        else if (Input.GetButtonDown("Jump") && (doubleJumped == false))
        {
            movementDirection.y = jumpSpeed * 1.2f;
            doubleJumped = true;
            animator.Play("doubleSaut");
        }

        //crouching
        if(Input.GetButton("crouch") && playerGrounded && !playerRunning)
        {
            iscrouching = true;
            characterController.height = crouchHeight;
            characterController.center = new Vector3(0,crouchHeight/2,0);
            animator.SetBool("crouch", true);
        }
        else
        {
            iscrouching = false;
            characterController.height = standingHeight;
            characterController.center = new Vector3(0, standingHeight / 2, 0);
            animator.SetBool("crouch", false);
        }

        movementDirection.y -= gravity * Time.deltaTime;

        characterController.Move(movementDirection * Time.deltaTime);

        //animations
        if (IsRunning())
        {
            if (inputMagnitude > 0f)
            {
                animator.SetBool("running", true);
                animator.SetBool("walking", true);
            }
            else
            {
                animator.SetBool("running", false);
                animator.SetBool("walking", false);
            }
            
        }
        else
        {
            if (inputMagnitude > 0f)
            {
                animator.SetBool("walking", true);
            }
            else
            {
                animator.SetBool("walking", false);
                animator.SetBool("running", false);
            }
            
        }
    }
    bool IsRunning()
    {
        if (!playerRunning)
        {
            if (Input.GetButton("Run") && characterController.isGrounded)
            {
                playerRunning = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }

}

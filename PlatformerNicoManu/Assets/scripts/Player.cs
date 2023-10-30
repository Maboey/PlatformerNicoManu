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

    private void Start()
    {
        characterController = this.gameObject.GetComponent<CharacterController>();
    }
    void Update()
    {
        InputAndMovement();
    }

    void InputAndMovement()
    {
        
        bool playerGrounded = characterController.isGrounded;

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
            transform.forward = cameraDirection;
        }
        Vector3 inputMovement = transform.forward * movementSpeed * runningSpeed * inputMagnitude;

        characterController.Move(inputMovement * Time.deltaTime);


        //jumping
        if (Input.GetButtonDown("Jump") && playerGrounded)
        {
            doubleJumped = false;
            movementDirection.y = jumpSpeed;
        }
        else if (Input.GetButtonDown("Jump") && (doubleJumped == false))
        {
            movementDirection.y = jumpSpeed * 1.2f;
            doubleJumped = true;
        }

        movementDirection.y -= gravity * Time.deltaTime;

        characterController.Move(movementDirection * Time.deltaTime);

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
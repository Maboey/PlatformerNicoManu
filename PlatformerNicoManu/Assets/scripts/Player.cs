using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed, runningSpeedFactor, rotationSpeed, jumpSpeed, gravity;

    [SerializeField]
    private Transform playerCamTransform;
    public bool isGrabingEdge = false;

    private CharacterController characterController;
    private Animator animator;
    private Vector3 movementDirection = Vector3.zero;
    private bool doubleJumped = true, playerRunning, iscrouching, playerGrounded;
    private float standingHeight, crouchHeight, inputMagnitude;

    private void Start()
    {
        characterController = this.gameObject.GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        standingHeight = characterController.height;
        crouchHeight = standingHeight / 2;
    }
    void Update()
    {
        if(isGrabingEdge)
        {
            LedgeMovementAndAnimation();
        }
        else
        {
            InputToMovement();
        }
        Animate();
    }

    void InputToMovement()
    {
        //Check and save if the character is grounded
        playerGrounded = characterController.isGrounded;

        // ========== Run / Walk ==========

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
        inputMagnitude = Mathf.Clamp01(cameraDirection.magnitude);
        cameraDirection = Quaternion.AngleAxis(playerCamTransform.rotation.eulerAngles.y, Vector3.up) * cameraDirection;
        cameraDirection.Normalize();

        //if the player stops trying to run we save it (to be able to keep the player running from the moment he pushed the button "run" until he releases the joystick that serves for movement)
        if (inputMagnitude <= 0.1f && inputMagnitude >= -0.1f)
        {
            //stoped
            playerRunning = false;
        }

        //the forward direction for the player becomes where he's trying to go and the speed becomes the magnitude he's trying to go
        if (cameraDirection != Vector3.zero)
        {
            transform.right = cameraDirection;
        }
        Vector3 inputMovement = transform.right * movementSpeed * runningSpeed * inputMagnitude;

        //Here we apply the forward speed to the player with an angle that has been calculated from the camera
        characterController.Move(inputMovement * Time.deltaTime);

        // ========== jumping ==========
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

        // ========== crouching ==========
        //Here we transform th height of the character for the collider to match the animations (and update the center of it)
        if (Input.GetButton("crouch") && playerGrounded)
        {
            iscrouching = true;
            characterController.height = crouchHeight;
            characterController.center = new Vector3(0,crouchHeight/2,0);
        }
        else
        {
            iscrouching = false;
            characterController.height = standingHeight;
            characterController.center = new Vector3(0, standingHeight / 2, 0);
        }

        if(iscrouching)
        {
            playerRunning = false;
        }

        movementDirection.y -= gravity * Time.deltaTime; // <-- Here we apply gravity to the vertical direction that we are gonna set to the player

        characterController.Move(movementDirection * Time.deltaTime); // Here we apply vertical speed to the player
    }
    void Animate()
    {
        animator.SetBool("grounded", playerGrounded);
        if (IsRunning())
        {
            if (inputMagnitude > 0f) // as long as the player is pushing the left joystick in any direction and was already running or walking we continue
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
        animator.SetBool("crouch", iscrouching);
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

    void LedgeMovementAndAnimation()
    {
    }
    public void StopGrabingEdge()
    {
    }
}

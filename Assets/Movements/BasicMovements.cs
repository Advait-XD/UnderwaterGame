using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class BasicMovement : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float swimSpeed = 4f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float buoyancy = 5f;
    public float lookSpeed = 2f; // Sensitivity for mouse look
    public float lookXLimit = 45f; // Limit for vertical look
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0; // Vertical rotation for camera
    private CharacterController characterController;
    private bool canMove = true;
    private bool isSwimming = false;
    private bool isCrouching = false;
    private bool isInSubmarine = false; // Track if the player is in the submarine
    private SubmarineController currentSubmarine; // Reference to the submarine controller

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (isInSubmarine)
        {
            ControlSubmarine();
            if (Input.GetKeyDown(KeyCode.E)) // Exit the submarine with 'E' key
            {
                ExitSubmarine();
            }
        }
        else
        {
            // Check if player is in swimming mode or walking mode
            if (isSwimming)
            {
                Swim();
            }
            else
            {
                Walk();
            }

            // Handle mouse look (both in walking and swimming modes)
            if (canMove)
            {
                HandleMouseLook();
            }
        }
    }

    private void HandleMouseLook()
    {
        // Horizontal rotation (Y-axis) for player movement
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        transform.rotation *= Quaternion.Euler(0, mouseX, 0);

        // Vertical rotation (X-axis) for camera
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }

    private void ControlSubmarine()
    {
        // Submarine movement logic
        float speed = 5f; // Speed of the submarine
        float mouseLookSpeed = 2f; // Sensitivity for mouse look

        // Forward/backward movement
        float moveForward = Input.GetAxis("Vertical") * speed * Time.deltaTime; // W/S keys for forward/backward
        float moveSide = Input.GetAxis("Horizontal") * speed * Time.deltaTime; // A/D keys for left/right
        float moveUp = 0;

        // Move up/down using Space and Left Control
        if (Input.GetKey(KeyCode.Space))
        {
            moveUp = speed * Time.deltaTime; // Move up
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            moveUp = -speed * Time.deltaTime; // Move down
        }

        // Apply movement
        Vector3 submarineMovement = transform.forward * moveForward + transform.right * moveSide + transform.up * moveUp;
        currentSubmarine.transform.position += submarineMovement;

        // Mouse look for submarine
        float mouseX = Input.GetAxis("Mouse X") * mouseLookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * mouseLookSpeed;

        // Horizontal rotation for submarine
        currentSubmarine.transform.Rotate(0, mouseX, 0);

        // Vertical rotation (clamped)
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        currentSubmarine.GetComponentInChildren<Camera>().transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }

    private void Walk()
    {
        // Walking or running movement
        if (!isInSubmarine) // Only allow walking if not in submarine
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            // Jumping logic
            if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
            {
                moveDirection.y = jumpPower;
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }

            // Apply gravity when not grounded
            if (!characterController.isGrounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }

            // Crouch toggle
            if (Input.GetKeyDown(KeyCode.C) && canMove)
            {
                isCrouching = !isCrouching;
                if (isCrouching)
                {
                    characterController.height = crouchHeight;
                    walkSpeed = crouchSpeed;
                    runSpeed = crouchSpeed;
                }
                else
                {
                    characterController.height = defaultHeight;
                    walkSpeed = 6f;
                    runSpeed = 12f;
                }
            }

            // Apply movement
            characterController.Move(moveDirection * Time.deltaTime);
        }
    }

    private void Swim()
    {
        // Swimming movement logic
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = swimSpeed * Input.GetAxis("Vertical");
        float curSpeedY = swimSpeed * Input.GetAxis("Horizontal");
        float verticalMovement = 0;

        // Move up/down while swimming (Space for up, Ctrl for down)
        if (Input.GetKey(KeyCode.Space))
        {
            verticalMovement = swimSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            verticalMovement = -swimSpeed;
        }

        moveDirection = (forward * curSpeedX) + (right * curSpeedY) + (Vector3.up * verticalMovement);

        // Simulate buoyancy (pulling the player up when in water)
        if (!characterController.isGrounded)
        {
            moveDirection.y += buoyancy * Time.deltaTime;
        }

        // Apply movement
        characterController.Move(moveDirection * Time.deltaTime);
    }

    // Detect when entering or exiting water
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            isSwimming = true;
        }
        else if (other.CompareTag("Submarine")) // Check for the submarine tag
        {
            // Removed the Enter key check here to manage entry through triggers
            EnterSubmarine(other.GetComponent<SubmarineController>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            isSwimming = false;
        }
        else if (other.CompareTag("Submarine"))
        {
            ExitSubmarine();
        }
    }

    private void EnterSubmarine(SubmarineController submarine)
    {
        isInSubmarine = true;
        currentSubmarine = submarine; // Reference to the submarine
        currentSubmarine.EnterSubmarine(gameObject); // Disable the player model and enable submarine controls
    }

    private void ExitSubmarine()
    {
        if (currentSubmarine != null)
        {
            currentSubmarine.ExitSubmarine(); // Re-enable the player model and disable submarine controls
            isInSubmarine = false; // Update the player's state
            currentSubmarine = null; // Clear the submarine reference
        }
    }
}
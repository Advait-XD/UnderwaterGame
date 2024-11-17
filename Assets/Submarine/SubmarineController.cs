using UnityEngine;

public class SubmarineController : MonoBehaviour
{
    public Camera submarineCamera;  // Camera to be used when in the submarine
    public float speed = 5f;         // Speed of the submarine
    public float mouseLookSpeed = 2f; // Sensitivity for mouse look
    public float lookXLimit = 45f;    // Limit for vertical look

    private float rotationX = 0;      // Vertical rotation for camera
    private bool isInSubmarine = false; // Track if the player is in the submarine
    private GameObject player;         // Reference to the player object

    void Start()
    {
        submarineCamera.enabled = false; // Disable submarine camera initially
    }

    void Update()
    {
        if (isInSubmarine)
        {
            MoveSubmarine();
            HandleMouseLook();

            // Check for exit command
            if (Input.GetKeyDown(KeyCode.E)) // Use 'E' key to exit
            {
                ExitSubmarine();
            }
        }
    }

    private void MoveSubmarine()
    {
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
        Vector3 movement = transform.forward * moveForward + transform.right * moveSide + transform.up * moveUp;
        transform.position += movement;
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseLookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * mouseLookSpeed;

        // Horizontal rotation
        transform.Rotate(0, mouseX, 0);

        // Vertical rotation (clamped)
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        submarineCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }

    public void EnterSubmarine(GameObject player)
    {
        isInSubmarine = true;
        this.player = player; // Store the player reference
        player.SetActive(false); // Disable the player model
        submarineCamera.enabled = true; // Enable the submarine camera
    }

    public void ExitSubmarine()
    {
        isInSubmarine = false;
        player.SetActive(true); // Enable the player model
        submarineCamera.enabled = false; // Disable the submarine camera
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the player enters the trigger
        {
            EnterSubmarine(other.gameObject); // Automatically enter the submarine
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the player exits the trigger
        {
            ExitSubmarine(); // Automatically exit the submarine
        }
    }
}
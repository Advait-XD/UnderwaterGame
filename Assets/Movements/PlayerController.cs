using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;            // Speed of movement
    public float jumpForce = 5f;            // Force applied when jumping
    public Transform playerCamera;           // Reference to the player camera
    private Rigidbody rb;                    // Rigidbody component
    private bool isGrounded;                 // Check if the player is on the ground
    public LayerMask groundLayer;            // Layer to identify the ground
    public float groundCheckDistance = 0.1f; // Distance for ground check
    private Vector3 moveDirection;           // Direction for movement

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Check if the player is grounded
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundLayer);

        // Call movement and jumping methods
        Move();
        Jump();
    }

    void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate movement direction relative to the camera
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Get angle in relation to camera
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
            Vector3 moveDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

            // Move the player
            rb.MovePosition(rb.position + moveDir.normalized * moveSpeed * Time.deltaTime);

            // Rotate the player towards the move direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    void Jump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmos()
    {
        // Draw a sphere in the editor to visualize ground check
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, groundCheckDistance);
    }
}

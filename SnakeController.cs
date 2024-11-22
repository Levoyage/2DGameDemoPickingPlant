using UnityEngine;

public class SnakeController : MonoBehaviour
{
    public float moveSpeed = 1f; // Snake's movement speed
    public float detectionRange = 5f; // Range to detect the player
    public float idleDurationMin = 1f; // Minimum idle duration
    public float idleDurationMax = 2f; // Maximum idle duration
    public float moveDurationMin = 2f; // Minimum move duration
    public float moveDurationMax = 5f; // Maximum move duration

    private Transform player; // Reference to the player
    private Vector2 moveDirection; // Current move direction
    private Animator animator;
    private Rigidbody2D rb;
    private float moveTimer; // Timer for moving
    private float idleTimer; // Timer for idling
    private bool isMoving = false;

    private Vector2 lastPosition; // To detect if stuck
    private float stuckTimer; // Timer for stuck state

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        idleTimer = Random.Range(idleDurationMin, idleDurationMax);

        // Automatically find the player in the scene
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void Update()
    {
        if (isMoving)
        {
            MoveSnake();
        }
        else
        {
            IdleSnake();
        }
    }

    void MoveSnake()
    {
        moveTimer -= Time.deltaTime;

        // Apply velocity
        rb.velocity = moveDirection * moveSpeed;

        // Update Animator parameters for movement direction
        animator.SetFloat("MoveX", moveDirection.x);
        animator.SetFloat("MoveY", moveDirection.y);

        // Check if snake is stuck
        if (Vector2.Distance(lastPosition, transform.position) < 0.01f)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= 0.5f) // If stuck for more than 0.5 seconds
            {
                ChooseRandomDirection(); // Change direction
                stuckTimer = 0; // Reset stuck timer
            }
        }
        else
        {
            stuckTimer = 0;
        }

        lastPosition = transform.position;

        // If move time is up, switch to idle
        if (moveTimer <= 0)
        {
            isMoving = false;
            rb.velocity = Vector2.zero;
            idleTimer = Random.Range(idleDurationMin, idleDurationMax);
        }
    }

    void IdleSnake()
    {
        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0)
        {
            // If player is within detection range, move towards player
            if (player != null && Vector2.Distance(transform.position, player.position) <= detectionRange)
            {
                moveDirection = (player.position - transform.position).normalized; // Move towards player
            }
            else
            {
                ChooseRandomDirection(); // Move randomly
            }

            moveTimer = Random.Range(moveDurationMin, moveDurationMax);
            isMoving = true;
        }
    }

    void ChooseRandomDirection()
    {
        int direction = Random.Range(0, 4);

        switch (direction)
        {
            case 0: moveDirection = Vector2.up; break;
            case 1: moveDirection = Vector2.down; break;
            case 2: moveDirection = Vector2.left; break;
            case 3: moveDirection = Vector2.right; break;
        }

        moveDirection = moveDirection.normalized;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Change direction when colliding with solid tiles or objects
        if (collision.collider != null)
        {
            ChooseRandomDirection();
        }
    }
}

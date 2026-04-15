using UnityEngine;

// Ensures this GameObject has a Rigidbody2D component
[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    // Base movement speed
    public float speed = 8f;

    // Multiplier for speed adjustments (e.g., power-ups)
    public float speedMultiplier = 1f;

    // Starting movement direction
    public Vector2 initialDirection;

    // Defines which layers count as obstacles
    public LayerMask obstacleLayer;

    // Rigidbody reference (readable but not modifiable externally)
    public Rigidbody2D rb { get; private set; }

    // Current movement direction
    public Vector2 direction { get; private set; }

    // Queued direction for smoother input
    public Vector2 nextDirection { get; private set; }

    // Initial spawn position
    public Vector3 startingPosition { get; private set; }

    private void Awake()
    {
        // Cache Rigidbody reference
        rb = GetComponent<Rigidbody2D>();

        // Store initial spawn position
        startingPosition = transform.position;
    }

    private void Start()
    {
        ResetState();
    }

    public void ResetState()
    {
        // Reset movement variables
        speedMultiplier = 1f;
        direction = initialDirection;
        nextDirection = Vector2.zero;

        // Reset position
        transform.position = startingPosition;

        // Enable physics movement
        rb.isKinematic = false;
        enabled = true;
    }

    private void Update()
    {
        // Try applying queued direction for responsive controls
        if (nextDirection != Vector2.zero) {
            SetDirection(nextDirection);
        }
    }

    private void FixedUpdate()
    {
        // Physics-based movement calculation
        Vector2 position = rb.position;

        // Calculate movement translation
        Vector2 translation =
            speed * speedMultiplier *
            Time.fixedDeltaTime *
            direction;

        // Move Rigidbody smoothly
        rb.MovePosition(position + translation);
    }

    public void SetDirection(Vector2 direction, bool forced = false)
    {
        // Change direction only if path is clear
        if (forced || !Occupied(direction))
        {
            this.direction = direction;
            nextDirection = Vector2.zero;
        }
        else
        {
            // Store direction until path becomes available
            nextDirection = direction;
        }
    }

    public bool Occupied(Vector2 direction)
    {
        // Cast a box in the given direction to detect obstacles
        RaycastHit2D hit =
            Physics2D.BoxCast(
                transform.position,
                Vector2.one * 0.75f,
                0f,
                direction,
                1.5f,
                obstacleLayer
            );

        // If collider is detected, path is blocked
        return hit.collider != null;
    }
}

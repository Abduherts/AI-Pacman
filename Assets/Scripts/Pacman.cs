using UnityEngine;
using UnityEngine.InputSystem;

public enum ControlType
{
    HeuristicAI, // State Machine AI
}

/// <summary>
/// Main Pac-Man component. Handles sprite, collider, death animation.
/// AI navigation is handled by PacmanAI component.
/// </summary>
[RequireComponent(typeof(Movement))]
public class Pacman : MonoBehaviour
{
    [SerializeField]
    private AnimatedSprite deathSequence;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;
    public Movement movement { get; private set; }

    public ControlType controlType = ControlType.HeuristicAI;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        movement = GetComponent<Movement>();

        // SELF-HEALING: Ensure AI Agent is attached
        if (GetComponent<PacmanAI>() == null)
        {
            gameObject.AddComponent<PacmanAI>();
            Debug.Log("Pacman: Auto-attached missing PacmanAI component.");
        }
        
        // CRITICAL FIX: Ensure Movement has an initial direction
        // This prevents the "stuck at zero" issue
        if (movement.initialDirection == Vector2.zero)
        {
            movement.initialDirection = Vector2.left; // Classic Pac-Man starts left
            Debug.Log("Pacman: Set initial direction to LEFT");
        }
    }

    private void Update()
    {
        // Rotate pacman to face the movement direction
        if (movement.direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(movement.direction.y, movement.direction.x);
            transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
        }
    }

    public void ResetState()
    {
        enabled = true;
        spriteRenderer.enabled = true;
        circleCollider.enabled = true;
        deathSequence.enabled = false;
        movement.ResetState();
        gameObject.SetActive(true);
        
        // CRITICAL: Notify AI to restart navigation after reset
        PacmanAI ai = GetComponent<PacmanAI>();
        if (ai != null)
        {
            ai.OnPacmanReset();
        }
    }

    public void DeathSequence()
    {
        enabled = false;
        spriteRenderer.enabled = false;
        circleCollider.enabled = false;
        movement.enabled = false;
        deathSequence.enabled = true;
        deathSequence.Restart();
    }
}

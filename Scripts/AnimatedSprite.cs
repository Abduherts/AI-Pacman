using UnityEngine;

/// <summary>
/// Handles frame-based sprite animation by cycling
/// through a provided array of sprites at a fixed interval.
/// Used for animating Pac-Man and ghost characters.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class AnimatedSprite : MonoBehaviour
{
    // Array of animation frames
    public Sprite[] sprites = new Sprite[0];

    // Time between frame switches
    public float animationTime = 0.125f;

    // Determines whether animation loops
    public bool loop = true;

    private SpriteRenderer spriteRenderer;
    private int animationFrame;

    // Called when the object is first initialized
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Ensures sprite is visible when object is enabled
    private void OnEnable()
    {
        spriteRenderer.enabled = true;
    }

    // Hides sprite when object is disabled
    private void OnDisable()
    {
        spriteRenderer.enabled = false;
    }

    // Starts repeating animation cycle
    private void Start()
    {
        InvokeRepeating(nameof(Advance), animationTime, animationTime);
    }

    // Advances animation to next frame
    private void Advance()
    {
        if (!spriteRenderer.enabled)
            return;

        animationFrame++;

        // Reset animation if looping
        if (animationFrame >= sprites.Length && loop)
            animationFrame = 0;

        // Apply new sprite frame
        if (animationFrame >= 0 && animationFrame < sprites.Length)
            spriteRenderer.sprite = sprites[animationFrame];
    }

    /// <summary>
    /// Restarts animation from first frame.
    /// </summary>
    public void Restart()
    {
        animationFrame = -1;
        Advance();
    }
}

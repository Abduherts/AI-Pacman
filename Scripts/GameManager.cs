using UnityEngine;
/// Manages global game state including score, lives,
/// round resets, and game over conditions.
public class GameManager : MonoBehaviour
{
    // Reference to Pac-Man controller
    public Pacman pacman;

    // Array of all ghost instances
    public Ghost[] ghosts;

    // Parent transform containing all pellets
    public Transform pellets;

    // Current score (readable externally, modifiable internally)
    public int score { get; private set; }

    // Remaining lives
    public int lives { get; private set; }

    // Called when scene starts
    private void Start()
    {
        NewGame();
    }

    // Checks for restart input after game over
    private void Update()
    {
        if (this.lives <= 0 && Input.GetKeyDown(KeyCode.Return))
        {
            NewGame();
        }
    }

    /// Initializes a new game.
    private void NewGame()
    {
        setScore(0);
        setLives(3);
        newRound();
    }

    /// Starts a new round by resetting pellets and characters.
    private void newRound()
    {
        foreach (Transform pellet in this.pellets)
        {
            pellet.gameObject.SetActive(true);
        }

        resetState();
    }

    /// Reactivates Pac-Man and all ghosts.
    private void resetState()
    {
        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].gameObject.SetActive(true);
        }

        this.pacman.gameObject.SetActive(true);
    }

    /// Disables all gameplay objects when the game ends.

    private void gameOver()
    {
        for (int i = 0; i < this.ghosts.Length; i++)
    {
            this.ghosts[i].gameObject.SetActive(false);
        }

        this.pacman.gameObject.SetActive(false);
    }

    // Updates score safely
    private void setScore(int score)
    {
        this.score = score;
    }

    // Updates lives safely
    private void setLives(int lives)
    {
        this.lives = lives;
    }

    /// Called when a ghost is eaten.
    /// Adds ghost's point value to score.
    public void GhostEaten(Ghost ghost)
    {
        setScore(this.score + ghost.points);
    }

    /// Called when Pac-Man is eaten.
    /// Reduces lives and either resets the round or ends the game.
    public void PacmanEaten()
    {
        this.pacman.gameObject.SetActive(false);
        setLives(this.lives - 1);

        if (this.lives > 0)
        {
            Invoke(nameof(resetState), 3.0f);
        }
        else
        {
            gameOver();
        }
    }
}

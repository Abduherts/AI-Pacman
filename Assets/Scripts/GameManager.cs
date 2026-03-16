using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Ghost[] ghosts;
    [SerializeField] private Pacman pacman;
    public Transform pellets; // Public for AI access
    [SerializeField] private Text gameOverText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text livesText;

    public int score { get; private set; } = 0;
    public int lives { get; private set; } = 3;

    public int round = 1;

    // Events for RL Agent (Disabled)
    // public System.Action<int> OnScoreChange;
    // public System.Action OnPacmanDeath;
    // public System.Action OnLevelComplete;

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void Start()
    {
        NewGame();
    }

    private void Update()
    {
        if (lives <= 0 && Keyboard.current.anyKey.wasPressedThisFrame) {
            NewGame();
        }
    }

    private void NewGame()
    {
        SetScore(0);
        SetLives(3);
        gameOverText.enabled = false; // Hide UI
        NewRound();
    }

    private void NewRound()
    {
        foreach (Transform pellet in pellets) {
            pellet.gameObject.SetActive(true);
        }
        
        ResetState();
        
        // Start Metrics
        if (PerformanceMetrics.Instance != null)
        {
            PerformanceMetrics.Instance.StartTracking();
        }
    }

    private void ResetState()
    {
        for (int i = 0; i < ghosts.Length; i++) {
            ghosts[i].ResetState();
        }

        pacman.ResetState();
    }

    private void GameOver()
    {
        for (int i = 0; i < ghosts.Length; i++) {
            ghosts[i].gameObject.SetActive(false);
        }

        pacman.gameObject.SetActive(false);
        gameOverText.enabled = true;

        // Stop Metrics and Report
        if (PerformanceMetrics.Instance != null)
        {
            PerformanceMetrics.Instance.StopTracking();
        }
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        livesText.text = "x" + lives.ToString();
    }

    private void SetScore(int score)
    {
        int change = score - this.score;
        this.score = score;
        scoreText.text = score.ToString().PadLeft(2, '0');
        
        // Notify RL agent
        // if (change > 0) OnScoreChange?.Invoke(change);
    }

    public void PacmanEaten()
    {
        pacman.DeathSequence();
        SetLives(lives - 1);

        // OnPacmanDeath?.Invoke(); // Notify RL (Disabled)
        
        if (lives > 0) {
            Invoke(nameof(ResetState), 3.0f);
        } else {
            GameOver();
        }
    }

    public void GhostEaten(Ghost ghost)
    {
        int points = ghost.points;
        SetScore(score + points);
    }

    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);
        SetScore(score + pellet.points);

        if (!HasRemainingPellets())
        {
            pacman.gameObject.SetActive(false);
            // OnLevelComplete?.Invoke(); // Notify RL (Disabled)
            Invoke(nameof(NewRound), 3.0f);
        }
    }

    public void PowerPelletEaten(PowerPellet pellet)
    {
        for (int i = 0; i < ghosts.Length; i++) {
            ghosts[i].frightened.Enable(pellet.duration);
        }

        PelletEaten(pellet);
    }

    private bool HasRemainingPellets()
    {
        foreach (Transform pellet in pellets)
        {
            if (pellet.gameObject.activeSelf) {
                return true;
            }
        }

        return false;
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private Button restartButton;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TextMeshProUGUI[] highScoreTexts; // Size 5 in inspector

    private const int maxHighScores = 5;
    private string[] highScoreNames = new string[maxHighScores];
    private int[] highScoreValues = new int[maxHighScores];

    private bool isPaused = false;

    // Array of Ghosts
    public Ghost[] ghosts;

    public Pacman Pacman;

    public Transform pellets;

    public int ghostMultiplier { get; private set; } = 1; // Default 1

    // Public Getter - Private Setter
    // If you need to access and read the currest score you can, but you cannot set it - it will be set automatically through game events
    public int score { get; private set; }
    public int lives { get; private set; }

    // Unity calls Start automatically
    private void Start()
    {
        /* Show start screen in beginning
        if (startScreen != null) { startScreen.SetActive(true); }*/
        if (gameOverScreen != null) { gameOverScreen.SetActive(false); }

        // Deactivate Pacman and ghosts until the game starts
        if (Pacman != null) Pacman.gameObject.SetActive(false);
        if (ghosts != null)
        {
            foreach (var ghost in ghosts)
                ghost.gameObject.SetActive(false);
        }

        NewGame();

        // Hook Restart button (game over) to restart
        if (restartButton != null) { 
            restartButton.onClick.AddListener(NewGame);
        }

        LoadHighScores();
        UpdateHighScoreUI();
    }

    // Called every frame the game is running by unity automatically
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    public void SubmitHighScore(string _)
    {
        SubmitHighScore();
    }

    public void SubmitHighScore()
    {
        string playerName = nameInputField.text; 

        if (string.IsNullOrWhiteSpace(playerName)) { playerName = "PLAYER"; }

        TryInsertHighScore(playerName, score);

        SaveHighScores();
        UpdateHighScoreUI();

        nameInputField.gameObject.SetActive(false);
    }

    private void TryInsertHighScore(string playerName, int newScore)
    {
        for (int i = 0; i < maxHighScores; i++)
        {
            if (newScore > highScoreValues[i])
            {
                // Shift down
                for (int j = maxHighScores - 1; j > i; j--)
                {
                    highScoreValues[j] = highScoreValues[j - 1];
                    highScoreNames[j] = highScoreNames[j - 1];
                }

                highScoreValues[i] = newScore;
                highScoreNames[i] = playerName;
                break;
            }
        }
    }

    private void LoadHighScores()
    {
        for (int i = 0; i < maxHighScores; i++)
        {
            highScoreNames[i] = PlayerPrefs.GetString($"HS_Name{i}", "---");
            highScoreValues[i] = PlayerPrefs.GetInt($"HS_Value{i}", 0);
        }
    }

    private void SaveHighScores()
    {
        for (int i = 0; i < maxHighScores; i++)
        {
            PlayerPrefs.SetString($"HS_Name_{i}", highScoreNames[i]);
            PlayerPrefs.SetInt($"HS_Value_{i}", highScoreValues[i]);
        }
        PlayerPrefs.Save();
    }

    private void UpdateHighScoreUI()
    {
        for (int i = 0; i < highScoreTexts.Length; i++)
        {
            highScoreTexts[i].text = $"{i + 1}. {highScoreNames[i]} - {highScoreValues[i]}";
        }
    }

    private void PauseGame()
    {
        isPaused = true;

        // Show pause screen
        if (pauseScreen != null) {  pauseScreen.SetActive(true); }

        // Disable ghost movement
        foreach (var ghost in ghosts)
        { 
            if (ghost != null && ghost.movement != null) { ghost.movement.enabled = false; } 
        }

        // Disable Pacman Movement
        if (Pacman != null) { Pacman.movement.enabled = false; }

        // Stop physics updates
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        isPaused = false;

        // Hide pause screen
        if (pauseScreen != null) { pauseScreen.SetActive(false); }

        // Enable pacman Movement
        if (Pacman != null) { Pacman.movement.enabled = true; }

        // Enable ghost movement
        foreach (var ghost in ghosts)
        {
            if (ghost != null && ghost.movement != null) { ghost.movement.enabled = true; }
        }

        // Resume physics updates
        Time.timeScale = 1f;
    }

    private void NewGame()
    {
        // Hide Game over screen so restart button disappears on new game
        if(gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }

        // Set score and lives back to default
        SetScore(0);
        SetLives(3);
        NewRound();
    }

    private void NewRound()
    {
        foreach (Transform pellet in this.pellets)
        {
            pellet.gameObject.SetActive(true); // Turn them back on
        }

        ResetState();

    }

    // This function needs to be separate from NewRound because when PacMan dies, you want to reset pacman and ghosts but not pellets
    private void ResetState()
    {
        ResetGhostMultiplier();
        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].ResetState();

            // If movement script disables:
            if (ghosts[i].movement != null && !ghosts[i].movement.enabled)
            {
                ghosts[i].movement.enabled = true;
            }
        }

        // Set Pacman Active too
        this.Pacman.ResetState();
    }

    private void GameOver()
    {
        // On Game over, set them to false to deactivate them.
        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].gameObject.SetActive(false);
        }

        this.Pacman.gameObject.SetActive(false);

        // Show Game over screen
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }

        if (score > highScoreValues[maxHighScores - 1])
        {
            if (nameInputField != null)
            {
                nameInputField.gameObject.SetActive(true);
                nameInputField.text = "";
                nameInputField.ActivateInputField();
            }
        }
    }

    private void SetScore(int score)
    {
        this.score = score;

        // Update UI
        if (scoreText != null) { scoreText.text = "Score: " + this.score; }
    }

    private void SetLives(int lives)
    {
        this.lives = lives;

        // Update UI
        if (livesText != null) { livesText.text = "Lives: " + this.lives; }
    }

    // Public functions for GhostEaten and PacmanEaten because they will be triggered from other scripts
    public void GhostEaten(Ghost ghost)
    {
        int points = ghost.points * this.ghostMultiplier; // Calculate points for eating ghost based off base amount * multiplier
        SetScore(this.score + points); // Add score
        this.ghostMultiplier++; // Increment multiplier
    }

    public void PacmanEaten()
    {
        // Deactivate Pacman and subtract a life
        this.Pacman.gameObject.SetActive(false);
        SetLives(this.lives - 1);

        if(this.lives > 0)
        {
            // Wait 3 seconds between Pacman dying and resetting state of pacman and ghosts
            Invoke(nameof(ResetState), 3.0f);
        }
        else
        {
            GameOver();
        }
    }

    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false); // Make it disappear
        SetScore(this.score + pellet.points); // Add score

        // Check if you've eaten all the pellets
        if(!HasRemainingPellets())
        {
            this.Pacman.gameObject.SetActive(false); // That way a Ghost can't come and eat you 
            Invoke(nameof(NewRound), 3.0f);
        }
    }

    public void PowerPelletEaten(PowerPellet pellet)
    {
        // Change Ghost State make ghosts vulnerable
        for(int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].frightened.Enable(pellet.duration);
        }

        // Anything that should happen when you eat a normal pellet should happen when you eat a normal pellet, so:
        PelletEaten(pellet);
        // If you eat another power pellet during the duration of it, reset duration of power mode while keeping ghost multiplier
        CancelInvoke();
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);
        

        
    }

    private bool HasRemainingPellets()
    {
        foreach (Transform pellet in this.pellets)
        {
            if(pellet.gameObject.activeSelf) // Meaning if there are remaining pellets
            {
                return true;
            }
        }
        // Return false if the code makes it through the loop (meaning no pellets left)
        return false; 
    }

    private void ResetGhostMultiplier()
    {
        this.ghostMultiplier = 1;
    }
}

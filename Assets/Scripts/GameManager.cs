using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private GameObject pauseScreen;

    private bool isPaused = false;
    public static GameManager instance;
    

    // Array of Ghosts
    public Ghost[] ghosts;

    public Pacman Pacman;

    public Transform pellets;

    public int ghostMultiplier { get; private set; } = 1; // Default 1

    // Public Getter - Private Setter
    // If you need to access and read the currest score you can, but you cannot set it - it will be set automatically through game events
    public int score { get; private set; }
    public int lives { get; private set; }

    public bool startingNewGame = false;

    // Unity calls Start automatically
    private void Start()
    {
        /*
        // Deactivate Pacman and ghosts until the game starts
        if (Pacman != null) Pacman.gameObject.SetActive(false);
        if (ghosts != null)
        {
            foreach (var ghost in ghosts)
                ghost.gameObject.SetActive(false);
        }

        NewGame();*/
        
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

    // COPILOT
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // COPILOT
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ignore start screen (index = 0)
        if (scene.buildIndex == 0) { return; }

        // Re-find pellets, Pacman, ghosts, and UI in the new scene
        GameObject pelletParent = GameObject.Find("Pellets");
        if (pelletParent != null)
        {
            pellets = pelletParent.transform;
        }

        Pacman = FindObjectOfType<Pacman>();
        ghosts = FindObjectsOfType<Ghost>();

        // Reconnect UI
        scoreText = GameObject.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
        livesText = GameObject.Find("LivesText")?.GetComponent<TextMeshProUGUI>();

        pauseScreen = GameObject.Find("PauseScreen");

        // Update UI with current values
        if (scoreText != null) { scoreText.text = "Score: " + score; }
        if (livesText != null) { livesText.text = "Lives: " + lives; }

        if (startingNewGame)
        {
            NewGame();
            startingNewGame = false;
        }
        else
        {
            // Start new round on new map
            NewRound();
        }
    }

    private void LoadNextLevel()
    {
        int current = SceneManager.GetActiveScene().buildIndex;
        
        if (current == 1)
        {
            SceneManager.LoadScene(2);
        }
        if (current == 2)
        {
            SceneManager.LoadScene(1);
        }
    }


    private void PauseGame()
    {
        isPaused = true;

        // Show pause screen
        ShowPanel(pauseScreen);

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

    public void ResumeGame()
    {
        isPaused = false;
        HidePanel(pauseScreen);

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

    private void ShowPanel(GameObject panel)
    {
        if (panel == null) { return; }

        var cg = panel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
    }

    private void HidePanel(GameObject panel)
    {
        if (panel == null) { return; }

        var cg = panel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
    }

    private void NewGame()
    {


        // Set score and lives back to default
        SetScore(0);
        SetLives(3);
        NewRound();
    }

    private void NewRound()
    {
        // COPILOT
        if (this.pellets != null)
        {
            foreach (Transform pellet in this.pellets)
            {
                if (pellet != null)
                {
                    pellet.gameObject.SetActive(true);
                }
            }
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
        }

        // Set Pacman Active too
        if (Pacman != null) { this.Pacman.ResetState(); }
    }

    private void GameOver()
    {
        // Play game over sfx
        AudioManager.instance.PlaySFX(AudioManager.instance.sfxGameOver);

        // Play death animation
        Pacman.PlayDeathAnimation();

        // On Game over, set them to false to deactivate them.
        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].gameObject.SetActive(false);
        }

        //this.Pacman.gameObject.SetActive(false);

        // Delay scene load so death animation can play
        Invoke(nameof(LoadEndScreen), 3.0f);
    }

    private void LoadEndScreen()
    {
        // Load Game Over Scene
        SceneManager.LoadScene("EndScreen");
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
        // Play sound for ghost being eaten
        AudioManager.instance.PlaySFX(AudioManager.instance.sfxGhostEaten);
        int points = ghost.points * this.ghostMultiplier; // Calculate points for eating ghost based off base amount * multiplier
        SetScore(this.score + points); // Add score
        this.ghostMultiplier++; // Increment multiplier
    }

    public void PacmanEaten()
    {
        // Deactivate Pacman and subtract a life
        //this.Pacman.gameObject.SetActive(false);
        SetLives(this.lives - 1);

        // Play death animation and play death sound
        Pacman.PlayDeathAnimation();
        AudioManager.instance.PlaySFX(AudioManager.instance.sfxPacmanDeath);

        if (this.lives > 0)
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
            //Invoke(nameof(NewRound), 3.0f);
            Invoke(nameof(LoadNextLevel), 2.0f); // COPILOT
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

        // Play sound for eating power pellet
        AudioManager.instance.PlaySFX(AudioManager.instance.sfxPowerPellet);

        // If you eat another power pellet during the duration of it, reset duration of power mode while keeping ghost multiplier
        CancelInvoke();
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);

        // Switch music to power pellet music
        AudioManager.instance.PlayPowerPelletMusic();
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
        
        // Multiplier gets reset when duration is over, so switch back to base music
        AudioManager.instance.PlayBaseMusic();
    }
}

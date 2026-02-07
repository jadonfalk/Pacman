using UnityEngine;

public class GameManager : MonoBehaviour
{
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
        NewGame();
    }

    // Called every frame the game is running by unity automatically
    private void Update()
    {
        // If game is over (lives <= 0) and you press any key, restart game
        if (this.lives <= 0 && Input.anyKeyDown)
        {
            NewGame();
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
            this.ghosts[i].gameObject.SetActive(true);
        }

        // Set Pacman Active too
        this.Pacman.gameObject.SetActive(true);
    }

    private void GameOver()
    {
        // On Game over, set them to false to deactivate them.
        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].gameObject.SetActive(false);
        }

        this.Pacman.gameObject.SetActive(false);
    }

    private void SetScore(int score)
    {
        this.score = score;
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
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
        // TO-DO: Change Ghost State make ghosts vulnerable

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

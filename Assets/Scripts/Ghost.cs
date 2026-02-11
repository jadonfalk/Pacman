using UnityEngine;

public class Ghost : MonoBehaviour
{
    public Movement movement {  get; private set; }
    public GhostHome home { get; private set; }
    public GhostScatter scatter { get; private set; }
    public GhostChase chase { get; private set; }
    public GhostFrightened frightened { get; private set; }

    public GhostBehavior initialBehavior;
    public Transform target; // Target object that you're chasing/running from (in this case Pacman)

    public int points = 200;

    private void Awake()
    {
        this.movement = GetComponent<Movement>();
        this.home = GetComponent<GhostHome>();
        this.scatter = GetComponent<GhostScatter>();
        this.chase = GetComponent<GhostChase>();
        this.frightened = GetComponent<GhostFrightened>();
    }

    private void Start()
    {
        ResetState();
    }

    public void ResetState()
    {
        this.gameObject.SetActive(true);
        this.movement.ResetState();

        // None of the ghosts will ever start in Frightened Mode or Chase Mode
        this.frightened.Disable();
        this.chase.Disable();

        // They all default start with scatter
        this.scatter.Enable();

        if (this.home != initialBehavior) { this.home.Disable(); }
        if (this.initialBehavior != null) { this.initialBehavior.Enable(); }
        //if (this.initialBehavior == this.home) { this.home.ExitHome(); }
    }

    // Collisions, not triggers
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            if(this.frightened.enabled) {
                Debug.Log($"{name} eaten by Pacman while frightened.");
                FindObjectOfType<GameManager>().GhostEaten(this);
            } else {
                Debug.Log($"{name} collided with Pacman, calling PacmanEaten.");
                FindObjectOfType<GameManager>().PacmanEaten();
            }
        }
    }

}

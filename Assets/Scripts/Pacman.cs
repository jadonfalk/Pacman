using UnityEngine;

public class Pacman : MonoBehaviour
{
    public Movement movement {  get; private set; }
    public SpriteRenderer body;
    public GameObject death;

    private void Awake()
    {
        this.movement = GetComponent<Movement>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
            this.movement.SetDirection(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
            this.movement.SetDirection(Vector2.down);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
            this.movement.SetDirection(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
            this.movement.SetDirection(Vector2.right);
        }

        /*
        // Angle of movement direction you're going in
        float angle = Mathf.Atan2(this.movement.direction.y, this.movement.direction.x);
        // Assign angle to rotation
        this.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward); */
    }

    public void ResetState()
    {
        // Restore gameplay
        GetComponent<Collider2D>().enabled = true;
        movement.enabled = true;

        // Restore normal body sprite/animation & hide death one
        body.enabled = true;
        death.GetComponent<SpriteRenderer>().enabled = false;

        this.gameObject.SetActive(true);
        this.movement.ResetState();
    }

    public void PlayDeathAnimation()
    {
        // Make it so Pacman can't collide with ghosts while in death animation
        GetComponent<Collider2D>().enabled = false;
        movement.enabled = false;

        // Disable normal animation
        body.enabled = false;

        // Enable death animation
        var deathRenderer = death.GetComponent<SpriteRenderer>();
        var deathAnim = death.GetComponent<AnimatedSprite>();

        deathRenderer.enabled = true;
        deathAnim.loop = false;

        deathAnim.Restart();
    }
}

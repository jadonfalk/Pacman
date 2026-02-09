using UnityEngine;

public class GhostFrightened : GhostBehavior
{
    public SpriteRenderer body;
    public SpriteRenderer eyes;
    public SpriteRenderer blue;
    public SpriteRenderer white;

    public bool eaten {  get; private set; }

    public override void Enable(float duration)
    {
        base.Enable(duration);

        this.body.enabled = false;
        this.eyes.enabled = false;
        this.blue.enabled = true;
        this.white.enabled = false;

        // Start flashing when duration is at 50%
        Invoke(nameof(Flash), duration / 2.0f);
    }

    public override void Disable()
    {
        base.Disable();

        this.body.enabled = true;
        this.eyes.enabled = true;
        this.blue.enabled = false;
        this.white.enabled = false;
    }

    private void Flash()
    {
        if(!this.eaten)
        {
            this.blue.enabled = false;
            this.white.enabled = true;
            this.white.GetComponent<AnimatedSprite>().Restart(); // reset animation
        }
    }

    private void Eaten()
    {
        this.eaten = true;

        Vector3 position = this.ghost.home.inside.position;
        position.z = this.ghost.transform.position.z;
        this.ghost.transform.position = position; // Teleport to home rather than traverse to it (simplicity)

        // Ensure they don't leave home before frightened state is up
        this.ghost.home.Enable(this.duration);

        this.body.enabled = false;
        this.eyes.enabled = true;
        this.blue.enabled = false;
        this.white.enabled = false;
    }

    private void OnEnable()
    {
        // Ghost moves 50% slower when frightened
        this.ghost.movement.speedMultiplier = 0.5f;
        this.eaten = false;
    }

    private void OnDisable()
    {
        this.ghost.movement.speedMultiplier = 1.0f;
        this.eaten = false;
    }

    // Frightened Movmeent Logic : Pretty much opposite of ghost chase 
    private void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();

        if (node != null && this.enabled)
        {
            Vector2 direction = Vector2.zero;
            float maxDistance = float.MinValue; // Initially the smallest value possible so the very first direction is always more than float.MinValue

            // Loop through available directions at this node and then calculate if moving in that direction puts you closer or further from Target (Pacman)
            foreach (Vector2 availableDirection in node.availableDirections)
            {
                Vector3 newPosition = this.transform.position + new Vector3(availableDirection.x, availableDirection.y, 0.0f); // Don't set Z since that's used for draw order in 2D games
                float distance = (this.ghost.target.position - newPosition).sqrMagnitude;

                // Loop through all and eventually be left with whatever is the direction that puts you closest to target
                if (distance > maxDistance)
                {
                    direction = availableDirection;
                    maxDistance = distance;
                }
            }

            this.ghost.movement.SetDirection(direction);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            if (this.enabled)
            {
                Eaten();
            }
        }
    }

}

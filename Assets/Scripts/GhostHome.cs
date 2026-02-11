using UnityEngine;
using System.Collections;

public class GhostHome : GhostBehavior
{
    public Transform inside;
    public Transform outside;

    private void OnEnable()
    {
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        // If statement ensures Coroutine isn't called when object is destroyed
        if(this.gameObject.activeSelf)
        {
            StartCoroutine(ExitTransition());
        }
    }

    public void ExitHome()
    {
        StartCoroutine(ExitTransition());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(this.enabled && collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            this.ghost.movement.SetDirection(-this.ghost.movement.direction);
        }
    }

    // Coroutine allows you to pause execution temporarily 
    private IEnumerator ExitTransition()
    {
        this.ghost.movement.SetDirection(Vector2.up, true); // Force direction
        // Set kinematic to true (turn off physics and collision while doing this transition)
        this.ghost.movement.rigidbody.bodyType = RigidbodyType2D.Kinematic;
        this.ghost.movement.enabled = false; 

        // Animating position of Ghost
        Vector3 position = this.transform.position;
        float duration = 0.5f;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            Vector3 newPosition = Vector3.Lerp(position, this.inside.position, elapsed / duration);
            newPosition.z = position.z;
            this.ghost.transform.position = newPosition;
            elapsed += Time.deltaTime;
            yield return null; // Wait a frame then return where it left off until elapsed is greater than duration
        }

        elapsed = 0.0f; // Reset elapsed time in between as if its a brand new animation

        while (elapsed < duration)
        {
            Vector3 newPosition = Vector3.Lerp(this.inside.position, this.outside.position, elapsed / duration);
            newPosition.z = position.z;
            this.ghost.transform.position = newPosition;
            elapsed += Time.deltaTime;
            yield return null; // Wait a frame then return where it left off until elapsed is greater than duration
        }

        // random direction | less than half -> left , greater than half -> right
        this.ghost.movement.SetDirection(new Vector2(Random.value < 0.5f ? -1.0f : 1.0f, 0.0f), true);
        this.ghost.movement.rigidbody.bodyType = RigidbodyType2D.Dynamic;
        this.ghost.movement.enabled = true;

    }
}

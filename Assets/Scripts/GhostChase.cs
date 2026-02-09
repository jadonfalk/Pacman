using UnityEngine;

public class GhostChase : GhostBehavior
{
    // Unity calls this function automatically on disable
    private void OnDisable()
    {
        this.ghost.scatter.Enable(); // When chase is disabled, switch to scatter
    }

    // Check when Ghost hits node
    private void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();

        if (node != null && this.enabled && !this.ghost.frightened.enabled)
        {
            Vector2 direction = Vector2.zero;
            float minDistance = float.MaxValue; // Initially the largest value possible so the very first direction is always less than float.MaxValue

            // Loop through available directions at this node and then calculate if moving in that direction puts you closer or further from Target (Pacman)
            foreach (Vector2 availableDirection in node.availableDirections)
            {
                Vector3 newPosition = this.transform.position + new Vector3(availableDirection.x, availableDirection.y, 0.0f); // Don't set Z since that's used for draw order in 2D games
                float distance = (this.ghost.target.position - newPosition).sqrMagnitude;

                // Loop through all and eventually be left with whatever is the direction that puts you closest to target
                if (distance < minDistance)
                {
                    direction = availableDirection;
                    minDistance = distance;
                }
            }

            this.ghost.movement.SetDirection(direction);
        }
    }
}

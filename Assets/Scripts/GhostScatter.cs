using UnityEngine;

public class GhostScatter : GhostBehavior
{
    // Unity calls this function automatically on disable
    private void OnDisable()
    {
        this.ghost.chase.Enable();
    }

    // Check when Ghost hits node
    private void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();

        // Even though you can be in scatter and frightened mode simultaneously, Frightened mode movement will override scatter movement
        // If there is a node & Scatter is enabled & Ghost isn't frightened:
        if (node != null && this.enabled && !this.ghost.frightened.enabled) 
        {
            // Pick random direction that's available from the node
            int index = Random.Range(0, node.availableDirections.Count);

            // Make it so Ghost doesn't backtrack and go the direction it was just coming from (assuming there are more than 1 available directions)
            if (node.availableDirections[index] == -this.ghost.movement.direction && node.availableDirections.Count > 1)
            {
                index++;

                // Prevent out of bounds
                if(index >= node.availableDirections.Count)
                {
                    index = 0;
                }
            }

            // Set ghost direction
            this.ghost.movement.SetDirection(node.availableDirections[index]);
        }
    }

}

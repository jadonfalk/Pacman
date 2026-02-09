using UnityEngine;
using System.Collections.Generic;

public class Node : MonoBehaviour
{
    public LayerMask obstacleLayer;
    public List<Vector2> availableDirections {  get; private set; }
    // Determine available direction

    public void Start()
    {
        // Initialize list
        this.availableDirections = new List<Vector2>();

        // Check all directions
        CheckAvailableDirection(Vector2.up);
        CheckAvailableDirection(Vector2.down);
        CheckAvailableDirection(Vector2.left);
        CheckAvailableDirection(Vector2.right);
    }

    private void CheckAvailableDirection(Vector2 direction)
    {
        // Boxcast
        RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, Vector2.one * 0.5f, 0.0f, direction, 1.0f, this.obstacleLayer);
        
        // If collider didn't hit anything, direction is available and add to available directions list
        if(hit.collider == null)
        {
            this.availableDirections.Add(direction);
        }
    }
}

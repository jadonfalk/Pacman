using UnityEngine;

// Inherit from Pellet
public class PowerPellet : Pellet
{
    // Duration of power pellet
    public float duration = 8.0f;

    // Overriding to call a different function
    protected override void Eat()
    {
        FindObjectOfType<GameManager>().PowerPelletEaten(this);
    }
}

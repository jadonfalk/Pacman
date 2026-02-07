using UnityEngine;

public class Pellet : MonoBehaviour
{
    public int points = 10;

    // Protected allows PowerPellet script to inherit it -- virtual allows it to be overriden
    protected virtual void Eat()
    {
        FindObjectOfType<GameManager>().PelletEaten(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ensure that the pellets are only eaten when colliding with Pacman
        if(other.gameObject.layer == LayerMask.NameToLayer("Pacman")) {
            Eat();
        }
    }
}

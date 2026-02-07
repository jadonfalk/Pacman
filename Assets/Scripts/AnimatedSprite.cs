using UnityEngine;

// In order to have an animated sprite, you must have the component of a Sprite Renderer on that object
[RequireComponent(typeof(SpriteRenderer))]

public class AnimatedSprite : MonoBehaviour
{
    public SpriteRenderer spriteRenderer { get; private set; }
    public Sprite[] sprites;
    public float animationTime = 0.25f; // every 0.25s increment to next sprite
    public int animationFrame {  get; private set; }
    public bool loop = true; 
    
    // Called automatically when object is initialized
    private void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(Advance), this.animationTime, this.animationTime);
    }

    private void Advance()
    {
        // Don't continue animation if sprite renderer is disabled
        if(!this.spriteRenderer.enabled)
        {
            return;
        }

        // Increment frame
        this.animationFrame++;

        // If it's overflowed and is looping, set it back to frame 0
        if(this.animationFrame >= this.sprites.Length && this.loop)
        {
            this.animationFrame = 0;
        }

        // Make sure never get index out of bounds exception
        if(this.animationFrame >= 0 && this.animationFrame < this.sprites.Length)
        {
            this.spriteRenderer.sprite = this.sprites[this.animationFrame];
        }
    }

    public void Restart()
    {
        // Instead of setting to 0 and readding code to set sprite, set to -1 and call Advance since it increments in the beginning
        this.animationFrame = -1;
        Advance();
    }
}

using UnityEngine;

[RequireComponent(typeof(Ghost))]

public abstract class GhostBehavior : MonoBehaviour
{
    // Basic functions for enabling/disabling different behaviors

    public Ghost ghost { get; private set;}
    public float duration;

    private void Awake()
    {
        this.ghost = GetComponent<Ghost>();
        this.enabled = false; // Behaviors are disabled by default
    }
    
    public void Enable()
    {
        Enable(this.duration);
    }

    public virtual void Enable(float duration)
    {
        this.enabled = true;

        CancelInvoke(); // Every time function is called, cancel invokes so if you eat another power pellet while in the duration of another, resets duration

        // Invoke disable after duration
        Invoke(nameof(Disable), duration);
    }

    public virtual void Disable()
    {
        this.enabled = false;

        CancelInvoke();
    }


}

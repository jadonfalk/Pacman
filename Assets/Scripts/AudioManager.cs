using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Music Sources")]
    public AudioSource musicSource;

    [Header("SFX Source")]
    public AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip baseMusic;
    public AudioClip powerPelletMusic;

    [Header("Sound Effects")]
    public AudioClip sfxPacmanDeath;
    public AudioClip sfxMenuClick;
    public AudioClip sfxPowerPellet;
    public AudioClip sfxGhostEaten;
    public AudioClip sfxGameOver;

    private bool musicEnabled = true;
    private bool soundEnabled = true;

    // Exposes whether music/sound is currently enabled (read‑only)
    public bool MusicEnabled => musicEnabled; 
    public bool SoundEnabled => soundEnabled;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        PlayBaseMusic();
    }

    public void PlayBaseMusic()
    {
        if (!musicEnabled) return;

        musicSource.clip = baseMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayPowerPelletMusic()
    {
        if (!musicEnabled) return;

        musicSource.clip = powerPelletMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void ToggleMusic(bool enabled)
    {
        musicEnabled = enabled;
        PlayerPrefs.SetInt("MusicEnabled", enabled ? 1 : 0);

        if (enabled)
        {
            PlayBaseMusic();
        }
        else
        {
            musicSource.Stop();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (!soundEnabled || clip == null) { return; }
        sfxSource.PlayOneShot(clip);
    }

    public void ToggleSound(bool enabled)
    {
        soundEnabled = enabled;
    }

}

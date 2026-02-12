using UnityEngine;
using UnityEngine.UI;

public class PauseOptions : MonoBehaviour
{
    public Toggle toggleMusic;
    public Toggle toggleFullScreen;
    public Toggle toggleSound;
    public Button resumeButton;

    private bool initialized = false;

    private void Start()
    {
        // Sync toggles with AudioManager state
        toggleMusic.isOn = AudioManager.instance.MusicEnabled;
        toggleSound.isOn = AudioManager.instance.SoundEnabled;
        toggleFullScreen.isOn = Screen.fullScreen;

        // Hook up listeners
        toggleMusic.onValueChanged.AddListener(OnToggleMusic);
        toggleFullScreen.onValueChanged.AddListener(OnToggleFullScreen);
        toggleSound.onValueChanged.AddListener(OnToggleSound);

        resumeButton.onClick.AddListener(() =>
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.sfxMenuClick);
            GameManager.instance.ResumeGame();
        });

        initialized = true;
    }

    private void OnToggleMusic(bool value)
    {
        if (!initialized) return;

        AudioManager.instance.PlaySFX(AudioManager.instance.sfxMenuClick);
        AudioManager.instance.ToggleMusic(value);
    }

    private void OnToggleFullScreen(bool value)
    {
        if (!initialized) return;

        AudioManager.instance.PlaySFX(AudioManager.instance.sfxMenuClick);
        Screen.fullScreen = value;
    }

    private void OnToggleSound(bool value)
    {
        if (!initialized) return;

        AudioManager.instance.PlaySFX(AudioManager.instance.sfxMenuClick);
        AudioManager.instance.ToggleSound(value);
    }
}

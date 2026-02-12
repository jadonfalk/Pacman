using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public CanvasGroup optionsTab;

    [Header("Buttons")]
    public Button buttonOptions;
    public Button backButton;

    [Header("Toggles")]
    public Toggle toggleMusic;
    public Toggle toggleFullScreen;
    public Toggle toggleSound;

    private bool initialized = false;

    private void Start()
    {
        // Hide options tab at start
        Hide(optionsTab, false);

        // Hook up buttons
        buttonOptions.onClick.AddListener(() => Show(optionsTab)); 
        backButton.onClick.AddListener(() => Hide(optionsTab));

        // Hook up toggles
        toggleMusic.onValueChanged.AddListener(OnToggleMusic); 
        toggleFullScreen.onValueChanged.AddListener(OnToggleFullScreen); 
        toggleSound.onValueChanged.AddListener(OnToggleSound);

        // Mark initialization after listeners are added
        initialized = true;
    }

    private void Show(CanvasGroup cg, bool playSound = true)
    {
        cg.alpha = 1.0f;
        cg.interactable = true;
        cg.blocksRaycasts = true;

        if (initialized && playSound)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.sfxMenuClick);
        }
    }

    private void Hide(CanvasGroup cg, bool playSound = true)
    {
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        if (initialized && playSound)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.sfxMenuClick);
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit button pressed");

        // Play click sound
        AudioManager.instance.PlaySFX(AudioManager.instance.sfxMenuClick);

        // Quit the application
        Application.Quit();
    }

    private void OnToggleMusic(bool value)
    {
        if (!initialized) { return; }
        Debug.Log("Music Toggled: " + value);
        AudioManager.instance.ToggleMusic(value);
        AudioManager.instance.PlaySFX(AudioManager.instance.sfxMenuClick);
    }

    private void OnToggleFullScreen(bool value)
    {
        if (!initialized) { return; }
        Screen.fullScreen = value;
        Debug.Log("Fullscreen: " + value);
        AudioManager.instance.PlaySFX(AudioManager.instance.sfxMenuClick);
    }

    private void OnToggleSound(bool value)
    {
        if (!initialized) { return; }
        Debug.Log("Sound FX toggled: " + value);
        AudioManager.instance.PlaySFX(AudioManager.instance.sfxMenuClick); // Click sound

        // Toggle sound in audio manager
        AudioManager.instance.ToggleSound(value);
    }
}

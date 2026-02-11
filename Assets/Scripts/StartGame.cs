using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [SerializeField] private Button startButton;


    private void Start()
    {
        //Hook Start button to begin game
        if (startButton != null)
        {
            startButton.onClick.AddListener(() =>
            {
                GameManager.instance.startingNewGame = true;
                SceneManager.LoadScene("Level2");
            });
        }
    }
}

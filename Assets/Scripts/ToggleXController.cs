using UnityEngine;
using UnityEngine.UI;

public class ToggleXController : MonoBehaviour
{
    public Toggle toggle;      // assign the Toggle
    public Text xText;         // assign the Text component on the X object

    private void Awake()
    {
        UpdateX(toggle.isOn);
        toggle.onValueChanged.AddListener(UpdateX);
    }

    private void UpdateX(bool isOn)
    {
        xText.gameObject.SetActive(isOn);

        if (isOn)
            xText.color = Color.black;
        else
            xText.color = Color.white;
    }
}

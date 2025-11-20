using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public static event System.Action OnSettingsClosed; // Avisa a otros menus que este esta cerrado

    public Slider volumeSlider;
    public Slider musicSlider;
    public Slider brightnessSlider;

    public void CloseSettings()
    {
        OnSettingsClosed?.Invoke();

        gameObject.SetActive(false);
    }

    public void SetVolume(float value)
    {
        Debug.Log("Volume set to: " + value);
    }

    public void SetMusic(float value)
    {
        if (AudioManager.instance != null)
            AudioManager.instance.SetMusicVolume(value);
    }

    public void SetBrightness(float value)
    {
        Debug.Log("Brightness set to: " + value);
    }
}

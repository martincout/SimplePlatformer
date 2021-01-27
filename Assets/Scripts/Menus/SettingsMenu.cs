using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider slider;
    private float maxVolume;
    public AudioSource BGM;

    private void Start()
    {
        maxVolume = BGM.volume;
        SetMaxVolume(maxVolume);
    }

    public void SetMaxVolume(float _maxVolume)
    {
        slider.maxValue = _maxVolume;
        slider.value = _maxVolume;
    }

    public void SetVolume(float volume)
    {
        BGM.volume = volume;
    }
}

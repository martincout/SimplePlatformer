
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

class MixLevels : MonoBehaviour
{
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private Slider slider; 
    private float musicLvl;

    private void Start()
    {
        float playerPrefMusicLvl = PlayerPrefs.GetFloat("SliderVolumeLevel");
        masterMixer.SetFloat("MasterVol", playerPrefMusicLvl);
        slider.value = playerPrefMusicLvl;
        float vol = 0f;
        masterMixer.GetFloat("MasterVol", out vol);
        this.musicLvl = vol;
    }

    public void SetMusicLvl(float musicLvl)
    {
        masterMixer.SetFloat("MasterVol", musicLvl);
        this.musicLvl = musicLvl;
    }

    public void SaveSliderVolume()
    {
        PlayerPrefs.SetFloat("SliderVolumeLevel", musicLvl);
    }

}


using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    public MainMenu Mainmenu;

    public AudioMixer mymixer;
    public Slider MusicSlider;
    public Slider SFXSlider;

    private void Start()
    {
        gameObject.SetActive(false);

        if (PlayerPrefs.HasKey("MusicVolume") && PlayerPrefs.HasKey("SFXVolume"))
        {
            loadsettings();
        }
        else
        {
            SetSFXSound();
            SetMusicSound();
        }
    }

    public void SetSFXSound()
    {

        float sfxvoloume = SFXSlider.value;
        mymixer.SetFloat("SFX", sfxvoloume);
        PlayerPrefs.SetFloat("SFXVolume", sfxvoloume);
    }

    public void SetMusicSound()
    {
        float musicvoloume = MusicSlider.value;
        mymixer.SetFloat("Music", musicvoloume);
        PlayerPrefs.SetFloat("MusicVolume", musicvoloume);
    }

    private void loadsettings()
    {
        SFXSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        MusicSlider.value = PlayerPrefs.GetFloat("SFXVolume");

        SetSFXSound();
        SetMusicSound();
    }


    public void back()
    {
        gameObject.SetActive(false);
        Mainmenu.gameObject.SetActive(true);
    }
}

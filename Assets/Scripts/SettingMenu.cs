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
        loadsettings();
        gameObject.SetActive(false);
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
        if (PlayerPrefs.HasKey("MusicVolume") && PlayerPrefs.HasKey("SFXVolume"))
        {
            SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
            MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume"); 
        }

        SetSFXSound();
        SetMusicSound();
    }

    public void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll(); // Borra todos los datos guardados en PlayerPrefs
        PlayerPrefs.Save(); // Asegúrate de guardar los cambios
        Debug.Log("Todos los datos de PlayerPrefs han sido borrados.");
    }

    public void back()
    {
        gameObject.SetActive(false);
        Mainmenu.gameObject.SetActive(true);
    }
}

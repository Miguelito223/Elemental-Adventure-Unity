using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{

    public SettingMenu settingmenu;
    private void Start()
    {
        gameObject.SetActive(true);
    }
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void SettingMenu()
    {
        gameObject.SetActive(false);
        settingmenu.gameObject.SetActive(true);
    }
}

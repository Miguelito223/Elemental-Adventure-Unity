using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu : MonoBehaviourPunCallbacks
{
    public Button button;
    public Button button2;
    public SettingMenu settingmenu;
    private void Start()
    {
        gameObject.SetActive(true);
    }
    public void StartGame()
    {
        PhotonNetwork.OfflineMode = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void StartMultiplayerGame()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public void SettingMenu()
    {
        gameObject.SetActive(false);
        settingmenu.gameObject.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}

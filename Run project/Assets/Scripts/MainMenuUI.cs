using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject stageSelectPanel;

    public void OnStartClicked()
    {
        mainMenuPanel.SetActive(false);
        stageSelectPanel.SetActive(true);
    }

    public void OnStage1Clicked()
    {
        SceneManager.LoadScene("GamePlayScene");
    }

    public void OnBackClicked()
    {
        stageSelectPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}

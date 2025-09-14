using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject stageSelectPanel;
    public GameObject characterSelectPanel;

    public void OnStartClicked()
    {
        mainMenuPanel.SetActive(false);
        characterSelectPanel.SetActive(true);
    }
    public void OnCharacterClicked()
    {
        characterSelectPanel.SetActive(false);
        stageSelectPanel.SetActive(true);
    }

    public void OnStage1Clicked()
    {
        SceneManager.LoadScene("GamePlayScene");
    }

    public void OnBackClicked()
    {
        stageSelectPanel.SetActive(false);
        characterSelectPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}

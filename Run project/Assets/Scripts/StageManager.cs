using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public List<GameObject> stageGrids; // Grid_1_1 ~ Grid_1_12
    public int currentStageIndex = 0;

    public GameObject fadeImage; // UI Image (������), Canvas�� �־�� ��
    private CanvasGroup fadeCanvasGroup;

    void Start()
    {
        fadeCanvasGroup = fadeImage.GetComponent<CanvasGroup>();

        for (int i = 0; i < stageGrids.Count; i++)
            stageGrids[i].SetActive(i == currentStageIndex); // ù ��°�� Ȱ��ȭ
    }

    public void GoToNextStage()
    {
        StartCoroutine(TransitionToNextStage());
    }

    private IEnumerator TransitionToNextStage()
    {
        // ���̵� �ƿ�
        yield return Fade(1f);

        // ���� �������� ���� ���� �������� �ѱ�
        stageGrids[currentStageIndex].SetActive(false);
        currentStageIndex++;

        if (currentStageIndex < stageGrids.Count)
        {
            stageGrids[currentStageIndex].SetActive(true);
            ResetPlayerPosition();
        }
        else
        {
            Debug.Log("All stages cleared!");
            // ���� ������ �̵��ϰų� Ŭ���� ó��
        }

        // ���̵� ��
        yield return Fade(0f);
    }

    IEnumerator Fade(float targetAlpha)
    {
        float duration = 0.5f;
        float start = fadeCanvasGroup.alpha;
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(start, targetAlpha, t / duration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }

    void ResetPlayerPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = new Vector3(0, 0, 0); // ���� ��ġ. �ʿ� �� ����
    }
}

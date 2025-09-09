using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public List<GameObject> stageGrids; // Grid_1_1 ~ Grid_1_12
    public int currentStageIndex = 0;

    public GameObject fadeImage; // UI Image (검은색), Canvas에 있어야 함
    private CanvasGroup fadeCanvasGroup;

    void Start()
    {
        fadeCanvasGroup = fadeImage.GetComponent<CanvasGroup>();

        for (int i = 0; i < stageGrids.Count; i++)
            stageGrids[i].SetActive(i == currentStageIndex); // 첫 번째만 활성화
    }

    public void GoToNextStage()
    {
        StartCoroutine(TransitionToNextStage());
    }

    private IEnumerator TransitionToNextStage()
    {
        // 페이드 아웃
        yield return Fade(1f);

        // 현재 스테이지 끄고 다음 스테이지 켜기
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
            // 다음 씬으로 이동하거나 클리어 처리
        }

        // 페이드 인
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
        player.transform.position = new Vector3(0, 0, 0); // 예시 위치. 필요 시 수정
    }
}

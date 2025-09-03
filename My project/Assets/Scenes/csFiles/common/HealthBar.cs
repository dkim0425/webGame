using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    [Header("UI")]
    public Image fillImage;
    public GameObject fillGO;
    public GameObject backgroundGO;

    public Sprite greenFillSprite;
    public Sprite orangeFillSprite;
    public Sprite redFillSprite;

    [Header("설정")]
    public float verticalOffset = 0.1f;
    public bool useFillAmount = false;

    private Transform target;
    private BaseUnit unit;
    private Castle castle;
    private Collider2D targetCollider;

    private Coroutine hideCoroutine;

    public void Init(Transform t)
    {
        target = t;
        unit = t.GetComponent<BaseUnit>();
        castle = t.GetComponent<Castle>();
        targetCollider = t.GetComponent<Collider2D>();

        SetVisible(false);
        UpdatePosition();
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        int currentHP = 1;
        int maxHP = 1;

        if (unit != null)
        {
            currentHP = unit.CurrentHP;
            maxHP = unit.MaxHP;
        }
        else if (castle != null)
        {
            currentHP = castle.CurrentHP;
            maxHP = castle.MaxHP;
        }

        float hpRatio = Mathf.Clamp01(currentHP / (float)maxHP);

        // 체력 비율에 따라 스프라이트 교체
        if (hpRatio > 0.5f)
            fillImage.sprite = greenFillSprite;
        else if (hpRatio > 0.2f)
            fillImage.sprite = orangeFillSprite;
        else
            fillImage.sprite = redFillSprite;

        // 체력 채움
        if (useFillAmount)
            fillImage.fillAmount = hpRatio;
        else
            fillImage.rectTransform.localScale = new Vector3(hpRatio, 1f, 1f);

        // 체력 감소 시 바로 표시
        if (hpRatio < 1f)
        {
            SetVisible(true);
            if (hideCoroutine != null)
            {
                StopCoroutine(hideCoroutine);
                hideCoroutine = null;
            }
        }

        // 체력 100% → 3초 후 숨김
        if (hpRatio >= 1f && hideCoroutine == null)
        {
            hideCoroutine = StartCoroutine(HideAfterDelay(3f));
        }

        UpdatePosition();
    }

    void SetVisible(bool visible)
    {
        backgroundGO.SetActive(visible);
        fillGO.SetActive(visible);
    }

    IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetVisible(false);
        hideCoroutine = null;
    }

    void UpdatePosition()
    {
        Vector3 position = target.position;

        if (targetCollider != null)
            position.y = targetCollider.bounds.max.y + verticalOffset;
        else
            position += new Vector3(0, 1f, 0);

        transform.position = position;
    }
}

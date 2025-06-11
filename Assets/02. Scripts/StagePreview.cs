using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StagePreview : MonoBehaviour
{
    [SerializeField] private TextMeshPro stageNameText;
    [SerializeField] private TextMeshPro stageDescriptionText;
    [SerializeField] private GameObject stageModel; // 스테이지의 3D 모델
    [SerializeField] private GameObject highlightEffect; // 선택 시 하이라이트 효과
    [SerializeField] private float hoverHeight = 0.1f; // 호버 시 올라가는 높이
    [SerializeField] private float hoverSpeed = 2f; // 호버 애니메이션 속도

    private Vector3 originalPosition;
    private bool isHovered = false;
    private StageSelectionManager.StageInfo stageInfo;

    private void Start()
    {
        originalPosition = transform.position;
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(false);
        }
    }

    public void Initialize(StageSelectionManager.StageInfo info)
    {
        stageInfo = info;
        if (stageNameText != null)
        {
            stageNameText.text = info.stageName;
        }
        if (stageDescriptionText != null)
        {
            stageDescriptionText.text = info.stageDescription;
        }
    }

    private void Update()
    {
        // 호버 애니메이션
        if (isHovered)
        {
            float newY = originalPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    public void OnHoverEnter()
    {
        isHovered = true;
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(true);
        }
    }

    public void OnHoverExit()
    {
        isHovered = false;
        transform.position = originalPosition;
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(false);
        }
    }

    public void OnSelect()
    {
        // 선택 시 효과
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(true);
        }
    }

    private void OnMouseDown()
    {
        OnSelect();
        var manager = FindObjectOfType<StageSelectionManager>();
        if (manager != null && stageInfo != null)
        {
            manager.SendMessage("OnStageSelected", stageInfo);
        }
    }
} 
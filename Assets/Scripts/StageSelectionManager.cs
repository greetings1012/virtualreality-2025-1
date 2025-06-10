using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectionManager : MonoBehaviour
{
    [System.Serializable]
    public class StageInfo
    {
        public string stageName;
        public string stageDescription;
        public Sprite stagePreview;
        public string sceneToLoad;
        public Vector3 stagePosition; // 스테이지 미니어처의 위치
    }

    [SerializeField] private StageInfo[] stages;
    [SerializeField] private GameObject stagePreviewPrefab; // 스테이지 미니어처 프리팹
    [SerializeField] private float stageSpacing = 2f; // 스테이지 간 간격
    [SerializeField] private float stageHeight = 1.2f; // 스테이지 미니어처의 높이 (플레이어 눈높이)
    [SerializeField] private float stageScale = 0.5f; // 스테이지 미니어처의 크기

    private void Start()
    {
        CreateStagePreviews();
    }

    private void CreateStagePreviews()
    {
        for (int i = 0; i < stages.Length; i++)
        {
            // 스테이지 미니어처 생성
            GameObject stagePreview = Instantiate(stagePreviewPrefab, transform);
            stagePreview.transform.localPosition = new Vector3(i * stageSpacing, stageHeight, 0);
            stagePreview.transform.localScale = Vector3.one * stageScale;

            // 스테이지 정보 설정
            StagePreview stagePreviewComponent = stagePreview.GetComponent<StagePreview>();
            if (stagePreviewComponent != null)
            {
                stagePreviewComponent.Initialize(stages[i]);
            }

            // XR Interactable 설정
            UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = stagePreview.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (grabInteractable != null)
            {
                grabInteractable.selectEntered.AddListener((args) => OnStageSelected(stages[i]));
            }
        }
    }

    private void OnStageSelected(StageInfo stage)
    {
        Debug.Log($"Selected stage: {stage.stageName}");
        SceneManager.LoadScene(stage.sceneToLoad);
    }
} 
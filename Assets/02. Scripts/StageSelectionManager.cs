using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static XRInputSystem;

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

    private List<GameObject> stagePrefabs = new List<GameObject>();

    class CustomRaycastResult
    {
        public GameObject go;
        public GameObject parent;
        public Vector3 point;
    }

    [SerializeField]
    private InputActionReference aButtonAction;
    [SerializeField]
    private InputActionReference xButtonAction;
    [SerializeField]
    private GameObject[] interactors;

    private bool isClicked = false;

    bool CustomRaycastAABB(GameObject interactors, out CustomRaycastResult result, string mask)
    {
        result = new CustomRaycastResult();

        Ray ray = new Ray();
        ray.direction = interactors.transform.forward;
        ray.origin = interactors.transform.position;
        Debug.DrawRay(ray.origin, ray.direction, Color.red, 1.0F);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100000.0F, LayerMask.GetMask(mask)))
        {
            result.parent = hit.transform.parent.gameObject;
            result.go = hit.transform.gameObject;
            result.point = hit.point;

            return true;
        }

        return false;
    }

    private void Start()
    {
        CreateStagePreviews();
    }

    void OnEnable()
    {
        aButtonAction.action.Enable();
        xButtonAction.action.Enable();
        aButtonAction.action.started += OnButtonStarted;
        xButtonAction.action.started += OnButtonStarted;
    }

    private void OnDisable()
    {
        aButtonAction.action.started -= OnButtonStarted;
        xButtonAction.action.started -= OnButtonStarted;
    }

    IEnumerator LoadScene(string name)
    {
        yield return new WaitForSeconds(1.0F);
        SceneManager.LoadScene(name);
        yield return null;
    }

    private void Update()
    {
        for (int i = 0; i < stagePrefabs.Count; ++i)
        {
            Transform stageModel = stagePrefabs[i].transform.Find("StageModel");

            if (stageModel != null)
            {
                Renderer renderer = stageModel.GetComponent<Renderer>();
                if (renderer != null)
                {
                    if (renderer.material.HasProperty("_BaseColor"))
                    {
                        renderer.material.SetColor("_BaseColor", Color.grey);
                    }
                    else if (renderer.material.HasProperty("_Color"))
                    {
                        renderer.material.color = Color.grey;
                    }
                }
            }
        }

        for (int i = 0; i < interactors.Length; ++i)
        {
            if (CustomRaycastAABB(interactors[i], out CustomRaycastResult result, "Block"))
            {
                if (result.go != null)
                {
                    Transform stageModel = result.go.transform.Find("StageModel");

                    if (stageModel != null)
                    {
                        Renderer renderer = stageModel.GetComponent<Renderer>();
                        if (renderer != null)
                        {
                            if (renderer.material.HasProperty("_BaseColor"))
                            {
                                renderer.material.SetColor("_BaseColor", Color.yellow);
                            }
                            else if (renderer.material.HasProperty("_Color"))
                            {
                                renderer.material.color = Color.yellow;
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnButtonStarted(InputAction.CallbackContext ctx)
    {
        if (isClicked)
            return;

        for (int i = 0; i < interactors.Length; ++i)
        {
            if (CustomRaycastAABB(interactors[i], out CustomRaycastResult result, "Block"))
            {
                StartCoroutine(LoadScene("Stage_" + result.go.name));
                isClicked = true;
            }
        }
    }

    private void CreateStagePreviews()
    {
        for (int i = 0; i < stages.Length; i++)
        {
            // 스테이지 미니어처 생성
            GameObject stagePreview = Instantiate(stagePreviewPrefab, transform);
            stagePrefabs.Add(stagePreview);
            stagePreview.name = (i + 1).ToString();
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
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class XRInputSystem : MonoBehaviour
{
    [SerializeField]
    private InputActionReference aButtonAction;
    [SerializeField]
    private InputActionReference xButtonAction;
    [SerializeField]
    private InputActionReference resetButtonAction;
    [SerializeField]
    private InputActionReference executeButtonAction;
    [SerializeField]
    private InputActionReference LsInputAction;
    [SerializeField]
    private InputActionReference RsInputAction;
    [SerializeField]
    private GameObject[] interactors;

    private CodeblockGenerator gen;
    public Vector2 ScrollRatio;

    [SerializeField]
    private GameObject blockPanel;
    [SerializeField]
    private GameObject outputPanel; // orderCodeblock

    private bool isSelecting = false;
    private GameObject selectedObject = null;
    private int interactedIdx = -1;

    [SerializeField]
    GameObject blockMessage;
    [SerializeField]
    GameObject forExpression;
    [SerializeField]
    GameObject forExpressionContent;
    [SerializeField]
    GameObject mvMessage;

    [SerializeField]
    GameObject switchCameraComponent;
    private CameraSwitcher switcher;
    [SerializeField]
    GameObject executeSequenceComponent;
    BlockSystem bsystem;

    private List<GameObject> forExpTempObject = new List<GameObject>();

    class CustomRaycastResult
    {
        public GameObject go;
        public GameObject parent;
        public Vector3 point;
    }

    public enum ForExpInputMode
    {
        None,
        Number,
        Command,
    }

    private ForExpInputMode forExpInputMode = ForExpInputMode.None;

    bool CustomRaycastAABB(GameObject interactors, out CustomRaycastResult result, string mask)
    {
        result = new CustomRaycastResult();

        Ray ray = new Ray();
        ray.direction = interactors.transform.forward;
        ray.origin = interactors.transform.position;
        Debug.DrawRay(ray.origin, ray.direction, Color.red, 1.0F);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10000.0F, LayerMask.GetMask(mask)))
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
        resetButtonAction.action.Enable();
        executeButtonAction.action.Enable();
        aButtonAction.action.Enable();
        xButtonAction.action.Enable();
        LsInputAction.action.Enable();
        RsInputAction.action.Enable();

        gen = GameObject.Find("BlockSystem").GetComponent<CodeblockGenerator>();
        switcher = switchCameraComponent.GetComponent<CameraSwitcher>();
        bsystem = executeSequenceComponent.GetComponent<BlockSystem>();
    }

    private void Update()
    {
        if (isSelecting)
        {
            selectedObject.transform.rotation = interactors[interactedIdx].transform.rotation;
            selectedObject.transform.position = interactors[interactedIdx].transform.position + interactors[interactedIdx].transform.forward * 0.1F;
        }

        switch (forExpInputMode)
        {
            case ForExpInputMode.None:
                blockPanel.SetActive(true);
                mvMessage.SetActive(false);
                blockMessage.SetActive(false);
                forExpression.SetActive(false);
                break;
            case ForExpInputMode.Number:
                blockPanel.SetActive(false);
                mvMessage.SetActive(false);
                blockMessage.SetActive(true);
                forExpression.SetActive(true);
                break;
            case ForExpInputMode.Command:
                blockPanel.SetActive(false);
                mvMessage.SetActive(true);
                blockMessage.SetActive(false);
                forExpression.SetActive(true);
                break;
        }
    }

    void OnEnable()
    {
        resetButtonAction.action.started += OnButtonResetStarted;
        executeButtonAction.action.started += OnButtonExecuteStarted;
        aButtonAction.action.started += OnButtonStarted;
        xButtonAction.action.started += OnButtonStarted;
        aButtonAction.action.canceled += OnButtonReleased;
        xButtonAction.action.canceled += OnButtonReleased;
        LsInputAction.action.performed += OnScrollPerformed;
        RsInputAction.action.performed += OnScrollPerformed;
    }

    private void OnButtonExecuteStarted(InputAction.CallbackContext context)
    {
        switcher.SwitchToRobotCamera();
        bsystem.ExecuteSequence();
    }

    private void OnButtonResetStarted(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDisable()
    {
        resetButtonAction.action.started -= OnButtonResetStarted;
        executeButtonAction.action.started -= OnButtonExecuteStarted;
        aButtonAction.action.started -= OnButtonStarted;
        xButtonAction.action.started -= OnButtonStarted;
        aButtonAction.action.canceled -= OnButtonReleased;
        xButtonAction.action.canceled -= OnButtonReleased;
        LsInputAction.action.performed -= OnScrollPerformed;
        RsInputAction.action.performed -= OnScrollPerformed;
    }

    private void OnScrollPerformed(InputAction.CallbackContext ctx)
    {
        Vector2 ls = ctx.ReadValue<Vector2>();
        Vector2 rs = ctx.ReadValue<Vector2>();

        ScrollRatio.x = Mathf.Abs(rs.x) > Mathf.Abs(ls.x) ? -rs.x : -ls.x;
        ScrollRatio.y = Mathf.Abs(rs.y) > Mathf.Abs(ls.y) ? -rs.y : -ls.y;
    }

    private void OnButtonStarted(InputAction.CallbackContext ctx)
    {
        for (int i = 0; i < interactors.Length; ++i)
        {
            if (CustomRaycastAABB(interactors[i], out CustomRaycastResult result, "Block"))
            {
                foreach (var t in gen.listBlockObject)
                {
                    if (t.block.actionName == result.go.name)
                    {
                        interactedIdx = i;
                        isSelecting = true;
                        selectedObject = Instantiate((GameObject)Resources.Load("Prefabs/" + result.parent.name), blockPanel.transform);
                        selectedObject.layer = LayerMask.GetMask("Default"); // Raycast에 안걸리도록 바꾸기
                    }
                }

                return;
            }
            else if (CustomRaycastAABB(interactors[i], out CustomRaycastResult result1, "Block1"))
            {
                foreach (var t in gen.listBlockObject)
                {
                    if (t.block.actionName == result1.parent.name)
                    {
                        gen.AddOrderBlock((GameObject)Resources.Load("Prefabs/" + result1.parent.name), true);
                        forExpTempObject.Add(Instantiate((GameObject)Resources.Load("Prefabs/" + result1.parent.name), forExpressionContent.transform));
                        forExpInputMode = ForExpInputMode.Command;
                    }
                }

                return;
            }
            else if (CustomRaycastAABB(interactors[i], out CustomRaycastResult result2, "Block2"))
            {
                foreach (var t in gen.listBlockObject)
                {
                    if (t.block.actionName == result2.parent.name)
                    {
                        gen.AddOrderBlock((GameObject)Resources.Load("Prefabs/" + result2.parent.name), true);
                        forExpTempObject.Add(Instantiate((GameObject)Resources.Load("Prefabs/" + result2.parent.name), forExpressionContent.transform));
                        forExpInputMode = ForExpInputMode.None;
                    }
                }

                return;
            }
        }

        interactedIdx = -1;
    }

    private void OnButtonReleased(InputAction.CallbackContext ctx)
    {
        if (interactedIdx == -1)
        {
            return;
        }

        if (CustomRaycastAABB(interactors[interactedIdx], out CustomRaycastResult result, "Panel") && forExpInputMode == ForExpInputMode.None)
        {
            if (outputPanel.name == result.go.name)
            {
                //selectedObject.layer = LayerMask.NameToLayer("Block"); // Layer를 다시 Block으로
                gen.AddOrderBlock(selectedObject);

                // 만약 for문이라면
                if (selectedObject.name == "for(Clone)")
                {
                    forExpInputMode = ForExpInputMode.Number;

                    for (int i = 0; i < forExpTempObject.Count; i++)
                    {
                        Destroy(forExpTempObject[i]);
                    }
                    forExpTempObject.Clear();
                }
            }
        }

        Destroy(selectedObject);
        isSelecting = false;
        interactedIdx = -1;
    }
}

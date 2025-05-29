using Oculus.Interaction;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using static OVRPlugin;

public class XRInputSystem : MonoBehaviour
{
    [SerializeField]
    private InputActionReference aButtonAction;
    [SerializeField]
    private InputActionReference xButtonAction;
    [SerializeField]
    private NearFarInteractor[] interactors;

    private CodeblockGenerator gen;

    [SerializeField]
    private GameObject blockPanel;
    [SerializeField]
    private GameObject outputPanel; // orderCodeblock

    private bool isSelecting = false;
    private GameObject selectedObject = null;

    private void Start()
    {
        aButtonAction.action.Enable();
        xButtonAction.action.Enable();
        gen = GameObject.Find("BlockSystem").GetComponent<CodeblockGenerator>();
    }

    private void Update()
    {
        if(isSelecting)
        {
            for (int i = 0; i < interactors.Length; ++i)
            {
                if (interactors[i].TryGetCurrentUIRaycastResult(out UnityEngine.EventSystems.RaycastResult result))
                {
                    selectedObject.transform.position = result.worldPosition;
                }
            }

            //Matrix4x4 rotationMatrix = Matrix4x4.Rotate(cam.transform.rotation);
        }
    }

    void OnEnable()
    {
        aButtonAction.action.started += OnButtonStarted;
        xButtonAction.action.started += OnButtonStarted;
        aButtonAction.action.canceled += OnButtonReleased;
        xButtonAction.action.canceled += OnButtonReleased;
    }

    private void OnDisable()
    {
        aButtonAction.action.started -= OnButtonStarted;
        xButtonAction.action.started -= OnButtonStarted;
        aButtonAction.action.canceled -= OnButtonReleased;
        xButtonAction.action.canceled -= OnButtonReleased;
    }

    private void OnButtonStarted(InputAction.CallbackContext ctx)
    {
        for (int i = 0; i < interactors.Length; ++i)
        {
            if (interactors[i].TryGetCurrentUIRaycastResult(out UnityEngine.EventSystems.RaycastResult result))
            {
                foreach (var t in gen.listBlockObject)
                {
                    if (t.block.actionName == result.gameObject.name)
                    {
                        isSelecting = true;
                        selectedObject = Instantiate(result.gameObject, blockPanel.transform);
                        selectedObject.layer = LayerMask.GetMask("Default"); // Raycast에 안걸리도록 바꾸기
                    }
                }
            }
        }
    }

    private void OnButtonReleased(InputAction.CallbackContext ctx)
    {
        for (int i = 0; i < interactors.Length; ++i)
        {
            if (interactors[i].TryGetCurrentUIRaycastResult(out UnityEngine.EventSystems.RaycastResult result))
            {
                if (outputPanel.name == result.gameObject.transform.parent.name)
                {
                    //selectedObject.layer = LayerMask.NameToLayer("UI"); // Layer를 다시 UI로
                    gen.AddOrderBlock(selectedObject);
                }
            }
        }

        Destroy(selectedObject);
        isSelecting = false;
    }
}

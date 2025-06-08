using Mono.Cecil;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class XRInputSystem : MonoBehaviour
{
    [SerializeField]
    private InputActionReference aButtonAction;
    [SerializeField]
    private InputActionReference xButtonAction;
    [SerializeField]
    private GameObject[] interactors;

    private CodeblockGenerator gen;

    [SerializeField]
    private GameObject blockPanel;
    [SerializeField]
    private GameObject outputPanel; // orderCodeblock

    private bool isSelecting = false;
    private GameObject selectedObject = null;
    private int interactedIdx = -1;

    class CustomRaycastResult
    {
        public GameObject go;
        public GameObject parent;
    }

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
            return true;
        }

        return false;
    }

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
            selectedObject.transform.position = interactors[interactedIdx].transform.position;
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
        }

        interactedIdx = -1;
    }

    private void OnButtonReleased(InputAction.CallbackContext ctx)
    {
        if (interactedIdx == -1)
            return;

        if (CustomRaycastAABB(interactors[interactedIdx], out CustomRaycastResult result, "Panel"))
        {
            if (outputPanel.name == result.go.name)
            {
                //selectedObject.layer = LayerMask.NameToLayer("Block"); // Layer를 다시 Block으로
                gen.AddOrderBlock(selectedObject);
            }
        }

        Destroy(selectedObject);
        isSelecting = false;
        interactedIdx = -1;
    }
}

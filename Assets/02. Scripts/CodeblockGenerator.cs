using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static BlockSystem;

public class CodeblockGenerator : MonoBehaviour
{
    public class CodeObject
    {
        public CodeBlock block;
        public GameObject go;
        public RectTransform rt;
    }

    private List<CodeObject> listBlockObject = new List<CodeObject>();
    private List<GameObject> orderBlockObject = new List<GameObject>();

    [SerializeField]
    private GameObject blockPanel;
    [SerializeField]
    private GameObject blockListPanel;
    [SerializeField]
    private GameObject blockOrderPanel;
    [SerializeField]
    private GameObject blockListContent;
    [SerializeField]
    private GameObject blockOrderContent;
    [SerializeField]
    private GameObject blockPrefab;

    private bool isSelecting = false;
    private RectTransform blockListRt = null;
    private RectTransform blockOrderRt = null;
    private GameObject selectedObject = null;
    private RectTransform selectedObjectRt = null;
    private ScrollRect listPanelSR = null;
    private BlockSystem blockSystem = null;
    int selectedIndex = -1;

    void Start()
    {
        blockSystem = GetComponent<BlockSystem>();
        blockListRt = blockListPanel.GetComponent<RectTransform>();
        blockOrderRt = blockOrderPanel.GetComponent<RectTransform>();
        listPanelSR = blockListPanel.GetComponent<ScrollRect>();

        AddBlock("MoveForward", "MoveForward", 1);
        AddBlock("RotateLeft", "RotateLeft", 1);
        AddBlock("RotateRight", "RotateRight", 1);
        AddBlock("for", "for", 1);
        AddBlock("n1", "n1", 1);
        AddBlock("n2", "n2", 1);
        AddBlock("n3", "n3", 1);
        AddBlock("n4", "n4", 1);
        AddBlock("n5", "n5", 1);
        AddBlock("n6", "n6", 1);
    }

    void Update()
    {
        Vector2 mp = Input.mousePosition;

        if (Input.GetMouseButtonDown(0) && !isSelecting)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(blockListRt, mp))
            {
                for (int i = 0; i < listBlockObject.Count; ++i)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(listBlockObject[i].rt, mp))
                    {
                        isSelecting = true;

                        selectedObject = Instantiate(listBlockObject[i].go, blockPanel.transform);
                        selectedObjectRt = selectedObject.GetComponent<RectTransform>();

                        selectedIndex = i;

                        listPanelSR.enabled = false;

                        break;
                    }
                }
            }
        }
        else if (Input.GetMouseButtonUp(0) && isSelecting)
        {
            selectedObject.SetActive(false);
            Destroy(selectedObject, 3.0F);

            isSelecting = false;
            listPanelSR.enabled = true;

            if (RectTransformUtility.RectangleContainsScreenPoint(blockOrderRt, mp))
            {
                orderBlockObject.Add(Instantiate(listBlockObject[selectedIndex].go, blockOrderContent.transform));
                blockSystem.AddBlock(listBlockObject[selectedIndex].block);
            }
        }

        if (isSelecting)
        {
            selectedObjectRt.transform.position = mp;
        }
    }

    public void AddBlock(string blockName, string actionName, int repeatCount)
    {
        CodeObject co = new CodeObject();

        CodeBlock cb = new CodeBlock();
        cb.blockName = blockName;
        cb.actionName = actionName;
        cb.repeatCount = repeatCount;

        co.block = cb;
        co.go = Instantiate(blockPrefab, blockListContent.transform);

        Texture tex = (Texture)Resources.Load(actionName);
        if (tex != null)
        {
            co.go.GetComponent<RawImage>().texture = tex;
        }

        co.rt = co.go.GetComponent<RectTransform>();

        listBlockObject.Add(co);
    }
}

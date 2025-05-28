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
            else if (RectTransformUtility.RectangleContainsScreenPoint(blockOrderRt, mp))
            {
                for (int i = 0; i < orderBlockObject.Count; ++i)
                {
                    RectTransform rt = orderBlockObject[i].GetComponent<RectTransform>();
                    if (RectTransformUtility.RectangleContainsScreenPoint(rt, mp))
                    {
                      
                        isSelecting = true;
                        selectedObject = orderBlockObject[i];
                        selectedObjectRt = selectedObject.GetComponent<RectTransform>();
                        selectedIndex = i;
                      
                        break;
                    }
                }
            }
        }
        else if (Input.GetMouseButtonUp(0) && isSelecting)
        {
            isSelecting = false;
            listPanelSR.enabled = true;

           
            if (selectedObject != null && !orderBlockObject.Contains(selectedObject))
            {
                selectedObject.SetActive(false);
                Destroy(selectedObject, 3.0f);

                if (RectTransformUtility.RectangleContainsScreenPoint(blockOrderRt, mp))
                {
                    GameObject newObj = Instantiate(listBlockObject[selectedIndex].go, blockOrderContent.transform);
                    orderBlockObject.Add(newObj);
                    blockSystem.AddBlock(listBlockObject[selectedIndex].block);
                }
            }
         
            else if (orderBlockObject.Contains(selectedObject))
            {
        
                if (!RectTransformUtility.RectangleContainsScreenPoint(blockOrderRt, mp))
                {
                    orderBlockObject.Remove(selectedObject);
                    blockSystem.RemoveBlock(selectedIndex);
                    Destroy(selectedObject);
                    selectedObject = null;
                    selectedIndex = -1;
                    return;
                }
            }

            selectedObject = null;
            selectedIndex = -1;
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

    public void ResetCodeBlocks()
    {
        foreach (Transform contentChild in blockOrderContent.transform)
        {
            Destroy(contentChild.gameObject);
        }
        orderBlockObject.Clear();

        if (blockSystem != null)
        {
            blockSystem.ClearBlocks();
        }

        if (selectedObject != null)
        {
            Destroy(selectedObject);
        }

        isSelecting = false;
        selectedObject = null;
        selectedObjectRt = null;
        selectedIndex = -1;
        listPanelSR.enabled = true;
    }
}

//using NUnit.Framework;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.UI;
//using static BlockSystem;

//public class CodeblockGenerator : MonoBehaviour
//{
//    public class CodeObject
//    {
//        public CodeBlock block;
//        public GameObject go;
//        public RectTransform rt;
//    }

//    private List<CodeObject> listBlockObject = new List<CodeObject>();
//    private List<GameObject> orderBlockObject = new List<GameObject>();

//    [SerializeField]
//    private GameObject blockPanel;
//    [SerializeField]
//    private GameObject blockListPanel;
//    [SerializeField]
//    private GameObject blockOrderPanel;
//    [SerializeField]
//    private GameObject blockListContent;
//    [SerializeField]
//    private GameObject blockOrderContent;
//    [SerializeField]
//    private GameObject blockPrefab;

//    private bool isSelecting = false;
//    private RectTransform blockListRt = null;
//    private RectTransform blockOrderRt = null;
//    private GameObject selectedObject = null;
//    private RectTransform selectedObjectRt = null;
//    private ScrollRect listPanelSR = null;
//    private BlockSystem blockSystem = null;
//    int selectedIndex = -1;

//    void Start()
//    {
//        blockSystem = GetComponent<BlockSystem>();
//        blockListRt = blockListPanel.GetComponent<RectTransform>();
//        blockOrderRt = blockOrderPanel.GetComponent<RectTransform>();
//        listPanelSR = blockListPanel.GetComponent<ScrollRect>();

//        AddBlock("MoveForward", "MoveForward", 1);
//        AddBlock("RotateLeft", "RotateLeft", 1);
//        AddBlock("RotateRight", "RotateRight", 1);
//        AddBlock("for", "for", 1);
//        AddBlock("n1", "n1", 1);
//        AddBlock("n2", "n2", 1);
//        AddBlock("n3", "n3", 1);
//        AddBlock("n4", "n4", 1);
//        AddBlock("n5", "n5", 1);
//        AddBlock("n6", "n6", 1);
//    }

//    void Update()
//    {
//        Vector2 mp = Input.mousePosition;

//        if (Input.GetMouseButtonDown(0))
//        {
//            HandleMouseDown(mp);
//        }
//        else if (Input.GetMouseButtonUp(0))
//        {
//            HandleMouseUp(mp);
//        }

//        if (isSelecting && selectedObjectRt != null)
//        {

//            if (selectedObject == null || selectedObjectRt == null || !selectedObject)
//            {
//                ResetSelectionState();
//                return;
//            }

//            selectedObjectRt.position = mp;
//        }
//    }

//    private void HandleMouseDown(Vector2 mp)
//    {
//        if (isSelecting) return;

//        if (RectTransformUtility.RectangleContainsScreenPoint(blockListRt, mp))
//        {
//            for (int i = 0; i < listBlockObject.Count; ++i)
//            {
//                if (RectTransformUtility.RectangleContainsScreenPoint(listBlockObject[i].rt, mp))
//                {
//                    isSelecting = true;
//                    selectedObject = Instantiate(listBlockObject[i].go, blockPanel.transform);
//                    selectedObjectRt = selectedObject.GetComponent<RectTransform>();
//                    selectedIndex = i;
//                    listPanelSR.enabled = false;
//                    return;
//                }
//            }
//        }

//        if (RectTransformUtility.RectangleContainsScreenPoint(blockOrderRt, mp))
//        {
//            for (int i = 0; i < orderBlockObject.Count; ++i)
//            {
//                RectTransform rt = orderBlockObject[i].GetComponent<RectTransform>();
//                if (RectTransformUtility.RectangleContainsScreenPoint(rt, mp))
//                {
//                    isSelecting = true;
//                    selectedObject = orderBlockObject[i];
//                    selectedObjectRt = rt;
//                    selectedIndex = i;
//                    return;
//                }
//            }
//        }
//    }

//    private void HandleMouseUp(Vector2 mp)
//    {
//        if (!isSelecting) return;

//        if (selectedObject == null || selectedObjectRt == null)
//        {
//            ResetSelectionState();
//            return;
//        }

//        if (RectTransformUtility.RectangleContainsScreenPoint(blockOrderRt, mp))
//        {
//            // 리스트에서 가져온 블록이라면 새로 추가
//            if (selectedObject.transform.parent == blockPanel.transform)
//            {
//                orderBlockObject.Add(Instantiate(listBlockObject[selectedIndex].go, blockOrderContent.transform));
//                blockSystem.AddBlock(listBlockObject[selectedIndex].block);
//                Destroy(selectedObject);
//                selectedObject = null;
//                selectedObjectRt = null;
//                return;
//            }
//        }
//        else
//        {
//            // 순서 패널에서 꺼낸 블록이라면 삭제
//            if (selectedObject.transform.parent == blockOrderContent.transform)
//            {
//                Destroy(orderBlockObject[selectedIndex]);
//                orderBlockObject.RemoveAt(selectedIndex);
//                blockSystem.RemoveBlock(selectedIndex);
//                selectedObject = null;
//                selectedObjectRt = null;
//                return;
//            }
//            else
//            {
//                Destroy(selectedObject);
//                selectedObject = null;
//                selectedObjectRt = null;
//                return;
//            }
//        }

//        ResetSelectionState();
//    }

//    private void ResetSelectionState()
//    {
//        isSelecting = false;
//        selectedObject = null;
//        selectedObjectRt = null;
//        selectedIndex = -1;
//        listPanelSR.enabled = true;
//    }

//    public void AddBlock(string blockName, string actionName, int repeatCount)
//    {
//        CodeObject co = new CodeObject();

//        CodeBlock cb = new CodeBlock();
//        cb.blockName = blockName;
//        cb.actionName = actionName;
//        cb.repeatCount = repeatCount;

//        co.block = cb;
//        co.go = Instantiate(blockPrefab, blockListContent.transform);

//        Texture tex = (Texture)Resources.Load(actionName);
//        if (tex != null)
//        {
//            co.go.GetComponent<RawImage>().texture = tex;
//        }

//        co.rt = co.go.GetComponent<RectTransform>();

//        listBlockObject.Add(co);
//    }

//    public void ResetCodeBlocks()
//    {
//        foreach (Transform contentChild in blockOrderContent.transform)
//        {
//            Destroy(contentChild.gameObject);
//        }
//        orderBlockObject.Clear();

//        if (blockSystem != null)
//        {
//            blockSystem.ClearBlocks();
//        }

//        if (selectedObject != null)
//        {
//            Destroy(selectedObject);
//        }

//        isSelecting = false;
//        selectedObject = null;
//        selectedObjectRt = null;
//        selectedIndex = -1;
//        listPanelSR.enabled = true;
//    }
//}

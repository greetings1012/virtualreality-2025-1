using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BlockSystem;

public class CodeblockGenerator : MonoBehaviour
{
    public class CodeObject
    {
        public CodeBlock block;
        public GameObject go;
    }

    public List<CodeObject> listBlockObject = new List<CodeObject>();
    public List<GameObject> orderBlockObject = new List<GameObject>();

    [SerializeField]
    private GameObject blockListContent;
    [SerializeField]
    private GameObject blockOrderContent;
    [SerializeField]
    private GameObject blockPrefab;

    private BlockSystem blockSystem = null;

    void Start()
    {
        blockSystem = GetComponent<BlockSystem>();

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
    }

    public void AddOrderBlock(GameObject orderBlock)
    {
        orderBlockObject.Add(Instantiate(orderBlock, blockOrderContent.transform));
        
        foreach(var i in listBlockObject)
        {
            if (i.go.name + "(Clone)" == orderBlock.name)
            {
                blockSystem.AddBlock(i.block);
            }
        }
    }

    public void AddBlock(string blockName, string actionName, int repeatCount)
    {
        CodeObject co = new CodeObject();

        CodeBlock cb = new CodeBlock();
        cb.blockName = blockName;
        cb.actionName = actionName;
        cb.repeatCount = repeatCount;

        string prefabsName = "Prefabs/" + actionName;

        co.block = cb;
        co.go = Instantiate((GameObject)Resources.Load(prefabsName), blockListContent.transform);
        co.go.name = actionName;

        listBlockObject.Add(co);
    }

    public void ResetCodeBlocks()
    {
        //foreach (Transform contentChild in blockOrderContent.transform)
        //{
        //    Destroy(contentChild.gameObject);
        //}
        //orderBlockObject.Clear();

        //if (blockSystem != null)
        //{
        //    blockSystem.ClearBlocks();
        //}
    }
}

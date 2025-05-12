using UnityEngine;
using System.Collections.Generic;

public class BlockSystem : MonoBehaviour
{
    [System.Serializable]
    public class CodeBlock
    {
        public string blockName;
        public string actionName;
        public int repeatCount = 1;
    }

    [Header("Block Settings")]
    public List<CodeBlock> blockSequence = new List<CodeBlock>();
    public RobotController robotController;

    [Header("Execution Settings")]
    public float executionDelay = 0.5f;
    private int currentBlockIndex = 0;
    private bool isExecuting = false;

    private void Start()
    {
        if (robotController == null)
        {
            robotController = FindObjectOfType<RobotController>();
        }
    }

    public void AddBlock(CodeBlock block)
    {
        blockSequence.Add(block);
    }

    public void ClearBlocks()
    {
        blockSequence.Clear();
        currentBlockIndex = 0;
    }

    public void ExecuteSequence()
    {
        if (!isExecuting)
        {
            isExecuting = true;
            currentBlockIndex = 0;
            ExecuteNextBlock();
        }
    }

    private void ExecuteNextBlock()
    {
        if (currentBlockIndex < blockSequence.Count)
        {
            CodeBlock currentBlock = blockSequence[currentBlockIndex];
            
            for (int i = 0; i < currentBlock.repeatCount; i++)
            {
                robotController.ExecuteAction(currentBlock.actionName);
            }

            currentBlockIndex++;
            Invoke(nameof(ExecuteNextBlock), executionDelay);
        }
        else
        {
            isExecuting = false;
        }
    }
} 
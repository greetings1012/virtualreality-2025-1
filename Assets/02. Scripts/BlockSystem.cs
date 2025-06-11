using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static BlockSystem;
using System.Collections;
using static Unity.Collections.AllocatorManager;
using System;

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
    public float executionDelay = 0.5F;
    private int currentBlockIndex = 0;
    private bool isExecuting = false;

    // 추후 상수 파일로 분리
    private const string FOR_BLOCK_NAME = "for";

    private void Start()
    {
        if (robotController == null)
        {
            robotController = FindObjectOfType<RobotController>();
        }
    }

    void Update()
    {
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

    public void RemoveBlock(int index)
    {
        if (index >= 0 && index < blockSequence.Count)
        {
            blockSequence.RemoveAt(index);
        }
    }
    
    private bool IsForBlock(CodeBlock codeBlock)
    {
        return codeBlock.blockName == FOR_BLOCK_NAME;
    }

    private void ExecuteForBlock()
    {
        if (currentBlockIndex + 2 > blockSequence.Count) return;

        CodeBlock repeatCountBlock = blockSequence[currentBlockIndex + 1];
        CodeBlock actionBlock = blockSequence[currentBlockIndex + 2];

        Match match = Regex.Match(repeatCountBlock.blockName, @"\d+");
        if (!match.Success) return;

        int repeatCountInLoop = int.Parse(match.Value);

        if (!IsForBlock(actionBlock))
        {
            StartCoroutine(PerformForLoop(actionBlock, repeatCountInLoop));
        }
    }

    private IEnumerator PerformForLoop(CodeBlock actionBlock, int repeatCount)
    {
        for (int i = 0; i < repeatCount; i++)
        {
            robotController.commandCompleted = false;
            robotController.ExecuteAction(actionBlock.actionName);
            yield return new WaitUntil(() => robotController.commandCompleted);
            yield return new WaitForSeconds(executionDelay);
        }

        currentBlockIndex += 3;
        ExecuteNextBlock();
    }

    private void ExecuteActionBlock(CodeBlock currentBlock)
    {
        StartCoroutine(PerformActionWithDelay(currentBlock));
    }

    IEnumerator PerformActionWithDelay(CodeBlock currentBlock)
    {
        for (int i = 0; i < currentBlock.repeatCount; i++)
        {
            robotController.commandCompleted = false;
            robotController.ExecuteAction(currentBlock.actionName);
            yield return new WaitUntil(() => robotController.commandCompleted);
            yield return new WaitForSeconds(executionDelay);
        }

        currentBlockIndex++;
        ExecuteNextBlock();
    }

    private void ExecuteNextBlock()
    {
        if (currentBlockIndex < blockSequence.Count)
        {
            CodeBlock currentBlock = blockSequence[currentBlockIndex];

            if (IsForBlock(currentBlock)) { ExecuteForBlock(); }
            else { ExecuteActionBlock(currentBlock); }
        }
        else
        {
            isExecuting = false;
        }
    }
}

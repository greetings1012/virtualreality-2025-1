using Unity.Hierarchy;
using UnityEngine;

public class CodeblockFlowview : MonoBehaviour
{
    [SerializeField]
    private GameObject GameOrderPanel;
    [SerializeField]
    private GameObject XRCamera;

    [SerializeField]
    private bool isHorizontal = true;
    private float scrollRatio = 0.0F;

    private void Start()
    {
        
    }

    void Update()
    {
        if (GameOrderPanel.transform.childCount <= 0)
            return;

        if (isHorizontal)
        {
            GameOrderPanel.transform.GetChild(0).transform.rotation = transform.rotation;
            GameOrderPanel.transform.GetChild(0).transform.position = transform.position
                - GameOrderPanel.transform.GetChild(0).transform.forward * 0.07F
                - GameOrderPanel.transform.GetChild(0).transform.right * 0.7F
                - GameOrderPanel.transform.GetChild(0).transform.up * 0.15F;

            for (int i = 1; i < GameOrderPanel.transform.childCount; i++)
            {
                GameOrderPanel.transform.GetChild(i).transform.rotation = GameOrderPanel.transform.GetChild(i - 1).transform.rotation;
                GameOrderPanel.transform.GetChild(i).transform.position = GameOrderPanel.transform.GetChild(i - 1).transform.position
                    + GameOrderPanel.transform.GetChild(i - 1).transform.right * 0.31F; // 0.3 -> scale transform
            }
        }
        else
        {
            GameOrderPanel.transform.GetChild(0).transform.rotation = transform.rotation;
            GameOrderPanel.transform.GetChild(0).transform.position = transform.position
                - GameOrderPanel.transform.GetChild(0).transform.forward * 0.07F
                + GameOrderPanel.transform.GetChild(0).transform.up * 0.25F;

            for (int i = 1; i < GameOrderPanel.transform.childCount; i++)
            {
                GameOrderPanel.transform.GetChild(i).transform.rotation = GameOrderPanel.transform.GetChild(i - 1).transform.rotation;
                GameOrderPanel.transform.GetChild(i).transform.position = GameOrderPanel.transform.GetChild(i - 1).transform.position
                    - GameOrderPanel.transform.GetChild(i - 1).transform.up * 0.35F; // 0.3 -> scale transform
            }
        }
    }
}

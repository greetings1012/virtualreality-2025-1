using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEngine;

public class CodeblockFlowview : MonoBehaviour
{
    [SerializeField]
    private GameObject GameOrderPanel;
    [SerializeField]
    private GameObject XRCamera;
    [SerializeField]
    private GameObject xrInputSystemObj;
    private XRInputSystem xrInputSystem;

    private float scrollVelocity = 1.0F;

    [SerializeField]
    private bool isHorizontal = true;
    private Vector2 scrollRatio = Vector2.zero;
    private float dampingFactor = 0.83F;

    private void Start()
    {
        xrInputSystem = xrInputSystemObj.GetComponent<XRInputSystem>();
    }

    void Update()
    {
        Vector2 damp = new Vector2(-dampingFactor, -dampingFactor) * xrInputSystem.ScrollRatio * Time.deltaTime;
        xrInputSystem.ScrollRatio = xrInputSystem.ScrollRatio * scrollVelocity + damp;
        scrollRatio += xrInputSystem.ScrollRatio * Time.deltaTime;
        scrollRatio.y = Mathf.Clamp(scrollRatio.y, 0.0F, 2.384F);

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
                + GameOrderPanel.transform.GetChild(0).transform.up * (0.25F + scrollRatio.y);

            for (int i = 1; i < GameOrderPanel.transform.childCount; i++)
            {
                GameOrderPanel.transform.GetChild(i).transform.rotation = GameOrderPanel.transform.GetChild(i - 1).transform.rotation;
                GameOrderPanel.transform.GetChild(i).transform.position = GameOrderPanel.transform.GetChild(i - 1).transform.position
                    - GameOrderPanel.transform.GetChild(i - 1).transform.up * 0.35F; // 0.3 -> scale transform
            }
        }
    }
}

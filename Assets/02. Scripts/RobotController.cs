using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RobotController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    private float desiredRotationEuler = 90.0F;
    // [Header("Movement Settings")]

    private float robotMovementTotalTime = 1.5F; // 로봇이 한 보폭을 가는데 걸리는 총 시간 (기본: 1.5초)

    private Animator animator;

    // 로봇의 한 보폭(거리)에 관한 예측값 (오차가 다소 있다.)
    private float desiredDeltaPosition = 5.0F;

    [Header("Components")]
    private Rigidbody rb;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    
    private GameSystem gameSystem;
    public bool commandCompleted = false;
    
    // 게임 클리어를 위한 아이템을 먹었는지 확인하는 변수
    public bool isClearCondition = false;
    // 게임 클리어를 했는지 확인하는 변
    public bool isGameClear = false;

    [SerializeField]
    private GameObject fadeOutScriptObject;
    private FadeOutScript fadeOutScript;
    [SerializeField]
    private GameObject destinationObject;
    private Animator destination;

    private void Start()
    {
        destination = destinationObject.GetComponent<Animator>();
        fadeOutScript = fadeOutScriptObject.GetComponent<FadeOutScript>();
        animator = GetComponent<Animator>();

        initialPosition = transform.position;
        initialRotation = transform.rotation;

        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    void Update()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        float checkDistance = 1.0f;

        RaycastHit hit;
        if (Physics.Raycast(origin, Vector3.down, out hit, checkDistance))
        {
            if (!hit.collider.CompareTag("Tile"))
            {
                gameSystem.GameOver();
            }
        }
        else
        {
            Debug.Log("오류 발생");
        }
    }

    // https://www.desmos.com/calculator/wgqvdrzuju
    float Sigmoid(float t)
    {
        return (1.0F / (1.0F + Mathf.Exp(-11.6F * (t - 0.5F))));
    }

    public void MoveForward()
    {
        Vector3 targetPosition = transform.position + transform.forward * desiredDeltaPosition;
        targetPosition = FetchPosition(targetPosition);

        StartCoroutine(MovingForward(transform.position, targetPosition));
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GoalItem"))
        {
            Debug.Log("게임 클리어를 위한 조건 만족");
            isClearCondition = true;
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Goal"))
        {
            Debug.Log("게임 클리어");
            isGameClear = true;
        }
    }

    private IEnumerator MovingForward(Vector3 origin, Vector3 targetPosition)
    {
        float step = Time.deltaTime / robotMovementTotalTime;
        float movedDistance = 0.0F;
        float totalTime = 0.0F;

        while (movedDistance <= 1.0F)
        {
            totalTime += Time.deltaTime;

            float beautifulValue = Sigmoid(movedDistance);
            transform.position = Vector3.Lerp(origin, targetPosition, beautifulValue);
            movedDistance += step;

            animator.SetFloat("vel", totalTime);

            yield return null;
        }

        transform.position = targetPosition;

        commandCompleted = true;
    }

    private IEnumerator RotateCour(Quaternion a, Quaternion b)
    {
        float step = Time.deltaTime;
        float movedDistance = 0.0F;

        while (movedDistance <= 1.0F)
        {
            transform.rotation = Quaternion.Slerp(a, b, movedDistance);
            movedDistance += step;

            yield return null;
        }

        transform.rotation = b;

        commandCompleted = true;
    }

    public void RotateLeft()
    {
        Quaternion a = transform.rotation;
        transform.Rotate(-Vector3.up * desiredRotationEuler);
        Quaternion b = transform.rotation;

        StartCoroutine(RotateCour(a, b));
    }

    public void RotateRight()
    {
        Quaternion a = transform.rotation;
        transform.Rotate(Vector3.up * desiredRotationEuler);
        Quaternion b = transform.rotation;

        StartCoroutine(RotateCour(a, b));
    }

    public void ExecuteAction(string actionName)
    {
        switch (actionName)
        {
            case "MoveForward":
                MoveForward();
                break;
            case "RotateLeft":
                RotateLeft();
                break;
            case "RotateRight":
                RotateRight();
                break;
            // 추가 동작들은 여기에 구현
            
        }
    }

    // 그냥 앞으로 움직이기만 하면 xz위치가 미묘하게 안맞는데 그걸 맞추는 함수
    private Vector3 FetchPosition(Vector3 targetPos)
    {
        Vector3 magicNumber = Vector3.up * 5.0F;

        Ray ray = new Ray();
        ray.direction = -Vector3.up;
        ray.origin = targetPos + magicNumber;

        RaycastHit hit;
        bool isCollided = Physics.Raycast(ray, out hit, 100.0F, LayerMask.GetMask("Tile"));
        Debug.DrawRay(targetPos, -Vector3.up, Color.red, 200.0F);

        if (isCollided)
        {
            // 비용이 큰 연산입니다. 참고 해주세요.
            // 최적화 전략1. 프리팹의 anchor를 정중앙으로 바꾼다.
            // 최적화 전략2. Start에서 모든 Tile의 프리팹 Renderer Component정보를 미리 Load해서 hashing형식으로 가져온다.
            Vector3 center = hit.transform.Find("Target").transform.position;

            targetPos = new Vector3(center.x, targetPos.y, center.z);
            fadeOutScript.DestroyAFOCommand();
            Debug.Log("Fetched !");
        }

        // else하면 안되요
        if (!isCollided)
        {
            Debug.Log("타일간의 위치가 맞지 않습니다 ! 타일의 위치를 조정해주세요 !");
            fadeOutScript.AnimateFadeOut();
        }

        return targetPos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "DEST")
        {
            fadeOutScript.DestroyAFOCommand();
            animator.SetTrigger("danceTrigger");
            destination.SetTrigger("danceTrigger");

            StartCoroutine(LoadNextScene(3.0F));
        }
    }

    IEnumerator LoadNextScene(float wait)
    {
        Instantiate((GameObject)Resources.Load("par"), transform.position, Quaternion.identity);
        yield return new WaitForSeconds(wait);

        if (SceneManager.GetActiveScene().name == "Stage_1")
        {
            SceneManager.LoadScene("Stage_2");
        }
        else if (SceneManager.GetActiveScene().name == "Stage_2")
        {
            SceneManager.LoadScene("Stage_3");
        }
        else if (SceneManager.GetActiveScene().name == "Stage_3")
        {
            SceneManager.LoadScene("StageSelect");
        }

        yield return null;
    }

    public void ResetToInitialPosition()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.SetPositionAndRotation(initialPosition, initialRotation);
    }
} 
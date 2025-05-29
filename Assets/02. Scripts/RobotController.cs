using TMPro;
using UnityEngine;
using System.Collections;


public class RobotController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float desiredDeltaPosition = 5.0F;
    public float desiredRotationEuler = 90.0F;

    [Header("Components")]
    private Rigidbody rb;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    
    private GameSystem gameSystem;

    private void Start()
    {
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

    public void MoveForward()
    {
        float moveDistance = 0.1f;
        Vector3 targetPosition = transform.position + transform.forward * desiredDeltaPosition;
        
        // 지속적으로 움직여야 하는게 아니라서 델타타임 필요 없을것같아요.
        //transform.Translate(Vector3.forward * desiredDeltaPosition);
        // transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        //transform.position = Vector3.MoveTowards(gameObject.transform.position, targetPosition, moveDistance);

        StartCoroutine(MovingForward(moveDistance, targetPosition));
        FetchPosition();
    }

    private IEnumerator MovingForward(float moveDistance, Vector3 targetPosition)
    {
        float movedDistance = moveDistance;

        while (movedDistance <= desiredDeltaPosition) {
            transform.position = Vector3.MoveTowards(gameObject.transform.position, targetPosition, moveDistance);
            movedDistance += moveDistance;
            yield return null;
        }
    }

    public void RotateLeft()
    {
        transform.Rotate(-Vector3.up * desiredRotationEuler);
        // transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
    }

    public void RotateRight()
    {
        transform.Rotate(Vector3.up * desiredRotationEuler);
        // transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
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
    private void FetchPosition()
    {
        Ray ray = new Ray();
        ray.direction = -Vector3.up;
        ray.origin = transform.position;

        RaycastHit hit;
        bool isCollided = Physics.Raycast(ray, out hit, 1.5F, LayerMask.GetMask("Tile"));
        if (isCollided)
        {
            transform.position = new Vector3(hit.transform.position.x, transform.position.y, hit.transform.position.z);
        }

        // else하면 안되요
        if (!isCollided)
        {
            Debug.Log("타일간의 위치가 맞지 않습니다 ! 타일의 위치를 조정해주세요 !");
        }
    }

    public void ResetToInitialPosition()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.SetPositionAndRotation(initialPosition, initialRotation);
    }
} 
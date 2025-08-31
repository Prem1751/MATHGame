using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [Header("การตั้งค่า")]
    public bool lookAtPlayer = true;
    public bool useSmoothRotation = true;
    public float rotationSpeed = 5f;
    public float maxViewDistance = 15f;

    private Transform player;
    private Transform cameraTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalRotation;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        cameraTransform = Camera.main.transform;
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        originalRotation = transform.eulerAngles;
    }

    void Update()
    {
        if (player == null || cameraTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // แสดง/ซ่อน Text ตามระยะทาง
        if (canvasGroup != null)
        {
            if (distanceToPlayer <= maxViewDistance)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
            else
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                return;
            }
        }

        if (lookAtPlayer)
        {
            RotateTowardsCamera();
        }
    }

    void RotateTowardsCamera()
    {
        // ใช้การหมุนแบบ Billboard ธรรมดา
        transform.LookAt(transform.position + cameraTransform.rotation * Vector3.forward,
                        cameraTransform.rotation * Vector3.up);
    }

    // ฟังก์ชันสำหรับเปิด/ปิดการหันหน้า
    public void SetLookAtPlayer(bool shouldLook)
    {
        lookAtPlayer = shouldLook;

        if (!shouldLook)
        {
            // คืนค่าการหมุนเดิม
            transform.eulerAngles = originalRotation;
        }
    }

    // ฟังก์ชันสำหรับตั้งค่าการมองเห็น
    public void SetVisibility(bool isVisible)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = isVisible ? 1f : 0f;
            canvasGroup.interactable = isVisible;
            canvasGroup.blocksRaycasts = isVisible;
        }
    }
}
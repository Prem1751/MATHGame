using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [Header("��õ�駤��")]
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

        // �ʴ�/��͹ Text ������зҧ
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
        // ������عẺ Billboard ������
        transform.LookAt(transform.position + cameraTransform.rotation * Vector3.forward,
                        cameraTransform.rotation * Vector3.up);
    }

    // �ѧ��ѹ����Ѻ�Դ/�Դ����ѹ˹��
    public void SetLookAtPlayer(bool shouldLook)
    {
        lookAtPlayer = shouldLook;

        if (!shouldLook)
        {
            // �׹��ҡ����ع���
            transform.eulerAngles = originalRotation;
        }
    }

    // �ѧ��ѹ����Ѻ��駤�ҡ���ͧ���
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
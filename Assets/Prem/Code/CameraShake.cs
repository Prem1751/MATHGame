using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.1f;
    public float dampingSpeed = 1.0f;

    private Vector3 initialPosition;
    private float currentShakeDuration = 0f;
    private float currentShakeMagnitude = 0f;

    void Awake()
    {
        if (Camera.main != null)
        {
            initialPosition = Camera.main.transform.localPosition;
        }
    }

    void Update()
    {
        if (currentShakeDuration > 0)
        {
            Camera.main.transform.localPosition = initialPosition + Random.insideUnitSphere * currentShakeMagnitude;
            currentShakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            currentShakeDuration = 0f;
            Camera.main.transform.localPosition = initialPosition;
        }
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        currentShakeDuration = duration;
        currentShakeMagnitude = magnitude;
    }

    public void ShakeCamera()
    {
        currentShakeDuration = shakeDuration;
        currentShakeMagnitude = shakeMagnitude;
    }
}
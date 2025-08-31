using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmMovement : MonoBehaviour
{
    [Header("Arm Settings")]
    public float walkBobSpeed = 10f;
    public float walkBobAmount = 0.05f;
    public float runBobSpeed = 15f;
    public float runBobAmount = 0.1f;
    public float swayAmount = 0.1f;
    public float swaySmoothness = 5f;

    private Vector3 initialPosition;
    private float bobTimer = 0f;
    private FirstPersonController fpsController;
    private CharacterController characterController;

    void Start()
    {
        initialPosition = transform.localPosition;
        fpsController = GetComponentInParent<FirstPersonController>();
        characterController = GetComponentInParent<CharacterController>();
    }

    void Update()
    {
        HandleBobAnimation();
        HandleSway();
    }

    void HandleBobAnimation()
    {
        if (characterController == null) return;

        float speed = GetMovementSpeed();
        float bobSpeed = speed > 5f ? runBobSpeed : walkBobSpeed;
        float bobAmount = speed > 5f ? runBobAmount : walkBobAmount;

        if (characterController.velocity.magnitude > 0.1f && characterController.isGrounded)
        {
            // Bobbing motion
            bobTimer += Time.deltaTime * bobSpeed;
            float bobY = Mathf.Sin(bobTimer) * bobAmount;
            float bobX = Mathf.Cos(bobTimer * 0.5f) * bobAmount * 0.5f;

            Vector3 bobPosition = new Vector3(
                initialPosition.x + bobX,
                initialPosition.y + bobY,
                initialPosition.z
            );

            transform.localPosition = Vector3.Lerp(transform.localPosition, bobPosition, Time.deltaTime * 10f);
        }
        else
        {
            // Return to initial position
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, Time.deltaTime * 5f);
            bobTimer = 0f;
        }
    }

    void HandleSway()
    {
        // Mouse look sway
        float mouseX = Input.GetAxis("Mouse X") * swayAmount;
        float mouseY = Input.GetAxis("Mouse Y") * swayAmount * 0.5f;

        Quaternion targetRotation = Quaternion.Euler(
            -mouseY,
            mouseX,
            mouseX * 0.5f
        );

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            Time.deltaTime * swaySmoothness
        );
    }

    float GetMovementSpeed()
    {
        if (characterController != null)
        {
            return characterController.velocity.magnitude;
        }
        return 0f;
    }

    // เรียกจาก animation events (ถ้ามี)
    public void OnFootstep()
    {
        // สามารถเพิ่ม effect การก้าวเท้าได้ที่นี่
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 10;
    public GameObject pressEText; // ข้อความกด E
    public float activationDistance = 5f;

    private bool playerInRange = false;
    private LookAtPlayer lookAtScript;

    void Start()
    {
        if (pressEText != null)
        {
            pressEText.SetActive(false);

            // เพิ่มหรือหา LookAtPlayer script
            lookAtScript = pressEText.GetComponent<LookAtPlayer>();
            if (lookAtScript == null)
            {
                lookAtScript = pressEText.AddComponent<LookAtPlayer>();
            }
        }
    }

    void Update()
    {
        // ตรวจสอบการกดปุ่ม E เมื่อผู้เล่นอยู่ในระยะ
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            PickUpAmmo();
        }

        // อัพเดทการแสดงผลข้อความ
        UpdateTextVisibility();
    }

    void UpdateTextVisibility()
    {
        if (pressEText != null && lookAtScript != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                bool shouldShow = distance <= activationDistance;

                pressEText.SetActive(shouldShow);
                lookAtScript.SetVisibility(shouldShow);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (pressEText != null)
            {
                pressEText.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (pressEText != null)
            {
                pressEText.SetActive(false);
            }
        }
    }

    void PickUpAmmo()
    {
        int reward = Random.Range(GameManager.instance.minAmmoReward, GameManager.instance.maxAmmoReward + 1);
        GameManager.instance.ShowMathProblem(reward);

        // ปิดข้อความ
        if (pressEText != null)
        {
            pressEText.SetActive(false);
        }

        Destroy(gameObject);
    }
}
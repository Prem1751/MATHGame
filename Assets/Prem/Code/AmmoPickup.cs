using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 10;
    public GameObject pressEText;
    public float activationDistance = 5f;

    private bool playerInRange = false;
    private LookAtPlayer lookAtScript;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (pressEText != null)
        {
            pressEText.SetActive(false);

            lookAtScript = pressEText.GetComponent<LookAtPlayer>();
            if (lookAtScript == null)
            {
                lookAtScript = pressEText.AddComponent<LookAtPlayer>();
            }
        }

        // ตรวจสอบและเพิ่ม Collider
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
        else
        {
            Debug.LogWarning("ไม่มี Collider บน AmmoPickup!");
        }
    }

    void Update()
    {
        UpdateTextVisibility();

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            PickUpAmmo();
        }
    }

    void UpdateTextVisibility()
    {
        if (player == null || pressEText == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        playerInRange = distance <= activationDistance;

        if (pressEText.activeSelf != playerInRange)
        {
            pressEText.SetActive(playerInRange);
        }
    }

    void PickUpAmmo()
    {
        Debug.Log("กด E เพื่อเก็บกระสุนแล้ว");

        // เรียก GameManager โดยตรง
        if (GameManager.instance != null)
        {
            int reward = Random.Range(
                GameManager.instance.minAmmoReward,
                GameManager.instance.maxAmmoReward + 1
            );

            Debug.Log("เรียก ShowMathProblem ด้วย reward: " + reward);
            GameManager.instance.ShowMathProblem(reward);
        }
        else
        {
            Debug.LogError("GameManager.instance is null!");

            // ลองหา GameManager ใน scene
            GameManager manager = FindObjectOfType<GameManager>();
            if (manager != null)
            {
                Debug.Log("พบ GameManager ใน scene");
                int reward = Random.Range(manager.minAmmoReward, manager.maxAmmoReward + 1);
                manager.ShowMathProblem(reward);
            }
            else
            {
                Debug.LogError("ไม่พบ GameManager ใน scene!");
            }
        }

        // ปิดข้อความและทำลาย object
        if (pressEText != null)
        {
            pressEText.SetActive(false);
        }

        Destroy(gameObject);
    }

    // สำหรับการดีบัก
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}
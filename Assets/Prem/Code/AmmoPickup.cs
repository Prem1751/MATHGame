using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 10;
    public GameObject pressEText; // ข้อความกด E
    private bool playerInRange = false;

    void Start()
    {
        if (pressEText != null)
            pressEText.SetActive(false);
    }

    private void Update()
    {
        // ตรวจสอบการกดปุ่ม E เมื่อผู้เล่นอยู่ในระยะ
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            PickUpAmmo();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (pressEText != null)
                pressEText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (pressEText != null)
                pressEText.SetActive(false);
        }
    }

    void PickUpAmmo()
    {
        int reward = Random.Range(GameManager.instance.minAmmoReward, GameManager.instance.maxAmmoReward + 1);
        GameManager.instance.ShowMathProblem(reward);

        // ปิดข้อความกด E
        if (pressEText != null)
            pressEText.SetActive(false);

        Destroy(gameObject);
    }
}
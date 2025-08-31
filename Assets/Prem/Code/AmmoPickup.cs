using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 10;
    public GameObject pressEText; // ��ͤ����� E
    private bool playerInRange = false;

    void Start()
    {
        if (pressEText != null)
            pressEText.SetActive(false);
    }

    private void Update()
    {
        // ��Ǩ�ͺ��á����� E ����ͼ��������������
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

        // �Դ��ͤ����� E
        if (pressEText != null)
            pressEText.SetActive(false);

        Destroy(gameObject);
    }
}
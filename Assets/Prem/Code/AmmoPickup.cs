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

        // ��Ǩ�ͺ������� Collider
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
        else
        {
            Debug.LogWarning("����� Collider �� AmmoPickup!");
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
        Debug.Log("�� E �����纡���ع����");

        // ���¡ GameManager �µç
        if (GameManager.instance != null)
        {
            int reward = Random.Range(
                GameManager.instance.minAmmoReward,
                GameManager.instance.maxAmmoReward + 1
            );

            Debug.Log("���¡ ShowMathProblem ���� reward: " + reward);
            GameManager.instance.ShowMathProblem(reward);
        }
        else
        {
            Debug.LogError("GameManager.instance is null!");

            // �ͧ�� GameManager � scene
            GameManager manager = FindObjectOfType<GameManager>();
            if (manager != null)
            {
                Debug.Log("�� GameManager � scene");
                int reward = Random.Range(manager.minAmmoReward, manager.maxAmmoReward + 1);
                manager.ShowMathProblem(reward);
            }
            else
            {
                Debug.LogError("��辺 GameManager � scene!");
            }
        }

        // �Դ��ͤ�����з���� object
        if (pressEText != null)
        {
            pressEText.SetActive(false);
        }

        Destroy(gameObject);
    }

    // ����Ѻ��ôպѡ
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}
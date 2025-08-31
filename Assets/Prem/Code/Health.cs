using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public bool isPlayer = false;

    [Header("UI Settings")]
    public Slider healthSlider;
    public Image healthFillImage;
    public Color fullHealthColor = Color.green;
    public Color lowHealthColor = Color.red;

    [Header("Effects")]
    public GameObject damageEffect;
    public AudioClip hurtSound;
    public AudioClip deathSound;

    private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1.0f;
        }

        // ตั้งค่า UI
        UpdateHealthUI();

        Debug.Log(gameObject.name + " health initialized: " + currentHealth + "/" + maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Health: " + currentHealth + "/" + maxHealth);

        // สร้างเอฟเฟกต์ damage
        if (damageEffect != null)
        {
            Instantiate(damageEffect, transform.position + Vector3.up, Quaternion.identity);
        }

        // เล่นเสียง被击中
        if (audioSource != null && hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }

        // อัพเดท UI
        UpdateHealthUI();

        // อัพเดท UI ถ้าเป็น玩家
        if (isPlayer && GameManager.instance != null)
        {
            GameManager.instance.UpdateUI();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)currentHealth / maxHealth;
        }

        if (healthFillImage != null)
        {
            healthFillImage.color = Color.Lerp(lowHealthColor, fullHealthColor, (float)currentHealth / maxHealth);
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " died!");

        // เล่นเสียงตาย
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        if (isPlayer)
        {
            Debug.Log("Player died! Game Over");
            // เพิ่มเกมโอเวอร์逻辑ที่นี่
            if (GameManager.instance != null)
            {
                GameManager.instance.GameOver();
            }
        }
        else
        {
            // สำหรับศัตรู
            Destroy(gameObject, 2f);
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log(gameObject.name + " healed for " + amount + ". Health: " + currentHealth + "/" + maxHealth);

        // อัพเดท UI
        UpdateHealthUI();

        if (isPlayer && GameManager.instance != null)
        {
            GameManager.instance.UpdateUI();
        }
    }

    // สำหรับ testing
    void OnMouseDown()
    {
        if (!isPlayer)
        {
            TakeDamage(25);
        }
    }
}
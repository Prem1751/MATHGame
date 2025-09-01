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

    [Header("Damage Screen Effect")]
    public Image redOverlay; // ลาก UI Image มาใส่ที่นี่
    public float flashTime = 0.3f;

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

        // ซ่อนเอฟเฟ็กต์สีแดงตอนเริ่มเกม
        if (redOverlay != null)
        {
            redOverlay.color = new Color(1, 0, 0, 0);
        }

        Debug.Log(gameObject.name + " health initialized: " + currentHealth + "/" + maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Health: " + currentHealth + "/" + maxHealth);

        // แสดงเอฟเฟ็กต์สีแดงเมื่อ玩家ถูกตี
        if (isPlayer && redOverlay != null)
        {
            StartCoroutine(ShowDamageEffect());
        }

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

    // ฟังก์ชันแสดงเอฟเฟ็กต์สีแดง
    private IEnumerator ShowDamageEffect()
    {
        // แสดงสีแดงเต็มจอ
        redOverlay.color = new Color(1, 0, 0, 0.8f);

        // รอสักครู่
        yield return new WaitForSeconds(flashTime);

        // ค่อยๆ จางหาย
        float timer = 0f;
        Color startColor = redOverlay.color;
        Color endColor = new Color(1, 0, 0, 0);

        while (timer < flashTime)
        {
            redOverlay.color = Color.Lerp(startColor, endColor, timer / flashTime);
            timer += Time.deltaTime;
            yield return null;
        }

        redOverlay.color = endColor;
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public bool isPlayer = false;

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
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;

        if (damageEffect != null)
        {
            Instantiate(damageEffect, transform.position, Quaternion.identity);
        }

        if (audioSource != null && hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }

        Debug.Log(gameObject.name + " took " + damage + " damage. Health: " + currentHealth);

        if (isPlayer)
        {
            GameManager.instance.UpdateUI();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        if (isPlayer)
        {
            Debug.Log("Player died! Game Over");
            // เพิ่มเกมโอเวอร์逻辑ที่นี่
        }
        else
        {
            Debug.Log(gameObject.name + " died");
            Destroy(gameObject, 2f);
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        if (isPlayer)
        {
            GameManager.instance.UpdateUI();
        }

        Debug.Log(gameObject.name + " healed for " + amount + ". Health: " + currentHealth);
    }
}
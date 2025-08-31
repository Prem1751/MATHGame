using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    [Header("Zombie Settings")]
    public int health = 100;
    public int damage = 15;
    public float attackRate = 2.0f;
    public float attackRange = 2.0f;
    public float moveSpeed = 1.5f;

    [Header("References")]
    public GameObject bloodEffect;
    public AudioClip[] attackSounds;
    public AudioClip[] deathSounds;
    public AudioClip[] hurtSounds;

    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource;
    private float nextAttackTime = 0f;
    private bool isDead = false;
    private Transform spawnPoint;
    private bool deathAnimationStarted = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1.0f;
            audioSource.maxDistance = 20f;
            audioSource.volume = 0.7f;
        }

        SetupNavMeshAgent();
    }

    void SetupNavMeshAgent()
    {
        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.acceleration = 8f;
            agent.stoppingDistance = attackRange - 0.2f;
            agent.angularSpeed = 120f;
            agent.autoBraking = true;
        }
    }

    public void TakeDamage(int amount, Vector3 hitPoint)
    {
        if (isDead || deathAnimationStarted) return;

        health -= amount;
        Debug.Log("Zombie took " + amount + " damage. Health: " + health + "/100");

        // สร้างเอฟเฟกต์เลือด
        if (bloodEffect != null)
        {
            GameObject blood = Instantiate(bloodEffect, hitPoint, Quaternion.identity);
            Destroy(blood, 2f);
        }

        // เล่นเสียงถูกยิง
        if (audioSource != null && hurtSounds.Length > 0)
        {
            audioSource.PlayOneShot(hurtSounds[Random.Range(0, hurtSounds.Length)]);
        }

        if (health <= 0 && !deathAnimationStarted)
        {
            Die();
        }
    }

    void Update()
    {
        if (isDead) return;

        UpdateAnimation();

        if (player != null && agent != null && agent.isActiveAndEnabled)
        {
            agent.SetDestination(player.position);

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + attackRate;
            }
        }
    }

    void UpdateAnimation()
    {
        if (animator != null && agent != null)
        {
            // ตั้งค่าความเร็วอนิเมชั่นตามการเคลื่อนที่
            float speed = agent.velocity.magnitude > 0.1f ? 1f : 0f;
            animator.SetFloat("Speed", speed);
        }
    }

    void Attack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
            Debug.Log("Playing attack animation");
        }

        // หันหน้าไปหาผู้เล่น
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        // เล่นเสียงโจมตี
        if (audioSource != null && attackSounds.Length > 0)
        {
            audioSource.PlayOneShot(attackSounds[Random.Range(0, attackSounds.Length)]);
        }

        // ทำ damage ทันทีเมื่อโจมตี
        ApplyDamage();
    }

    public void ApplyDamage()
    {
        if (player != null && !isDead)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= attackRange + 1f)
            {
                Health playerHealth = player.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    Debug.Log("Zombie attacked player for " + damage + " damage");

                    if (bloodEffect != null)
                    {
                        Vector3 bloodPos = player.position + Vector3.up * 1.5f;
                        GameObject blood = Instantiate(bloodEffect, bloodPos, Quaternion.identity);
                        Destroy(blood, 2f);
                    }
                }
            }
        }
    }

    void Die()
    {
        if (deathAnimationStarted) return;

        deathAnimationStarted = true;
        isDead = true;
        Debug.Log("Zombie is dying");

        // หยุดการเคลื่อนที่
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        // เล่นอนิเมชั่นตาย
        if (animator != null)
        {
            animator.SetTrigger("Death");
            Debug.Log("Playing death animation");
        }

        // ปิด Collider
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }

        // เล่นเสียงตาย
        if (audioSource != null && deathSounds.Length > 0)
        {
            audioSource.PlayOneShot(deathSounds[Random.Range(0, deathSounds.Length)]);
        }

        // แจ้ง GameManager
        if (GameManager.instance != null)
        {
            if (spawnPoint != null)
            {
                GameManager.instance.ZombieKilled(gameObject, spawnPoint);
            }
            else
            {
                GameManager.instance.ZombieKilled(gameObject);
            }
        }

        // ลบ object หลังจาก 3 วินาที
        Destroy(gameObject, 3f);
    }

    // ✅ Method สำหรับ Animation Event (ถ้ามี)
    public void OnDeathAnimationComplete()
    {
        Debug.Log("Death animation complete - destroying zombie");
        Destroy(gameObject);
    }

    public void SetSpawnPoint(Transform point)
    {
        spawnPoint = point;
    }

    public bool IsDead()
    {
        return isDead;
    }
}
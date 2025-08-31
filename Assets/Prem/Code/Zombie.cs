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

    [Header("Animation Settings")]
    public float walkAnimationSpeed = 1.0f;
    public float runAnimationSpeed = 1.5f;

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

    public void TakeDamage(int amount)
    {
        TakeDamage(amount, transform.position + Vector3.up * 1.5f);
    }

    public void TakeDamage(int amount, Vector3 hitPoint)
    {
        if (isDead) return;

        health -= amount;

        if (bloodEffect != null)
        {
            Instantiate(bloodEffect, hitPoint, Quaternion.identity);
        }

        // 触发被击中动画
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        if (audioSource != null && hurtSounds.Length > 0)
        {
            audioSource.PlayOneShot(hurtSounds[Random.Range(0, hurtSounds.Length)]);
        }

        Debug.Log("Zombie took " + amount + " damage. Health: " + health);

        if (health <= 0)
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
            // ตั้งค่าความเร็วสำหรับอนิเมชัน
            float speed = agent.velocity.magnitude / agent.speed;
            animator.SetFloat("Speed", speed);
        }
    }

    void Attack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        if (audioSource != null && attackSounds.Length > 0)
        {
            audioSource.PlayOneShot(attackSounds[Random.Range(0, attackSounds.Length)]);
        }
    }

    // ✅ เมธอดนี้ต้องมีสำหรับ Animation Event
    public void ApplyDamage()
    {
        if (player != null && !isDead)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= attackRange + 0.5f)
            {
                Health playerHealth = player.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    Debug.Log("Zombie attacked player for " + damage + " damage");
                }
            }
        }
    }

    void Die()
    {
        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        if (agent != null)
        {
            agent.isStopped = true;
        }

        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }

        if (audioSource != null && deathSounds.Length > 0)
        {
            audioSource.PlayOneShot(deathSounds[Random.Range(0, deathSounds.Length)]);
        }

        if (spawnPoint != null)
        {
            GameManager.instance.ZombieKilled(gameObject, spawnPoint);
        }
        else
        {
            GameManager.instance.ZombieKilled(gameObject);
        }

        Debug.Log("Zombie died");
    }

    // ✅ เมธอดนี้ต้องมีสำหรับ Animation Event
    public void OnDeathAnimationComplete()
    {
        Destroy(gameObject, 2f);
    }

    public void SetSpawnPoint(Transform point)
    {
        spawnPoint = point;
    }

    public bool IsDead()
    {
        return isDead;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    // สำหรับ testing
    void OnMouseDown()
    {
        TakeDamage(25);
    }
}
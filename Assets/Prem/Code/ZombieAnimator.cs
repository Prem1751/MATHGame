using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    public float walkAnimationSpeed = 1.0f;
    public float runAnimationSpeed = 1.5f;

    private Animator animator;
    private NavMeshAgent agent;
    private Zombie zombieScript;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        zombieScript = GetComponent<Zombie>();

        if (animator != null)
        {
            animator.SetFloat("WalkSpeed", walkAnimationSpeed);
            animator.SetFloat("RunSpeed", runAnimationSpeed);
        }
    }

    void Update()
    {
        if (animator != null && agent != null)
        {
            // ตั้งค่าความเร็วสำหรับอนิเมชัน
            float speed = agent.velocity.magnitude / agent.speed;
            animator.SetFloat("Speed", speed);

            // ตั้งสภาวะ dying
            if (zombieScript != null && zombieScript.IsDead())
            {
                animator.SetTrigger("Death");
            }
        }
    }

    // เรียกจาก Zombie script เมื่อโจมตี
    public void TriggerAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    // เรียกจาก Zombie script เมื่อถูกยิง
    public void TriggerHit()
    {
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
    }

    // เรียกจาก Zombie script เมื่อตาย
    public void TriggerDeath()
    {
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
    }

    // Animation Event - เรียกเมื่ออนิเมชันโจมตีถึงจุดที่ควรทำ damage
    public void OnAttackHit()
    {
        if (zombieScript != null)
        {
            zombieScript.ApplyDamage();
        }
    }

    // Animation Event - เรียกเมื่ออนิเมชันตายจบ
    public void OnDeathComplete()
    {
        if (zombieScript != null)
        {
            zombieScript.OnDeathAnimationComplete();
        }
    }
}
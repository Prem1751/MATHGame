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
            // ��駤�Ҥ�����������Ѻ͹����ѹ
            float speed = agent.velocity.magnitude / agent.speed;
            animator.SetFloat("Speed", speed);

            // �������� dying
            if (zombieScript != null && zombieScript.IsDead())
            {
                animator.SetTrigger("Death");
            }
        }
    }

    // ���¡�ҡ Zombie script ���������
    public void TriggerAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    // ���¡�ҡ Zombie script ����Ͷ١�ԧ
    public void TriggerHit()
    {
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
    }

    // ���¡�ҡ Zombie script ����͵��
    public void TriggerDeath()
    {
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
    }

    // Animation Event - ���¡�����͹����ѹ���ն֧�ش����÷� damage
    public void OnAttackHit()
    {
        if (zombieScript != null)
        {
            zombieScript.ApplyDamage();
        }
    }

    // Animation Event - ���¡�����͹����ѹ��¨�
    public void OnDeathComplete()
    {
        if (zombieScript != null)
        {
            zombieScript.OnDeathAnimationComplete();
        }
    }
}
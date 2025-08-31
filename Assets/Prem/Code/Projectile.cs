using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 50f;
    public int damage = 25;
    public GameObject impactEffect;
    public LayerMask collisionMask;

    private Vector3 direction;
    private float currentSpeed;
    private float lifetime = 3f;
    private float currentLifetime = 0f;

    public void SetDirection(Vector3 dir, float spd)
    {
        direction = dir;
        currentSpeed = spd;

        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    void Update()
    {
        currentLifetime += Time.deltaTime;
        if (currentLifetime >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        float moveDistance = currentSpeed * Time.deltaTime;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, out hit, moveDistance, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
        }
        else
        {
            transform.Translate(direction * moveDistance, Space.World);
        }
    }

    void OnHitObject(RaycastHit hit)
    {
        if (hit.collider.CompareTag("Enemy"))
        {
            Zombie zombie = hit.collider.GetComponent<Zombie>();
            if (zombie != null)
            {
                zombie.TakeDamage(damage, hit.point);
            }
        }

        if (impactEffect != null)
        {
            Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Bullet"))
            return;

        if (other.CompareTag("Enemy"))
        {
            Zombie zombie = other.GetComponent<Zombie>();
            if (zombie != null)
            {
                zombie.TakeDamage(damage, other.ClosestPoint(transform.position));
            }
        }

        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
}
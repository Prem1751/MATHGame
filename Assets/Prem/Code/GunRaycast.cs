using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRaycast : MonoBehaviour
{
    [Header("Gun Settings")]
    public int damagePerShot = 25;
    public float range = 100f;
    public float fireRate = 0.1f;
    public int maxAmmo = 90;
    public int currentAmmo = 30;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public AudioClip shootSound;

    [Header("References")]
    public Camera fpsCamera;
    public LayerMask shootableMask;

    private float nextFireTime = 0f;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime && currentAmmo > 0)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        currentAmmo--;

        // Muzzle flash
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // Sound
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        // Raycast
        RaycastHit hit;
        Vector3 rayOrigin = fpsCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(rayOrigin, fpsCamera.transform.forward, out hit, range, shootableMask))
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name + " (Tag: " + hit.collider.tag + ")");

            // ตรวจสอบว่ากระสุนถูกซอมบี้
            if (hit.collider.CompareTag("Enemy"))
            {
                Zombie zombie = hit.collider.GetComponent<Zombie>();
                if (zombie == null)
                {
                    // ลองหาใน parent หรือ children
                    zombie = hit.collider.GetComponentInParent<Zombie>();
                    if (zombie == null)
                    {
                        zombie = hit.collider.GetComponentInChildren<Zombie>();
                    }
                }

                if (zombie != null)
                {
                    zombie.TakeDamage(damagePerShot, hit.point);
                    Debug.Log("Successfully damaged zombie!");
                }
                else
                {
                    Debug.LogWarning("Zombie component not found on: " + hit.collider.gameObject.name);
                }
            }

            // สร้าง impact effect
            if (impactEffect != null)
            {
                Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
        else
        {
            Debug.Log("No hit detected");
        }

        // Update UI
        if (GameManager.instance != null)
        {
            GameManager.instance.UpdateUI();
        }
    }

    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Min(currentAmmo + amount, maxAmmo);
        if (GameManager.instance != null)
        {
            GameManager.instance.UpdateUI();
        }
    }

    // สำหรับ debug
    void OnDrawGizmos()
    {
        if (fpsCamera != null)
        {
            Vector3 rayOrigin = fpsCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
            Gizmos.color = Color.red;
            Gizmos.DrawRay(rayOrigin, fpsCamera.transform.forward * range);
        }
    }
}
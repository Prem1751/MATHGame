using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("ปืน settings")]
    public int currentAmmo = 30;
    public int maxAmmo = 90;
    public float fireRate = 0.1f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 30f;

    private float nextTimeToFire = 0f;
    private Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            playerCamera = GetComponentInParent<Camera>();
        }
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && currentAmmo > 0)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        currentAmmo--;

        // หาจุดที่เล็ง (กลางหน้าจอ)
        Vector3 targetPoint;
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(1000f);
        }

        // คำนวณทิศทาง
        Vector3 direction = (targetPoint - firePoint.position).normalized;

        // สร้างกระสุน
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));

            // ตั้งความเร็วกระสุน
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * bulletSpeed;
            }

            Debug.Log("กระสุนออกแล้ว! ทิศทาง: " + direction);
        }

        // อัพเดท UI
        if (GameManager.instance != null)
        {
            GameManager.instance.UpdateUI();
        }
    }

    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
        if (currentAmmo > maxAmmo)
            currentAmmo = maxAmmo;

        if (GameManager.instance != null)
        {
            GameManager.instance.UpdateUI();
        }
    }
}
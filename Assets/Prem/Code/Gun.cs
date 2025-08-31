using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Gun : MonoBehaviour
{
    [Header("การตั้งค่าปืน")]
    public float fireRate = 0.1f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 30f;
    public ParticleSystem muzzleFlash;
    public AudioClip shootSound;

    [Header("ระบบรีโหลด")]
    public int currentAmmo = 10;
    public int magazineSize = 30;
    public int reserveAmmo = 90;
    public int maxReserveAmmo = 180;
    public float reloadTime = 2.0f;
    public bool autoReload = true;
    public KeyCode reloadKey = KeyCode.R;

    [Header("UI References")]
    public Image reloadCircle;
    public TextMeshProUGUI ammoText;
    public GameObject reloadPanel;

    [Header("Sound Effects")]
    public AudioClip reloadSound;
    public AudioClip dryFireSound;

    private float nextTimeToFire = 0f;
    private Camera playerCamera;
    private AudioSource audioSource;
    private bool isReloading = false;
    private float reloadTimer = 0f;

    void Start()
    {
        playerCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        UpdateAmmoUI();
        HideReloadUI();
    }

    void Update()
    {
        HandleShooting();
        HandleReloadInput();
        UpdateReloadProgress();
    }

    void HandleShooting()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && !isReloading)
        {
            if (currentAmmo > 0)
            {
                Shoot();
            }
            else if (reserveAmmo > 0 && !isReloading)
            {
                // พยายามยิงแต่กระสุนหมด
                if (audioSource != null && dryFireSound != null)
                {
                    audioSource.PlayOneShot(dryFireSound);
                }

                if (autoReload)
                {
                    StartReload();
                }
            }
        }
    }

    void HandleReloadInput()
    {
        if (isReloading) return;

        // รีโหลดเมื่อกดปุ่ม R หรือกระสุนหมด (ถ้าเปิด autoReload)
        if (Input.GetKeyDown(reloadKey) || (autoReload && currentAmmo == 0 && reserveAmmo > 0))
        {
            StartReload();
        }
    }

    void StartReload()
    {
        if (reserveAmmo <= 0 || currentAmmo >= magazineSize || isReloading) return;

        isReloading = true;
        reloadTimer = 0f;

        // แสดง UI รีโหลด
        ShowReloadUI();

        // เล่นเสียงรีโหลด
        if (audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }

        Debug.Log("เริ่มรีโหลด...");
    }

    void UpdateReloadProgress()
    {
        if (!isReloading) return;

        reloadTimer += Time.deltaTime;

        // อัพเดท UI วงกลม
        if (reloadCircle != null)
        {
            float progress = reloadTimer / reloadTime;
            reloadCircle.fillAmount = progress;
        }

        // รีโหลดเสร็จสิ้น
        if (reloadTimer >= reloadTime)
        {
            CompleteReload();
        }
    }

    void CompleteReload()
    {
        int ammoNeeded = magazineSize - currentAmmo;
        int ammoToAdd = Mathf.Min(ammoNeeded, reserveAmmo);

        currentAmmo += ammoToAdd;
        reserveAmmo -= ammoToAdd;

        isReloading = false;
        HideReloadUI();

        UpdateAmmoUI();
        Debug.Log("รีโหลดเสร็จสิ้น! กระสุนปัจจุบัน: " + currentAmmo);
    }

    void Shoot()
    {
        currentAmmo--;
        nextTimeToFire = Time.time + fireRate;

        // Muzzle Flash
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // Sound
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        // หาจุดที่เล็ง
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

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

            // ตั้งค่าความเร็วกระสุน
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * bulletSpeed;
            }

            // ตั้งค่า damage
            Projectile projectile = bullet.GetComponent<Projectile>();
            if (projectile != null)
            {
                // สามารถตั้งค่าความเสียหายได้ที่นี่
            }
        }

        UpdateAmmoUI();
    }

    public void AddAmmo(int amount)
    {
        reserveAmmo = Mathf.Min(reserveAmmo + amount, maxReserveAmmo);
        UpdateAmmoUI();
        Debug.Log("ได้รับกระสุน " + amount + " นัด! คลัง: " + reserveAmmo);
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo} / {reserveAmmo}";
        }

        // อัพเดท GameManager UI
        if (GameManager.instance != null)
        {
            GameManager.instance.UpdateUI();
        }
    }

    void ShowReloadUI()
    {
        if (reloadPanel != null)
        {
            reloadPanel.SetActive(true);
        }

        if (reloadCircle != null)
        {
            reloadCircle.fillAmount = 0f;
        }
    }

    void HideReloadUI()
    {
        if (reloadPanel != null)
        {
            reloadPanel.SetActive(false);
        }
    }

    public bool CanShoot()
    {
        return !isReloading && currentAmmo > 0;
    }

    public bool IsReloading()
    {
        return isReloading;
    }

    // สำหรับ testing
    public void ForceReload()
    {
        if (!isReloading)
        {
            StartReload();
        }
    }

    public void SetAmmo(int current, int reserve)
    {
        currentAmmo = Mathf.Clamp(current, 0, magazineSize);
        reserveAmmo = Mathf.Clamp(reserve, 0, maxReserveAmmo);
        UpdateAmmoUI();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GunReloadSystem : MonoBehaviour
{
    [Header("กระสุน Settings")]
    public int currentAmmo = 10; // กระสุนในแม็กกาซีนปัจจุบัน
    public int magazineSize = 30; // ความจุแม็กกาซีน
    public int reserveAmmo = 90; // กระสุนในคลัง
    public int maxReserveAmmo = 180; // ความจุคลังสูงสุด
    
    [Header("รีโหลด Settings")]
    public float reloadTime = 2.0f; // เวลารีโหลด
    public bool autoReload = true; // รีโหลดอัตโนมัติเมื่อกระสุนหมด
    public KeyCode reloadKey = KeyCode.R; // ปุ่มรีโหลด
    
    [Header("UI References")]
    public Image reloadCircle; // วงกลมรีโหลด
    public TextMeshProUGUI ammoText; // ข้อความแสดงกระสุน
    public GameObject reloadPanel; // Panel รีโหลด
    
    [Header("Sound Effects")]
    public AudioClip reloadSound;
    public AudioClip dryFireSound;
    
    private bool isReloading = false;
    private float reloadTimer = 0f;
    private AudioSource audioSource;
    
    void Start()
    {
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
        HandleReloadInput();
        UpdateReloadProgress();
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

        // Debug log เพื่อตรวจสอบ
        Debug.Log("เริ่มรีโหลด");
        Debug.Log("Reload Circle: " + (reloadCircle != null));
        Debug.Log("Reload Panel: " + (reloadPanel != null));

        // แสดง UI รีโหลด
        ShowReloadUI();

        // เล่นเสียงรีโหลด
        if (audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }
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
    
    public bool Shoot()
    {
        if (isReloading) return false;
        
        if (currentAmmo > 0)
        {
            currentAmmo--;
            UpdateAmmoUI();
            return true;
        }
        else
        {
            // เล่นเสียงเมื่อพยายามยิงแต่กระสุนหมด
            if (audioSource != null && dryFireSound != null)
            {
                audioSource.PlayOneShot(dryFireSound);
            }
            return false;
        }
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
    }

    void ShowReloadUI()
    {
        Debug.Log("พยายามแสดง UI รีโหลด");

        if (reloadPanel != null)
        {
            reloadPanel.SetActive(true);
            Debug.Log("แสดง Panel แล้ว");
        }
        else
        {
            Debug.LogWarning("Reload Panel is null!");
        }

        if (reloadCircle != null)
        {
            reloadCircle.fillAmount = 0f;
            reloadCircle.gameObject.SetActive(true);
            Debug.Log("แสดง Circle แล้ว");
        }
        else
        {
            Debug.LogWarning("Reload Circle is null!");
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

}
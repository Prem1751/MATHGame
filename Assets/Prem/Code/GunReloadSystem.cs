using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GunReloadSystem : MonoBehaviour
{
    [Header("����ع Settings")]
    public int currentAmmo = 10; // ����ع���硡ҫչ�Ѩ�غѹ
    public int magazineSize = 30; // ��������硡ҫչ
    public int reserveAmmo = 90; // ����ع㹤�ѧ
    public int maxReserveAmmo = 180; // �����ؤ�ѧ�٧�ش
    
    [Header("����Ŵ Settings")]
    public float reloadTime = 2.0f; // ��������Ŵ
    public bool autoReload = true; // ����Ŵ�ѵ��ѵ�����͡���ع���
    public KeyCode reloadKey = KeyCode.R; // ��������Ŵ
    
    [Header("UI References")]
    public Image reloadCircle; // ǧ�������Ŵ
    public TextMeshProUGUI ammoText; // ��ͤ����ʴ�����ع
    public GameObject reloadPanel; // Panel ����Ŵ
    
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
        
        // ����Ŵ����͡����� R ���͡���ع��� (����Դ autoReload)
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

        // Debug log ���͵�Ǩ�ͺ
        Debug.Log("���������Ŵ");
        Debug.Log("Reload Circle: " + (reloadCircle != null));
        Debug.Log("Reload Panel: " + (reloadPanel != null));

        // �ʴ� UI ����Ŵ
        ShowReloadUI();

        // ������§����Ŵ
        if (audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }
    }

    void UpdateReloadProgress()
    {
        if (!isReloading) return;
        
        reloadTimer += Time.deltaTime;
        
        // �Ѿഷ UI ǧ���
        if (reloadCircle != null)
        {
            float progress = reloadTimer / reloadTime;
            reloadCircle.fillAmount = progress;
        }
        
        // ����Ŵ�������
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
        Debug.Log("����Ŵ�������! ����ع�Ѩ�غѹ: " + currentAmmo);
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
            // ������§����;������ԧ�����ع���
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
        Debug.Log("���Ѻ����ع " + amount + " �Ѵ! ��ѧ: " + reserveAmmo);
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
        Debug.Log("�������ʴ� UI ����Ŵ");

        if (reloadPanel != null)
        {
            reloadPanel.SetActive(true);
            Debug.Log("�ʴ� Panel ����");
        }
        else
        {
            Debug.LogWarning("Reload Panel is null!");
        }

        if (reloadCircle != null)
        {
            reloadCircle.fillAmount = 0f;
            reloadCircle.gameObject.SetActive(true);
            Debug.Log("�ʴ� Circle ����");
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public int currentAmmo = 30;
    public int maxAmmo = 90;
    public int damage = 25;
    public float fireRate = 0.1f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 50f; // �������ǡ���ع

    private Camera playerCamera;
    private float nextTimeToFire = 0f;

    void Start()
    {
        playerCamera = GetComponentInParent<Camera>();
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

        // �ҵ��˹觷��������� (��ҧ˹�Ҩ�)
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100); // �ش��������ҡ��誹����
        }

        // �ӹǳ��ȷҧ�ҡ�ҡ��к͡�׹��ѧ���
        Vector3 direction = (targetPoint - firePoint.position).normalized;

        // ���ҧ����ع
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));

        // ��駤�Ҥ������ǡ���ع
        Projectile projectile = bullet.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetDirection(direction, bulletSpeed);
        }

        GameManager.instance.UpdateUI();
    }

    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
        if (currentAmmo > maxAmmo)
            currentAmmo = maxAmmo;

        GameManager.instance.UpdateUI();
    }
}
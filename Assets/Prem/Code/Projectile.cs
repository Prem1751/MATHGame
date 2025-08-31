using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 25;
    public GameObject hitEffect;

    void Start()
    {
        // ลบกระสุนหลังจาก 3 วินาที
        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter(Collider other)
    {
        // ไม่ทำอะไรถ้าชนกับ Player หรือ กระสุนด้วยกัน
        if (other.CompareTag("Player") || other.CompareTag("Bullet"))
            return;

        Debug.Log("กระสุนชน: " + other.gameObject.name + " (แท็ก: " + other.tag + ")");

        // ตรวจสอบว่าชนซอมบี้
        if (other.CompareTag("Enemy"))
        {
            Zombie zombie = other.GetComponent<Zombie>();
            if (zombie != null)
            {
                // หาจุดชนที่ถูกต้อง
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                zombie.TakeDamage(damage, hitPoint);
                Debug.Log("ซอมบี้โดนยิง! damage: " + damage);
            }
        }

        // สร้างเอฟเฟกต์
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, transform.rotation);
        }

        // ลบกระสุน
        Destroy(gameObject);
    }
}
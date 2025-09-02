using UnityEngine;

public class SceneChangeTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    public string playerTag = "Player";
    public ParticleSystem triggerEffect;
    public AudioClip triggerSound;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            TriggerSceneChange();
        }
    }

    void TriggerSceneChange()
    {
        Debug.Log("Player hit scene change trigger!");

        // เล่นเอฟเฟกต์
        if (triggerEffect != null)
        {
            triggerEffect.Play();
        }

        if (triggerSound != null)
        {
            AudioSource.PlayClipAtPoint(triggerSound, transform.position);
        }

        // เรียก TimerController เพื่อเปลี่ยนฉาก
        if (TimerController.instance != null)
        {
            TimerController.instance.PlayerHitObjective();
        }

        // ปิด collider เพื่อไม่ให้触发ซ้ำ
        GetComponent<Collider>().enabled = false;

        // ซ่อนวัตถุหลังจาก 1 วินาที
        StartCoroutine(HideObject(1f));
    }

    System.Collections.IEnumerator HideObject(float delay)
    {
        yield return new WaitForSeconds(delay);
        GetComponent<MeshRenderer>().enabled = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 1f);
        Gizmos.DrawIcon(transform.position + Vector3.up * 2f, "scene_change_icon", true);
    }
}
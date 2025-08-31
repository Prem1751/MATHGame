using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    [Header("Win Settings")]
    public string playerTag = "Player";
    public ParticleSystem winParticles;
    public AudioClip triggerSound;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            PlayerReachedWin();
        }
    }

    void PlayerReachedWin()
    {
        Debug.Log("Player reached the win object!");

        // เล่นเอฟเฟกต์
        if (winParticles != null)
        {
            winParticles.Play();
        }

        if (triggerSound != null)
        {
            AudioSource.PlayClipAtPoint(triggerSound, transform.position);
        }

        // เรียก TimerController เพื่อจบเกมแบบชนะ
        if (TimerController.instance != null)
        {
            TimerController.instance.TimerEnded(true);
        }

        // ปิด collider เพื่อไม่ให้触发ซ้ำ
        GetComponent<Collider>().enabled = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawIcon(transform.position + Vector3.up * 2f, "win_icon", true);
    }
}
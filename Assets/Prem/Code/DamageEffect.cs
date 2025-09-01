using UnityEngine;
using UnityEngine.UI;

public class DamageEffect : MonoBehaviour
{
    public Image redOverlay;
    public float flashTime = 0.3f;

    void Start()
    {
        // ซ่อนเอฟเฟ็กต์ตอนเริ่มเกม
        if (redOverlay != null)
            redOverlay.color = new Color(1, 0, 0, 0);
    }

    public void ShowDamage()
    {
        if (redOverlay != null)
            StartCoroutine(FlashRed());
    }

    private System.Collections.IEnumerator FlashRed()
    {
        // แสดงสีแดงเต็มจอ
        redOverlay.color = new Color(1, 0, 0, 0.8f);

        // รอสักครู่
        yield return new WaitForSeconds(flashTime);

        // ค่อยๆ จางหาย
        float timer = 0f;
        Color startColor = redOverlay.color;
        Color endColor = new Color(1, 0, 0, 0);

        while (timer < flashTime)
        {
            redOverlay.color = Color.Lerp(startColor, endColor, timer / flashTime);
            timer += Time.deltaTime;
            yield return null;
        }

        redOverlay.color = endColor;
    }
}
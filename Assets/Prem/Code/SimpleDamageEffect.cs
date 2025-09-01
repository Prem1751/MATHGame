using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SimpleDamageEffect : MonoBehaviour
{
    public static SimpleDamageEffect Instance;

    [Header("ตั้งค่าเอฟเฟ็กต์")]
    public Image redOverlay;
    public float effectTime = 0.3f;

    void Awake()
    {
        Instance = this;
        if (redOverlay != null) redOverlay.color = Color.clear;
    }

    public void ShowDamage()
    {
        StartCoroutine(FlashRed());
    }

    IEnumerator FlashRed()
    {
        // แสดงสีแดงเต็มจอ
        if (redOverlay != null) redOverlay.color = new Color(1, 0, 0, 0.8f);

        // รอสักครู่
        yield return new WaitForSeconds(effectTime);

        // ค่อยๆ จางหาย
        float timer = 0f;
        while (timer < effectTime)
        {
            if (redOverlay != null)
            {
                float alpha = Mathf.Lerp(0.8f, 0f, timer / effectTime);
                redOverlay.color = new Color(1, 0, 0, alpha);
            }
            timer += Time.deltaTime;
            yield return null;
        }

        if (redOverlay != null) redOverlay.color = Color.clear;
    }
}
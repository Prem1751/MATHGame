using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SimpleDamageEffect : MonoBehaviour
{
    public static SimpleDamageEffect Instance;

    [Header("��駤���Ϳ�硵�")]
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
        // �ʴ���ᴧ�����
        if (redOverlay != null) redOverlay.color = new Color(1, 0, 0, 0.8f);

        // ���ѡ����
        yield return new WaitForSeconds(effectTime);

        // ����� �ҧ���
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
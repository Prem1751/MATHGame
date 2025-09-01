using UnityEngine;
using UnityEngine.UI;

public class DamageEffect : MonoBehaviour
{
    public Image redOverlay;
    public float flashTime = 0.3f;

    void Start()
    {
        // ��͹�Ϳ�硵�͹�������
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
        // �ʴ���ᴧ�����
        redOverlay.color = new Color(1, 0, 0, 0.8f);

        // ���ѡ����
        yield return new WaitForSeconds(flashTime);

        // ����� �ҧ���
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
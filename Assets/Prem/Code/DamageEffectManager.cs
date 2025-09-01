using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageEffectManager : MonoBehaviour
{
    public static DamageEffectManager Instance;

    [Header("UI Effects")]
    public Image bloodOverlay;
    public Image damageVignette;
    public Text damageText;
    public CanvasGroup damageCanvasGroup;

    [Header("Screen Shake")]
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 0.2f;

    [Header("Particle Effects")]
    public ParticleSystem bloodSplatter;
    public ParticleSystem hitEffect;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] damageSounds;
    public AudioClip[] gruntSounds;

    [Header("Settings")]
    public float effectDuration = 1.5f;
    public float maxAlpha = 0.8f;
    public float textDisplayTime = 1f;

    private Vector3 originalCameraPosition;
    private Camera mainCamera;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            originalCameraPosition = mainCamera.transform.localPosition;
        }

        // ซ่อนเอฟเฟ็กต์ทั้งหมดตอนเริ่มเกม
        ResetAllEffects();
    }

    void ResetAllEffects()
    {
        if (bloodOverlay) bloodOverlay.color = new Color(1, 0, 0, 0);
        if (damageVignette) damageVignette.color = new Color(1, 0, 0, 0);
        if (damageText) damageText.gameObject.SetActive(false);
        if (damageCanvasGroup) damageCanvasGroup.alpha = 0;
    }

    public void ShowDamageEffect(int damageAmount, Vector3 hitPosition)
    {
        StartCoroutine(DamageEffectSequence(damageAmount, hitPosition));
    }

    private IEnumerator DamageEffectSequence(int damageAmount, Vector3 hitPosition)
    {
        // 1. Screen Shake
        StartCoroutine(ShakeCamera());

        // 2. Blood Overlay Effect
        StartCoroutine(ShowBloodOverlay());

        // 3. Damage Text
        ShowDamageText(damageAmount);

        // 4. Particle Effects
        PlayParticleEffects(hitPosition);

        // 5. Sound Effects
        PlayDamageSounds();

        yield return new WaitForSeconds(effectDuration);

        // ค่อยๆ จางหาย
        yield return StartCoroutine(FadeOutEffects());
    }

    private IEnumerator ShakeCamera()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            if (mainCamera != null)
            {
                float x = Random.Range(-1f, 1f) * shakeMagnitude;
                float y = Random.Range(-1f, 1f) * shakeMagnitude;

                mainCamera.transform.localPosition = originalCameraPosition + new Vector3(x, y, 0);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (mainCamera != null)
        {
            mainCamera.transform.localPosition = originalCameraPosition;
        }
    }

    private IEnumerator ShowBloodOverlay()
    {
        if (bloodOverlay == null) yield break;

        // แสดงเลือดเต็มจอ
        bloodOverlay.color = new Color(1, 0, 0, maxAlpha);
        damageVignette.color = new Color(1, 0, 0, maxAlpha * 0.6f);

        // ค่อยๆ จาง
        float elapsed = 0f;
        while (elapsed < effectDuration)
        {
            float alpha = Mathf.Lerp(maxAlpha, 0, elapsed / effectDuration);
            bloodOverlay.color = new Color(1, 0, 0, alpha);
            damageVignette.color = new Color(1, 0, 0, alpha * 0.6f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        bloodOverlay.color = new Color(1, 0, 0, 0);
        damageVignette.color = new Color(1, 0, 0, 0);
    }

    private void ShowDamageText(int damage)
    {
        if (damageText == null) return;

        damageText.text = $"-{damage}";
        damageText.gameObject.SetActive(true);
        damageText.transform.localPosition = new Vector3(
            Random.Range(-100, 100),
            Random.Range(-50, 50),
            0
        );

        StartCoroutine(HideDamageText());
    }

    private IEnumerator HideDamageText()
    {
        yield return new WaitForSeconds(textDisplayTime);
        if (damageText) damageText.gameObject.SetActive(false);
    }

    private void PlayParticleEffects(Vector3 hitPosition)
    {
        if (bloodSplatter != null)
        {
            bloodSplatter.transform.position = hitPosition;
            bloodSplatter.Play();
        }

        if (hitEffect != null)
        {
            hitEffect.transform.position = hitPosition;
            hitEffect.Play();
        }
    }

    private void PlayDamageSounds()
    {
        if (audioSource == null) return;

        // เลือกเสียงสุ่ม
        if (damageSounds.Length > 0)
        {
            AudioClip clip = damageSounds[Random.Range(0, damageSounds.Length)];
            audioSource.PlayOneShot(clip);
        }

        if (gruntSounds.Length > 0)
        {
            AudioClip grunt = gruntSounds[Random.Range(0, gruntSounds.Length)];
            audioSource.PlayOneShot(grunt, 0.7f);
        }
    }

    private IEnumerator FadeOutEffects()
    {
        if (damageCanvasGroup == null) yield break;

        float elapsed = 0f;
        float fadeDuration = 0.5f;

        while (elapsed < fadeDuration)
        {
            damageCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        damageCanvasGroup.alpha = 0;
    }

    // ฟังก์ชันสำหรับทดสอบ
    public void TestDamageEffect()
    {
        ShowDamageEffect(25, new Vector3(Screen.width / 2, Screen.height / 2, 0));
    }
}
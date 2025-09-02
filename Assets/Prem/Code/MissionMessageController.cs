using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionMessageController : MonoBehaviour
{
    public static MissionMessageController instance;

    [Header("UI References")]
    public GameObject messagePanel;
    public TMP_Text messageText;
    public Image messageBackground;
    public AudioClip typingSound;

    [Header("Text Settings")]
    [TextArea(3, 5)]
    public string missionMessage = "目標: 生き残れ!\n時間内に生存しろ!\nヘリコプターが迎えに来る!";
    public float typingSpeed = 0.05f; // ความเร็วในการพิมพ์
    public float showDuration = 5f; // เวลาแสดงข้อความหลังจากพิมพ์เสร็จ
    public float fadeDuration = 1f; // เวลา fade in/out

    private AudioSource audioSource;
    private bool isShowingMessage = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // ซ่อน message panel ตอนเริ่ม
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }

        // เริ่มแสดงข้อความ mission
        ShowMissionMessage(missionMessage);
    }

    public void ShowMissionMessage(string message)
    {
        if (!isShowingMessage)
        {
            StartCoroutine(ShowMessageCoroutine(message));
        }
    }

    IEnumerator ShowMessageCoroutine(string message)
    {
        isShowingMessage = true;

        // แสดง panel
        if (messagePanel != null)
        {
            messagePanel.SetActive(true);
        }

        // Fade in
        yield return StartCoroutine(FadePanel(0f, 1f, fadeDuration));

        // ล้างข้อความเก่า
        if (messageText != null)
        {
            messageText.text = "";
        }

        // พิมพ์ข้อความทีละตัวอักษร
        if (messageText != null)
        {
            for (int i = 0; i < message.Length; i++)
            {
                messageText.text += message[i];

                // เล่นเสียงพิมพ์ (ถ้ามี)
                if (typingSound != null && i % 2 == 0) // เล่นเสียงทุกๆ 2 ตัวอักษร
                {
                    audioSource.PlayOneShot(typingSound, 0.3f);
                }

                yield return new WaitForSeconds(typingSpeed);
            }
        }

        // รอหลังจากพิมพ์เสร็จ
        yield return new WaitForSeconds(showDuration);

        // Fade out
        yield return StartCoroutine(FadePanel(1f, 0f, fadeDuration));

        // ซ่อน panel
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }

        isShowingMessage = false;
    }

    IEnumerator FadePanel(float startAlpha, float targetAlpha, float duration)
    {
        float elapsed = 0f;
        CanvasGroup canvasGroup = messagePanel.GetComponent<CanvasGroup>();

        // ถ้าไม่มี CanvasGroup ให้เพิ่ม
        if (canvasGroup == null)
        {
            canvasGroup = messagePanel.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = startAlpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    // สำหรับแสดงข้อความอื่นๆ ในเกม
    public void ShowQuickMessage(string message, float duration = 3f)
    {
        StartCoroutine(QuickMessageCoroutine(message, duration));
    }

    IEnumerator QuickMessageCoroutine(string message, float duration)
    {
        // แสดง panel
        if (messagePanel != null)
        {
            messagePanel.SetActive(true);
            messagePanel.GetComponent<CanvasGroup>().alpha = 1f;
        }

        // แสดงข้อความทันที
        if (messageText != null)
        {
            messageText.text = message;
        }

        // รอ
        yield return new WaitForSeconds(duration);

        // ซ่อน panel
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }
    }

    // สำหรับ debug
    public void TestMessage()
    {
        ShowMissionMessage("テストメッセージです!\nこれはデモンストレーションです!");
    }
}
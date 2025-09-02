using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LoadingSystem : MonoBehaviour
{
    public static LoadingSystem instance;

    [Header("Loading UI References")]
    public GameObject loadingPanel;
    public Image progressBar;
    public TMP_Text progressText;
    public TMP_Text loadingTipText;
    public Image backgroundImage;

    [Header("Loading Settings")]
    public float minLoadingTime = 2f;
    public float fadeDuration = 0.5f;
    public string[] loadingTips;

    [Header("Scene Names")]
    public string mainMenuScene = "MainMenu";
    public string gameScene = "GameScene";
    public string loadingScene = "LoadingScene";

    private AsyncOperation loadingOperation;
    private float loadingProgress = 0f;
    private bool isLoading = false;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // สร้าง Canvas Group ถ้ายังไม่มี
        if (loadingPanel != null && loadingPanel.GetComponent<CanvasGroup>() == null)
        {
            canvasGroup = loadingPanel.AddComponent<CanvasGroup>();
        }
        else if (loadingPanel != null)
        {
            canvasGroup = loadingPanel.GetComponent<CanvasGroup>();
        }
    }

    void Start()
    {
        // ซ่อน loading panel ตอนเริ่ม
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
            canvasGroup.alpha = 0f;
        }
    }

    void Update()
    {
        // ตรวจสอบการกดปุ่มสำหรับ skip loading (optional)
        if (isLoading && Input.GetKeyDown(KeyCode.Space))
        {
            SkipLoading();
        }
    }

    // โหลด scene หลัก
    public void LoadGameScene()
    {
        StartCoroutine(LoadSceneCoroutine(gameScene));
    }

    // โหลด main menu
    public void LoadMainMenu()
    {
        StartCoroutine(LoadSceneCoroutine(mainMenuScene));
    }

    // โหลด scene ตามชื่อ
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    // โหลด scene ปัจจุบันใหม่ (restart)
    public void RestartCurrentScene()
    {
        StartCoroutine(LoadSceneCoroutine(SceneManager.GetActiveScene().name));
    }

    IEnumerator LoadSceneCoroutine(string sceneName)
    {
        isLoading = true;
        loadingProgress = 0f;

        // Fade in loading screen
        yield return StartCoroutine(FadeLoadingPanel(0f, 1f, fadeDuration));

        // แสดง loading panel
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }

        // แสดง loading tip สุ่ม
        if (loadingTipText != null && loadingTips.Length > 0)
        {
            string randomTip = loadingTips[Random.Range(0, loadingTips.Length)];
            loadingTipText.text = "💡 " + randomTip;
        }

        // รอ 1 frame เพื่อให้ UI อัพเดท
        yield return null;

        // เริ่มโหลด scene
        loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        loadingOperation.allowSceneActivation = false;

        float timer = 0f;
        bool reachedMinTime = false;

        while (!loadingOperation.isDone)
        {
            timer += Time.deltaTime;

            // คำนวณ progress
            loadingProgress = Mathf.Clamp01(loadingOperation.progress / 0.9f);
            float displayProgress = Mathf.Clamp(loadingProgress, 0f, 1f);

            // อัพเดท UI
            UpdateLoadingUI(displayProgress);

            // ตรวจสอบว่าโหลดเสร็จและผ่านเวลาขั้นต่ำแล้ว
            if (loadingOperation.progress >= 0.9f)
            {
                if (timer >= minLoadingTime && !reachedMinTime)
                {
                    reachedMinTime = true;
                    // แสดงข้อความว่า ready
                    if (progressText != null)
                    {
                        progressText.text = "Press SPACE to continue...";
                    }
                }

                if (reachedMinTime && Input.GetKeyDown(KeyCode.Space))
                {
                    loadingOperation.allowSceneActivation = true;
                }
            }

            yield return null;
        }

        // รอให้ scene ใหม่โหลดเสร็จ
        yield return new WaitForSeconds(0.5f);

        // Fade out loading screen
        yield return StartCoroutine(FadeLoadingPanel(1f, 0f, fadeDuration));

        // ซ่อน loading panel
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }

        isLoading = false;
    }

    void UpdateLoadingUI(float progress)
    {
        // อัพเดท progress bar
        if (progressBar != null)
        {
            progressBar.fillAmount = progress;
        }

        // อัพเดท progress text
        if (progressText != null)
        {
            progressText.text = $"Loading... {(progress * 100):F0}%";
        }
    }

    IEnumerator FadeLoadingPanel(float startAlpha, float targetAlpha, float duration)
    {
        if (canvasGroup == null) yield break;

        float elapsed = 0f;
        canvasGroup.alpha = startAlpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    public void SkipLoading()
    {
        if (loadingOperation != null && loadingOperation.progress >= 0.9f)
        {
            loadingOperation.allowSceneActivation = true;
        }
    }

    public bool IsLoading()
    {
        return isLoading;
    }

    public float GetLoadingProgress()
    {
        return loadingProgress;
    }

    // ฟังก์ชันสำหรับปุ่ม UI
    public void OnRestartButtonClick()
    {
        RestartCurrentScene();
    }

    public void OnMainMenuButtonClick()
    {
        LoadMainMenu();
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }

    // ฟังก์ชันสำหรับการโหลดด้วย fade effect
    public void LoadSceneWithFade(string sceneName, float customFadeDuration = 1f)
    {
        StartCoroutine(LoadSceneWithFadeCoroutine(sceneName, customFadeDuration));
    }

    IEnumerator LoadSceneWithFadeCoroutine(string sceneName, float fadeDuration)
    {
        // Fade out
        yield return StartCoroutine(FadeLoadingPanel(0f, 1f, fadeDuration));

        // โหลด scene
        yield return StartCoroutine(LoadSceneCoroutine(sceneName));

        // Fade in
        yield return StartCoroutine(FadeLoadingPanel(1f, 0f, fadeDuration));
    }
}
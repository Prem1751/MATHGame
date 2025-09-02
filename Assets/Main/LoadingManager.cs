using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager instance;

    [Header("UI References")]
    public GameObject loadingPanel;
    public Image progressBar;
    public TMP_Text progressText;
    public TMP_Text loadingTipText;
    public Image backgroundImage;

    [Header("Loading Settings")]
    public float minLoadingTime = 2f;
    public string[] loadingTips;

    private AsyncOperation loadingOperation;
    private float loadingProgress = 0f;
    private bool isLoading = false;

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
        }
    }

    void Start()
    {
        // ซ่อน loading panel ตอนเริ่ม
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    IEnumerator LoadSceneCoroutine(string sceneName)
    {
        isLoading = true;
        loadingProgress = 0f;

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

        while (!loadingOperation.isDone)
        {
            timer += Time.deltaTime;

            // คำนวณ progress
            loadingProgress = Mathf.Clamp01(loadingOperation.progress / 0.9f);
            float displayProgress = Mathf.Clamp(loadingProgress, 0f, 1f);

            // อัพเดท UI
            if (progressBar != null)
            {
                progressBar.fillAmount = displayProgress;
            }

            if (progressText != null)
            {
                progressText.text = $"Loading... {(displayProgress * 100):F0}%";
            }

            // รอจนกว่า loading จะเสร็จและผ่านเวลาขั้นต่ำ
            if (loadingOperation.progress >= 0.9f && timer >= minLoadingTime)
            {
                loadingOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        // ซ่อน loading panel
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }

        isLoading = false;
    }

    // สำหรับโหลด scene ด้วย fade effect
    public void LoadSceneWithFade(string sceneName, float fadeDuration = 1f)
    {
        StartCoroutine(LoadSceneWithFadeCoroutine(sceneName, fadeDuration));
    }

    IEnumerator LoadSceneWithFadeCoroutine(string sceneName, float fadeDuration)
    {
        // Fade out (ถ้ามี fade system)
        yield return new WaitForSeconds(fadeDuration);

        // โหลด scene
        yield return StartCoroutine(LoadSceneCoroutine(sceneName));

        // Fade in (ถ้ามี fade system)
        yield return new WaitForSeconds(fadeDuration);
    }

    // ตรวจสอบสถานะ loading
    public bool IsLoading()
    {
        return isLoading;
    }

    // สำหรับปุ่ม skip (optional)
    public void SkipLoading()
    {
        if (loadingOperation != null && loadingOperation.progress >= 0.9f)
        {
            loadingOperation.allowSceneActivation = true;
        }
    }
}
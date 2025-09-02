using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SurvivalTimerController : MonoBehaviour
{
    public static SurvivalTimerController instance;

    [Header("Timer Settings")]
    public float survivalTime = 180f; // 3 นาที
    public float currentTime;
    public bool isTimerRunning = false;

    [Header("Helicopter Sound Settings")]
    public AudioClip helicopterSound;
    public float minHelicopterVolume = 0.1f;
    public float maxHelicopterVolume = 1.0f;
    public float minHelicopterPitch = 0.5f;
    public float maxHelicopterPitch = 1.2f;
    public float soundFadeSpeed = 2f;
    public float helicopterStartTime = 60f; // เริ่มเล่นเสียงเมื่อเหลือ 1 นาที

    [Header("UI References")]
    public TMP_Text timerText;
    public TMP_Text statusText;
    public GameObject winPanel;
    public TMP_Text winText;
    public GameObject losePanel;

    [Header("Sound Effects")]
    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip timeWarningSound;
    public AudioClip tickSound;
    public AudioClip helicopterArrivalSound;

    [Header("Warning Settings")]
    public float warningTime = 30f; // แจ้งเตือนเมื่อเหลือ 30 วินาที
    public Color normalColor = Color.white;
    public Color warningColor = Color.red;
    public float warningFlashSpeed = 0.5f;

    private AudioSource audioSource;
    private AudioSource helicopterAudioSource;
    private bool warningPlayed = false;
    private bool helicopterSoundPlaying = false;
    private bool isGameOver = false;

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

        // สร้าง AudioSource สำหรับเสียง Helicopter
        helicopterAudioSource = gameObject.AddComponent<AudioSource>();
        helicopterAudioSource.loop = true;
        helicopterAudioSource.volume = 0f;
        helicopterAudioSource.pitch = minHelicopterPitch;

        currentTime = survivalTime;
        UpdateTimerDisplay();
        UpdateStatusText();

        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
    }

    void Update()
    {
        if (isTimerRunning && !isGameOver)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();

            // อัพเดทเสียง Helicopter
            if (currentTime <= helicopterStartTime && !helicopterSoundPlaying)
            {
                StartHelicopterSound();
            }

            if (helicopterSoundPlaying)
            {
                UpdateHelicopterSound();
            }

            // เช็คเวลาเตือน
            if (currentTime <= warningTime && !warningPlayed)
            {
                StartWarning();
            }

            // เช็คเวลาเหลือ 0 - ชนะ!
            if (currentTime <= 0)
            {
                currentTime = 0;
                PlayerSurvived();
            }
        }
    }

    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            timerText.text = string.Format("Survival time: {0:00}:{1:00}", minutes, seconds);

            // เปลี่ยนสีเมื่อเวลาใกล้หมด
            if (currentTime <= warningTime)
            {
                float flash = Mathf.PingPong(Time.time * warningFlashSpeed, 1f);
                timerText.color = Color.Lerp(normalColor, warningColor, flash);
            }
            else
            {
                timerText.color = normalColor;
            }
        }
    }

    void UpdateStatusText()
    {
        if (statusText != null)
        {
            if (currentTime <= helicopterStartTime)
            {
                statusText.text = "helicopter is comming";
                statusText.color = Color.yellow;
            }
            else
            {
                statusText.text = "You must survive.";
                statusText.color = Color.green;
            }
        }
    }

    void StartHelicopterSound()
    {
        if (!helicopterSoundPlaying && helicopterSound != null)
        {
            helicopterAudioSource.clip = helicopterSound;
            helicopterAudioSource.Play();
            helicopterSoundPlaying = true;

            // เล่นเสียงเตือน Helicopter มา
            if (audioSource != null && helicopterArrivalSound != null)
            {
                audioSource.PlayOneShot(helicopterArrivalSound);
            }

            Debug.Log("เสียง Helicopter เริ่มเล่นแล้ว!");
        }
    }

    void UpdateHelicopterSound()
    {
        if (helicopterSoundPlaying)
        {
            // คำนวณความเข้มเสียงตามเวลาที่เหลือ (ยิ่งน้อยยิ่งดัง)
            float timeRemaining = Mathf.Clamp(currentTime, 0f, helicopterStartTime);
            float soundIntensity = 1f - (timeRemaining / helicopterStartTime);

            // ค่อยๆ เพิ่มเสียงและ pitch
            helicopterAudioSource.volume = Mathf.Lerp(
                minHelicopterVolume,
                maxHelicopterVolume,
                soundIntensity
            );

            helicopterAudioSource.pitch = Mathf.Lerp(
                minHelicopterPitch,
                maxHelicopterPitch,
                soundIntensity
            );

            // อัพเดทข้อความสถานะ
            UpdateStatusText();
        }
    }

    void StartWarning()
    {
        warningPlayed = true;

        // เล่นเสียงเตือน
        if (audioSource != null && timeWarningSound != null)
        {
            audioSource.PlayOneShot(timeWarningSound);
        }

        // เริ่มเล่นเสียงติ๊กต่อก
        StartCoroutine(PlayTickSound());
    }

    IEnumerator PlayTickSound()
    {
        while (currentTime > 0 && isTimerRunning && !isGameOver)
        {
            if (currentTime <= warningTime && tickSound != null)
            {
                audioSource.PlayOneShot(tickSound);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    public void StartTimer()
    {
        if (!isGameOver)
        {
            isTimerRunning = true;
            Debug.Log("Survival timer started!");
        }
    }

    public void StopTimer()
    {
        isTimerRunning = false;
        Debug.Log("Survival timer stopped!");
    }

    public void AddTime(float secondsToAdd)
    {
        if (!isGameOver)
        {
            currentTime += secondsToAdd;
            UpdateTimerDisplay();
            Debug.Log($"Added {secondsToAdd} seconds! Current time: {currentTime}");
        }
    }

    void PlayerSurvived()
    {
        if (isGameOver) return;

        isGameOver = true;
        StopTimer();
        StopAllCoroutines();

        // หยุดเสียง Helicopter
        if (helicopterAudioSource != null)
        {
            StartCoroutine(FadeOutHelicopterSound());
        }

        Debug.Log("ผู้เล่นรอดชีวิต! ชนะเกม");

        // แสดง UI ชนะ
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            if (winText != null)
            {
                winText.text = "The Hereditary has arrived.";
            }
        }

        // เล่นเสียงชนะ
        if (audioSource != null && winSound != null)
        {
            audioSource.PlayOneShot(winSound);
        }

        // หยุดเกม
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    IEnumerator FadeOutHelicopterSound()
    {
        float startVolume = helicopterAudioSource.volume;
        float fadeDuration = 2f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            helicopterAudioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        helicopterAudioSource.Stop();
        helicopterSoundPlaying = false;
    }

    public void PlayerDied()
    {
        if (isGameOver) return;

        isGameOver = true;
        StopTimer();
        StopAllCoroutines();

        // หยุดเสียง Helicopter
        if (helicopterAudioSource != null)
        {
            helicopterAudioSource.Stop();
        }

        Debug.Log("ผู้เล่นตาย! แพ้เกม");

        // แสดง UI แพ้
        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }

        // เล่นเสียงแพ้
        if (audioSource != null && loseSound != null)
        {
            audioSource.PlayOneShot(loseSound);
        }

        // หยุดเกม
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // สำหรับปุ่ม UI
    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
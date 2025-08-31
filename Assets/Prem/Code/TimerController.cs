using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerController : MonoBehaviour
{
    public static TimerController instance;

    [Header("Timer Settings")]
    public float initialTime = 60f; // เวลาเริ่มต้น (วินาที)
    public float currentTime;
    public bool isTimerRunning = false;

    [Header("UI References")]
    public TMP_Text timerText;
    public GameObject winPanel;
    public TMP_Text winText;
    public GameObject losePanel;
    public AudioClip winSound;
    public AudioClip timeWarningSound;
    public AudioClip tickSound;

    [Header("Warning Settings")]
    public float warningTime = 10f; // แจ้งเตือนเมื่อเหลือกี่วินาที
    public Color normalColor = Color.white;
    public Color warningColor = Color.red;
    public float warningFlashSpeed = 0.5f;

    private AudioSource audioSource;
    private bool warningPlayed = false;

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

        currentTime = initialTime;
        UpdateTimerDisplay();

        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
    }

    void Update()
    {
        if (isTimerRunning)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();

            // เช็คเวลาเตือน
            if (currentTime <= warningTime && !warningPlayed)
            {
                StartWarning();
            }

            // เช็คเวลาเหลือ 0
            if (currentTime <= 0)
            {
                currentTime = 0;
                TimerEnded(false); // แพ้เพราะเวลาหมด
            }
        }
    }

    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

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
        while (currentTime > 0 && isTimerRunning)
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
        isTimerRunning = true;
        Debug.Log("Timer started!");
    }

    public void StopTimer()
    {
        isTimerRunning = false;
        Debug.Log("Timer stopped!");
    }

    public void ResetTimer()
    {
        currentTime = initialTime;
        warningPlayed = false;
        UpdateTimerDisplay();
        Debug.Log("Timer reset!");
    }

    public void AddTime(float secondsToAdd)
    {
        currentTime += secondsToAdd;
        UpdateTimerDisplay();
        Debug.Log($"Added {secondsToAdd} seconds! Current time: {currentTime}");
    }

    public void TimerEnded(bool playerWon)
    {
        StopTimer();
        StopAllCoroutines();

        if (playerWon)
        {
            WinGame();
        }
        else
        {
            LoseGame();
        }
    }

    void WinGame()
    {
        Debug.Log("Player won the game!");

        if (winPanel != null)
        {
            winPanel.SetActive(true);
            if (winText != null)
            {
                winText.text = "ชนะ!\nเวลาเหลือ: " + FormatTime(currentTime);
            }
        }

        if (audioSource != null && winSound != null)
        {
            audioSource.PlayOneShot(winSound);
        }

        // หยุดเกม
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void LoseGame()
    {
        Debug.Log("Player lost the game!");

        if (losePanel != null)
        {
            losePanel.SetActive(true);
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
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
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
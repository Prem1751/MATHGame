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
    public bool playerReachedObjective = false;

    [Header("Helicopter Rescue Settings")]
    public GameObject helicopterPrefab;
    public Transform[] helicopterSpawnPoints;
    public Transform helicopterLandingSpot;
    public float helicopterFlyHeight = 50f;
    public float helicopterSpeed = 20f;
    public float helicopterSpawnTime = 30f; // เหลือเวลาเท่าไหร่ถึงให้เฮลิคอปเตอร์มา
    public AudioClip helicopterSound;

    [Header("UI References")]
    public TMP_Text timerText;
    public TMP_Text objectiveText;
    public GameObject winPanel;
    public TMP_Text winText;
    public GameObject losePanel;
    public GameObject objectiveReachedPanel;
    public GameObject helicopterArrivalPanel;

    [Header("Sound Effects")]
    public AudioClip winSound;
    public AudioClip timeWarningSound;
    public AudioClip tickSound;
    public AudioClip objectiveReachedSound;
    public AudioClip helicopterArriveSound;

    [Header("Warning Settings")]
    public float warningTime = 10f; // แจ้งเตือนเมื่อเหลือกี่วินาที
    public Color normalColor = Color.white;
    public Color warningColor = Color.red;
    public float warningFlashSpeed = 0.5f;

    private AudioSource audioSource;
    private bool warningPlayed = false;
    private bool helicopterSpawned = false;
    private GameObject currentHelicopter;
    private Vector3 helicopterTargetPosition;

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
        UpdateObjectiveUI();

        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
        if (objectiveReachedPanel != null) objectiveReachedPanel.SetActive(false);
        if (helicopterArrivalPanel != null) helicopterArrivalPanel.SetActive(false);
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

            // เช็คเวลาเหลือสำหรับเฮลิคอปเตอร์
            if (currentTime <= helicopterSpawnTime && !helicopterSpawned && playerReachedObjective)
            {
                StartCoroutine(SpawnHelicopter());
            }

            // เช็คเวลาเหลือ 0
            if (currentTime <= 0)
            {
                currentTime = 0;
                if (playerReachedObjective)
                {
                    PlayerSurvived();
                }
                else
                {
                    TimerEnded(false); // แพ้เพราะเวลาหมด
                }
            }
        }

        // เคลื่อนไหวเฮลิคอปเตอร์
        if (currentHelicopter != null && helicopterSpawned)
        {
            MoveHelicopter();
        }
    }

    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            timerText.text = string.Format("救援まで: {0:00}:{1:00}", minutes, seconds);

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

    void UpdateObjectiveUI()
    {
        if (objectiveText != null)
        {
            if (playerReachedObjective)
            {
                objectiveText.text = "目標達成! ヘリコプターを待て";
                objectiveText.color = Color.green;
            }
            else
            {
                objectiveText.text = "目標地点へ向かえ";
                objectiveText.color = Color.yellow;
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

    IEnumerator SpawnHelicopter()
    {
        helicopterSpawned = true;

        // แสดง UI เฮลิคอปเตอร์มา
        if (helicopterArrivalPanel != null)
        {
            helicopterArrivalPanel.SetActive(true);
            yield return new WaitForSeconds(3f);
            helicopterArrivalPanel.SetActive(false);
        }

        // เล่นเสียงเฮลิคอปเตอร์มา
        if (audioSource != null && helicopterArriveSound != null)
        {
            audioSource.PlayOneShot(helicopterArriveSound);
        }

        yield return new WaitForSeconds(2f); // รอ 2 วินาทีก่อนเฮลิคอปเตอร์มา

        // สุ่มจุด spawn
        if (helicopterSpawnPoints.Length > 0 && helicopterPrefab != null)
        {
            Transform spawnPoint = helicopterSpawnPoints[Random.Range(0, helicopterSpawnPoints.Length)];

            // สร้างเฮลิคอปเตอร์ที่ความสูง
            Vector3 spawnPosition = spawnPoint.position + Vector3.up * helicopterFlyHeight;
            currentHelicopter = Instantiate(helicopterPrefab, spawnPosition, spawnPoint.rotation);

            // ตั้งค่าเป้าหมาย
            helicopterTargetPosition = helicopterLandingSpot.position + Vector3.up * 10f;

            // เล่นเสียงเฮลิคอปเตอร์
            AudioSource helicopterAudio = currentHelicopter.AddComponent<AudioSource>();
            helicopterAudio.clip = helicopterSound;
            helicopterAudio.loop = true;
            helicopterAudio.spatialBlend = 1f;
            helicopterAudio.maxDistance = 100f;
            helicopterAudio.Play();

            Debug.Log("เฮลิคอปเตอร์มาถึงแล้ว!");
        }
    }

    void MoveHelicopter()
    {
        // บินไปยังจุดลงจอด
        currentHelicopter.transform.position = Vector3.MoveTowards(
            currentHelicopter.transform.position,
            helicopterTargetPosition,
            helicopterSpeed * Time.deltaTime
        );

        // หมุนหน้าไปทางจุดหมาย
        Vector3 direction = (helicopterTargetPosition - currentHelicopter.transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            currentHelicopter.transform.rotation = Quaternion.Slerp(
                currentHelicopter.transform.rotation,
                targetRotation,
                2f * Time.deltaTime
            );
        }

        // เช็คว่าถึงจุดหมายแล้วยัง
        float distance = Vector3.Distance(currentHelicopter.transform.position, helicopterTargetPosition);
        if (distance < 1f)
        {
            HelicopterLanded();
        }
    }

    void HelicopterLanded()
    {
        Debug.Log("เฮลิคอปเตอร์ลงจอดแล้ว!");
        // สามารถเพิ่ม animation ลงจอดได้ที่นี่
    }

    public void PlayerReachedObjective()
    {
        if (!playerReachedObjective)
        {
            playerReachedObjective = true;

            // เล่นเสียงและแสดง UI
            if (audioSource != null && objectiveReachedSound != null)
            {
                audioSource.PlayOneShot(objectiveReachedSound);
            }

            if (objectiveReachedPanel != null)
            {
                objectiveReachedPanel.SetActive(true);
                StartCoroutine(HideObjectivePanel(3f));
            }

            UpdateObjectiveUI();
            Debug.Log("ผู้เล่นถึงจุดหมายแล้ว! รอเฮลิคอปเตอร์มารับ");
        }
    }

    IEnumerator HideObjectivePanel(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (objectiveReachedPanel != null)
        {
            objectiveReachedPanel.SetActive(false);
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
            PlayerSurvived();
        }
        else
        {
            LoseGame();
        }
    }

    void PlayerSurvived()
    {
        Debug.Log("ผู้เล่นรอดชีวิต! ชนะเกม");
        StopTimer();

        if (winPanel != null)
        {
            winPanel.SetActive(true);
            if (winText != null)
            {
                winText.text = "任務成功!\nヘリコプターで脱出せよ!\n時間: " + FormatTime(currentTime);
            }
        }

        if (audioSource != null && winSound != null)
        {
            audioSource.PlayOneShot(winSound);
        }

        // เปิดให้玩家ขึ้นเฮลิคอปเตอร์ได้
        StartCoroutine(WaitForHelicopterBoard());
    }

    IEnumerator WaitForHelicopterBoard()
    {
        // รอ玩家走上直升机
        yield return new WaitForSeconds(5f);

        // 显示最终胜利UI
        ShowFinalWinScreen();
    }

    void ShowFinalWinScreen()
    {
        if (winText != null)
        {
            winText.text = "脱出成功!\n任務完了!\n残り時間: " + FormatTime(currentTime);
        }

        // หยุดเกม
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void LoseGame()
    {
        Debug.Log("เวลาหมด! ผู้เล่นแพ้");

        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }

        // หยุดเกม
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PlayerBoardedHelicopter()
    {
        // เมื่อ玩家走上直升机
        Debug.Log("玩家登上直升机!");
        ShowFinalWinScreen();
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
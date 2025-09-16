using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public bool isPlayer = false;

    [Header("UI Settings")]
    public Slider healthSlider;
    public Image healthFillImage;
    public Color fullHealthColor = Color.green;
    public Color lowHealthColor = Color.red;

    [Header("Game Over Settings")]
    public GameObject gameOverPanel;
    public TMP_Text gameOverText;
    public string gameOverMessage = "YOU DIED";
    public string winMessage = "VICTORY!";
    public AudioClip gameOverSound;
    public float gameOverDelay = 2f;

    [Header("Game Over Buttons")]
    public Button restartButton;
    public Button mainMenuButton;
    public Button backToStartButton;
    public Button quitButton;

    [Header("Keyboard Controls")]
    public KeyCode mainMenuKey = KeyCode.M;
    public KeyCode quitKey = KeyCode.Q;

    [Header("UI Instructions Text")]
    public TMP_Text keyboardInstructionsText;

    [Header("Damage Screen Effect")]
    public Image redOverlay;
    public float flashTime = 0.3f;

    [Header("Effects")]
    public GameObject damageEffect;
    public AudioClip hurtSound;
    public AudioClip deathSound;

    [Header("Scene Names")]
    public string mainMenuSceneName = "MainMenu";
    public string startSceneName = "StartScene";

    private AudioSource audioSource;
    private bool isDead = false;
    private bool gameOverShown = false; // ✅ ตัวแปรเช็คว่าแสดง Game Over แล้วหรือยัง
    private bool victoryShown = false; // ✅ ตัวแปรเช็คว่าแสดง Victory แล้วหรือยัง

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1.0f;
        }

        // ตั้งค่า UI
        UpdateHealthUI();

        // ซ่อนเอฟเฟ็กต์สีแดงและ Game Over Panel
        if (redOverlay != null)
        {
            redOverlay.color = new Color(1, 0, 0, 0);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // ตั้งค่าปุ่ม UI
        SetupButtons();

        // ✅ ตั้งค่าข้อความคำแนะนำคีย์บอร์ด
        SetupKeyboardInstructions();

        Debug.Log(gameObject.name + " health initialized: " + currentHealth + "/" + maxHealth);
    }

    void Update()
    {
        // ✅ เช็คการกดคีย์บอร์ดเฉพาะเมื่อแสดง Game Over หรือ Victory แล้ว
        if (isPlayer && (gameOverShown || victoryShown))
        {
            HandleKeyboardInput();
        }
    }

    // ✅ ฟังก์ชันจัดการการกดคีย์บอร์ด
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(mainMenuKey))
        {
            MainMenu();
        }
        else if (Input.GetKeyDown(quitKey))
        {
            QuitGame();
        }
    }

    // ✅ ตั้งค่าข้อความคำแนะนำ
    void SetupKeyboardInstructions()
    {
        if (keyboardInstructionsText != null)
        {
            string instructions = $"Press [{mainMenuKey}] Main Menu | [{quitKey}] Quit Game";
            keyboardInstructionsText.text = instructions;
        }
    }

    void SetupButtons()
    {
        // ตั้งค่า Restart Button
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartGame);
        }

        // ตั้งค่า Main Menu Button
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(MainMenu);
        }

        // ตั้งค่า Back to Start Button
        if (backToStartButton != null)
        {
            backToStartButton.onClick.RemoveAllListeners();
            backToStartButton.onClick.AddListener(BackToStart);
        }

        // ตั้งค่า Quit Button
        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0 || isDead) return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Health: " + currentHealth + "/" + maxHealth);

        // แสดงเอฟเฟ็กต์สีแดงเมื่อผู้เล่นถูกตี
        if (isPlayer && redOverlay != null)
        {
            StartCoroutine(ShowDamageEffect());
        }

        // สร้างเอฟเฟกต์ damage
        if (damageEffect != null)
        {
            Instantiate(damageEffect, transform.position + Vector3.up, Quaternion.identity);
        }

        // เล่นเสียงถูกโจมตี
        if (audioSource != null && hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }

        // อัพเดท UI
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ฟังก์ชันแสดงเอฟเฟ็กต์สีแดง
    private IEnumerator ShowDamageEffect()
    {
        // แสดงสีแดงเต็มจอ
        redOverlay.color = new Color(1, 0, 0, 0.8f);

        // รอสักครู่
        yield return new WaitForSeconds(flashTime);

        // ค่อยๆ จางหาย
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

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)currentHealth / maxHealth;
        }

        if (healthFillImage != null)
        {
            healthFillImage.color = Color.Lerp(lowHealthColor, fullHealthColor, (float)currentHealth / maxHealth);
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log(gameObject.name + " died!");

        // เล่นเสียงตาย
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        if (isPlayer)
        {
            Debug.Log("Player died! Game Over");
            StartCoroutine(ShowGameOver());
        }
        else
        {
            // สำหรับศัตรู
            Destroy(gameObject, 2f);
        }
    }

    IEnumerator ShowGameOver()
    {
        // รอก่อนแสดง Game Over
        yield return new WaitForSeconds(gameOverDelay);

        // ✅ เปิดใช้งานคีย์บอร์ด
        gameOverShown = true;

        // แสดง Game Over Panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            // ตั้งค่าข้อความ
            if (gameOverText != null)
            {
                gameOverText.text = gameOverMessage;
                gameOverText.color = Color.red;
            }
        }

        // เล่นเสียง Game Over
        if (audioSource != null && gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }

        // หยุดเกม
        Time.timeScale = 0f;

        // ปลดล็อกเมาส์
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // ปิดการควบคุมผู้เล่น
        if (TryGetComponent<FirstPersonController>(out var controller))
        {
            controller.enabled = false;
        }

        // ปิดการควบคุมปืน
        Gun gun = GetComponentInChildren<Gun>();
        if (gun != null)
        {
            gun.enabled = false;
        }

        Debug.Log("Game Over displayed - Keyboard controls enabled");
    }

    // ฟังก์ชันสำหรับชนะเกม
    public void ShowWinScreen()
    {
        if (isPlayer && !isDead)
        {
            StartCoroutine(ShowVictory());
        }
    }

    IEnumerator ShowVictory()
    {
        yield return new WaitForSeconds(gameOverDelay);

        // ✅ เปิดใช้งานคีย์บอร์ด
        victoryShown = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (gameOverText != null)
            {
                gameOverText.text = winMessage;
                gameOverText.color = Color.green;
            }
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (TryGetComponent<FirstPersonController>(out var controller))
        {
            controller.enabled = false;
        }

        Gun gun = GetComponentInChildren<Gun>();
        if (gun != null)
        {
            gun.enabled = false;
        }

        Debug.Log("Victory displayed - Keyboard controls enabled");
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log(gameObject.name + " healed for " + amount + ". Health: " + currentHealth + "/" + maxHealth);

        UpdateHealthUI();
    }

    // ฟังก์ชันสำหรับปุ่ม UI
    public void RestartGame()
    {
        Debug.Log("Restarting game...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        Debug.Log("Going to main menu scene: " + mainMenuSceneName);
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void BackToStart()
    {
        Debug.Log("Going back to start scene: " + startSceneName);
        Time.timeScale = 1f;
        SceneManager.LoadScene(startSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

        // สำหรับทดสอบใน Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // สำหรับ testing
    [ContextMenu("Test Take Damage")]
    public void TestTakeDamage()
    {
        TakeDamage(25);
    }

    [ContextMenu("Test Game Over")]
    public void TestGameOver()
    {
        currentHealth = 1;
        TakeDamage(10);
    }

    [ContextMenu("Test Win")]
    public void TestWin()
    {
        ShowWinScreen();
    }

    [ContextMenu("Test Back to Start")]
    public void TestBackToStart()
    {
        BackToStart();
    }

    // ✅ ฟังก์ชันทดสอบคีย์บอร์ด
    [ContextMenu("Test Keyboard Controls")]
    public void TestKeyboardControls()
    {
        gameOverShown = true;
        Debug.Log("Keyboard controls enabled for testing");
    }
}
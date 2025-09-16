using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuUI;
    public Text instructionText;

    [Header("Settings")]
    public string mainMenuSceneName = "Start"; // ชื่อ Scene ของ Main Menu

    private bool isPaused = false;

    void Start()
    {
        // ตั้งค่าข้อความคำแนะนำ
        if (instructionText != null)
        {
            instructionText.text = "Press Space Bar to go to the main menu.\nPress Q to exit the game.\nPress C to continue.";
        }

        // ซ่อนเมนูหยุดตอนเริ่มต้น
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
    }

    void Update()
    {
        // กด ESC เพื่อหยุด/เล่นต่อ
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        // เมื่อเกมหยุด ให้ตรวจสอบปุ่มอื่นๆ
        if (isPaused)
        {
            // กด Space Bar เพื่อไปหน้าเมนูหลัก
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GoToMainMenu();
            }

            // กด Q เพื่อออกจากเกม
            if (Input.GetKeyDown(KeyCode.Q))
            {
                QuitGame();
            }

            // กด C เพื่อเล่นต่อ
            if (Input.GetKeyDown(KeyCode.C))
            {
                ResumeGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // หยุดเวลาในเกม

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }

        // ไม่แสดงเมาส์เมื่อหยุดเกม
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // เล่นเกมต่อ

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        // ล็อคเมาส์กลับ (ถ้าจำเป็นสำหรับเกม)
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // รีเซ็ตเวลาก่อนเปลี่ยน Scene
        SceneManager.LoadScene("Start");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ฟังก์ชันสำหรับปุ่ม UI (ถ้าต้องการใช้)
    public void OnResumeButtonClick()
    {
        ResumeGame();
    }

    public void OnMainMenuButtonClick()
    {
        GoToMainMenu();
    }

    public void OnQuitButtonClick()
    {
        QuitGame();
    }
}
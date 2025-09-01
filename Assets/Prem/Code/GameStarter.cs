using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviour
{
    public void StartGame()
    {
        // โหลด Scene ต่อไป (เปลี่ยน "GameScene" เป็นชื่อ Scene ของคุณ)
        SceneManager.LoadScene("Main");
    }

    public void QuitGame()
    {
        // ปิดเกม
        Application.Quit();

        // สำหรับการทดสอบใน Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
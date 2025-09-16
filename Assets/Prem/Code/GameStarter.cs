using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStarter : MonoBehaviour
{
    public string Scene = "Main";
    public void StartGame()
    {
        // โหลด Scene ต่อไป (เปลี่ยน "GameScene" เป็นชื่อ Scene ของคุณ)
        SceneManager.LoadScene(Scene);
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
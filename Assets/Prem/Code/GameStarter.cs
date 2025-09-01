using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviour
{
    public void StartGame()
    {
        // ��Ŵ Scene ���� (����¹ "GameScene" �繪��� Scene �ͧ�س)
        SceneManager.LoadScene("Main");
    }

    public void QuitGame()
    {
        // �Դ��
        Application.Quit();

        // ����Ѻ��÷��ͺ� Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}